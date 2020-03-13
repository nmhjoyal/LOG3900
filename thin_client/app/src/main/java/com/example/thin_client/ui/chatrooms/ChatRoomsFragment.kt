package com.example.thin_client.ui.chatrooms

import android.app.AlertDialog
import android.content.res.Resources
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.os.Parcel
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.view.WindowManager
import android.widget.*
import androidx.fragment.app.Fragment
import androidx.recyclerview.widget.DividerItemDecoration
import com.arlib.floatingsearchview.FloatingSearchView
import com.arlib.floatingsearchview.suggestions.model.SearchSuggestion
import com.example.thin_client.R
import com.example.thin_client.data.Feedback
import com.example.thin_client.data.model.Room
import com.example.thin_client.data.rooms.RoomManager
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.google.gson.Gson
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.chatrooms_fragment.*


class ChatRoomsFragment : Fragment() {
    private val adapter = GroupAdapter<GroupieViewHolder>()
    private var roomList: ArrayList<String> = ArrayList()
    private var selectedRoom : String = ""
    private var newRoomName : String = ""

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        adapter.setOnItemClickListener{ item, v ->
            selectedRoom = (item as ChatRoomItem).roomname
            RoomManager.currentRoom = selectedRoom
            SocketHandler.joinChatRoom(selectedRoom)
        }

        setupSocketEvents()

        add_room.setOnClickListener(({
            val alertBuilder = AlertDialog.Builder(context)
            alertBuilder.setTitle(R.string.create_room)
            val dialogView = layoutInflater.inflate(R.layout.dialog_create_room, null)
            alertBuilder.setView(dialogView)
            val radioGroup = dialogView.findViewById<RadioGroup>(R.id.room_visibility)
            var isPrivate = false
            radioGroup.check(R.id.is_public_room)
            radioGroup.setOnCheckedChangeListener(({ _, checkedId ->
                isPrivate = checkedId == R.id.is_private_room
            }))

            alertBuilder
                .setPositiveButton(R.string.ok) { _, _ ->
                    newRoomName = dialogView.findViewById<EditText>(R.id.room_name).text.toString()
                    if (newRoomName.isNotBlank()) {
                        SocketHandler.createChatRoom(newRoomName, isPrivate)
                    } else {
                        dialogView.findViewById<EditText>(R.id.room_name).error = Resources.getSystem().getString(R.string.error_roomname)
                    }
                }
                .setNegativeButton(R.string.cancel) { _, _ -> }
            val dialog = alertBuilder.create()
            dialog.window!!.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE)
            dialog.show()
        }))

        rooms_list.setOnClickListener(({ v ->
            val menu = PopupMenu(context, v)
            val groupId = 1
            for (room in roomList) {
                menu.menu.add(groupId, roomList.indexOf(room), 1, room)
            }
            menu.setOnMenuItemClickListener(({ item ->
                if (!RoomManager.roomsJoined.containsKey(item.title)) {
                    RoomManager.currentRoom = item.title.toString()
                    SocketHandler.joinChatRoom(item.title.toString())
                }
                true
            }))
            menu.show()
        }))

        search_room.setOnSearchListener(object: FloatingSearchView.OnSearchListener {
            override fun onSearchAction(currentQuery: String?) {
            }

            override fun onSuggestionClicked(searchSuggestion: SearchSuggestion?) {
                if (searchSuggestion !== null && !RoomManager.roomsJoined.containsKey(searchSuggestion.body)) {
                    RoomManager.currentRoom = searchSuggestion.body
                    SocketHandler.joinChatRoom(searchSuggestion.body)
                }
            }
        })

        search_room.setOnQueryChangeListener(object: FloatingSearchView.OnQueryChangeListener {
            override fun onSearchTextChanged(oldQuery: String?, newQuery: String?) {
                val filterList: MutableList<SearchSuggestion> = ArrayList()
                if (newQuery != null && newQuery.isNotEmpty()) {
                    for (i in roomList.indices) {
                        if (roomList[i].toUpperCase().contains(newQuery.toString().toUpperCase())) {
                            filterList.add(object: SearchSuggestion {
                                override fun describeContents(): Int {
                                    return 0
                                }

                                override fun writeToParcel(dest: Parcel?, flags: Int) {
                                }

                                override fun getBody(): String {
                                    return roomList[i]
                                }
                            })
                        }
                    }
                } else {
                    for (i in roomList.indices) {
                        filterList.add(object: SearchSuggestion {
                            override fun describeContents(): Int {
                                return 0
                            }

                            override fun writeToParcel(dest: Parcel?, flags: Int) {
                            }

                            override fun getBody(): String {
                                return roomList[i]
                            }
                        })
                    }
                }
                search_room.swapSuggestions(filterList)
            }
        })

        SocketHandler.searchRooms()
        fetchRooms()
        recyclerview_chatrooms.addItemDecoration(DividerItemDecoration(recyclerview_chatrooms.context, DividerItemDecoration.VERTICAL))
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        return inflater.inflate(R.layout.chatrooms_fragment, container, false)
    }

    override fun onDestroy() {
        super.onDestroy()
        turnOffSocketEvents()
    }

    private fun fetchRooms() {
        for (room in RoomManager.roomsJoined.keys) {
            adapter.add(ChatRoomItem(room))
        }
        recyclerview_chatrooms.adapter = adapter
    }

    private fun refreshRoomAdapter() {
        adapter.clear()
        for (room in RoomManager.roomsJoined.keys) {
            adapter.add(ChatRoomItem(room))
        }
    }

    private fun setupSocketEvents() {
        SocketHandler.socket!!
            .on(SocketEvent.ROOM_CREATED, ({ data ->
                val roomCreateFeedback = Gson().fromJson(data.first().toString(), Feedback::class.java)
                Handler(Looper.getMainLooper()).post(Runnable {
                    if (roomCreateFeedback.status) {
                        if (newRoomName.isNotBlank() && !RoomManager.roomsJoined.containsKey(newRoomName)) {
                            RoomManager.addRoom(Room(newRoomName, arrayListOf(), mapOf()))
                            adapter.add(ChatRoomItem(newRoomName))
                        }
                    } else {
                        Toast.makeText(context, roomCreateFeedback.log_message, Toast.LENGTH_SHORT)
                            .show()
                    }
                })
            }))
            .on(SocketEvent.ROOM_DELETED, ({ data ->
                Handler(Looper.getMainLooper()).post(Runnable {
                    removeRoom(Gson().fromJson(data.first().toString(), Feedback::class.java))
                })
            }))
            .on(SocketEvent.USER_LEFT_ROOM, ({ data ->
                Handler(Looper.getMainLooper()).post(Runnable {
                    removeRoom(Gson().fromJson(data.first().toString(), Feedback::class.java))
                })
            }))
            .on(SocketEvent.ROOMS, ({ data ->
                Handler(Looper.getMainLooper()).post(Runnable {
                    val list = Gson().fromJson(data.first().toString(), ArrayList::class.java)
                    for (room in list) {
                        roomList.add(room.toString())
                    }
                })
            }))
    }

    private fun removeRoom(feedback: Feedback) {
        Handler(Looper.getMainLooper()).post(Runnable {
            if (feedback.status) {
                RoomManager.leaveRoom()
                refreshRoomAdapter()
            } else {
                Toast.makeText(context, feedback.log_message, Toast.LENGTH_SHORT).show()
            }
        })
    }

    private fun turnOffSocketEvents() {
        if (SocketHandler.socket != null) {
            SocketHandler.socket!!
                .off(SocketEvent.ROOM_CREATED)
                .off(SocketEvent.ROOM_DELETED)
                .off(SocketEvent.USER_LEFT_ROOM)
                .off(SocketEvent.ROOMS)
        }
    }
}
