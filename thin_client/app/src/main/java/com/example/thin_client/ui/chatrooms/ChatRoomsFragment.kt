package com.example.thin_client.ui.chatrooms

import android.app.AlertDialog
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
import androidx.recyclerview.widget.RecyclerView
import com.arlib.floatingsearchview.FloatingSearchView
import com.arlib.floatingsearchview.suggestions.model.SearchSuggestion
import com.example.thin_client.R
import com.example.thin_client.data.Feedback
import com.example.thin_client.data.model.Room
import com.example.thin_client.data.rooms.Invitation
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
    private var newRoomName : String = ""
    private var inviteList: ArrayList<String> = ArrayList()

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        adapter.setOnItemClickListener { item, v ->
            RoomManager.currentRoom = (item as ChatRoomItem).roomname
            SocketHandler.joinChatRoom(RoomManager.currentRoom)
        }

        setupSocketEvents()

        invites.setOnClickListener(({
            showInboxDialog()
        }))

        add_room.setOnClickListener(({
            showCreateNewRoomDialog()
        }))

        rooms_list.setOnClickListener(({ v ->
            val menu = PopupMenu(context, v)
            val groupId = 1
            for (room in roomList) {
                if (!RoomManager.roomsJoined.containsKey(room)) {
                    menu.menu.add(groupId, roomList.indexOf(room), 1, room)
                }
            }
            menu.setOnMenuItemClickListener(({ item ->
                RoomManager.currentRoom = item.title.toString()
                SocketHandler.joinChatRoom(item.title.toString())
                true
            }))
            menu.show()
        }))

        search_room.setOnSearchListener(object: FloatingSearchView.OnSearchListener {
            override fun onSearchAction(currentQuery: String?) {
            }

            override fun onSuggestionClicked(searchSuggestion: SearchSuggestion?) {
                if (searchSuggestion !== null) {
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
                        if (roomList[i].toUpperCase().contains(newQuery.toString().toUpperCase())
                            && !RoomManager.roomsJoined.containsKey(roomList[i])) {
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
                        if (!RoomManager.roomsJoined.containsKey(roomList[i])) {
                            filterList.add(object : SearchSuggestion {
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
                }
                search_room.swapSuggestions(filterList)
            }
        })

        recyclerview_chatrooms.adapter = adapter
        recyclerview_chatrooms.addItemDecoration(DividerItemDecoration(recyclerview_chatrooms.context, DividerItemDecoration.VERTICAL))
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        return inflater.inflate(R.layout.chatrooms_fragment, container, false)
    }

    override fun onStart() {
        super.onStart()
        SocketHandler.searchRooms()
        refreshRoomAdapter()
        if (RoomManager.invites.isEmpty()) {
            invites.setImageResource(R.drawable.ic_inbox_24px)
        } else {
            invites.setImageResource(R.drawable.inbox_notification)
        }
    }

    override fun onDestroy() {
        super.onDestroy()
        turnOffSocketEvents()
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
                        SocketHandler.searchRooms()
                        if (newRoomName.isNotBlank() && !RoomManager.roomsJoined.containsKey(newRoomName)) {
                            RoomManager.addRoom(Room(newRoomName, arrayListOf(), mapOf()))
                            adapter.add(ChatRoomItem(newRoomName))
                            for (invite in inviteList) {
                                SocketHandler.sendInvite(Invitation(newRoomName, invite))
                            }
                            inviteList = arrayListOf()
                        }
                    } else {
                        if (activity !== null) {
                            Toast.makeText(
                                context,
                                roomCreateFeedback.log_message,
                                Toast.LENGTH_SHORT
                            ).show()
                        }
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
                    roomList = arrayListOf()
                    val list = Gson().fromJson(data.first().toString(), ArrayList::class.java)
                    for (room in list) {
                        roomList.add(room.toString())
                    }
                })
            }))
            .on(SocketEvent.RECEIVE_INVITE, ({ data ->
                val invite = Gson().fromJson(data.first().toString(), Invitation::class.java)
                Handler(Looper.getMainLooper()).post(Runnable {
                    if (invites !== null ) {
                        invites.setImageResource(R.drawable.inbox_notification)
                    }
                    if (!RoomManager.invites.contains(invite.id)) {
                        RoomManager.invites.add(invite.id)
                    }
                })
            }))
            .on(SocketEvent.LOAD_HISTORY, ({ data ->
                val room = Gson().fromJson(data.first().toString(), Room::class.java)
                if (RoomManager.roomsJoined.containsKey(room.id)) {
                    RoomManager.roomsJoined.put(room.id, room.messages)
                    RoomManager.roomAvatars.put(room.id, room.avatars)
                }
            }))
            .on(SocketEvent.USER_SENT_INVITE, ({ data ->
                val feedback = Gson().fromJson(data.first().toString(), Feedback::class.java)
                Handler(Looper.getMainLooper()).post(Runnable {
                    if (activity !== null) {
                        Toast.makeText(
                            context,
                            feedback.log_message,
                            Toast.LENGTH_SHORT
                        ).show()
                    }
                })
            }))
    }

    private fun removeRoom(feedback: Feedback) {
        Handler(Looper.getMainLooper()).post(Runnable {
            if (feedback.status) {
                SocketHandler.searchRooms()
                RoomManager.leaveRoom()
                refreshRoomAdapter()
            } else {
                if (activity !== null) {
                    Toast.makeText(
                        context,
                        feedback.log_message,
                        Toast.LENGTH_SHORT
                    ).show()
                }
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

    private fun showInboxDialog() {
        val alertBuilder = AlertDialog.Builder(context)
        alertBuilder.setTitle(R.string.invites)
        val dialogView = layoutInflater.inflate(R.layout.dialog_invite_inbox, null)
        alertBuilder.setView(dialogView)
        val inviteRecyclerView = dialogView.findViewById<RecyclerView>(R.id.invite_list)
        val inviteListAdapter = GroupAdapter<GroupieViewHolder>()
        for (invitation in RoomManager.invites) {
            inviteListAdapter.add(InviteInboxRow(invitation))
        }
        inviteRecyclerView.adapter = inviteListAdapter
        alertBuilder
            .setPositiveButton(R.string.done) { _, _ ->
                refreshRoomAdapter()
                if (RoomManager.invites.isEmpty()) {
                    invites.setImageResource(R.drawable.ic_inbox_24px)
                }
            }
        val dialog = alertBuilder.create()
        dialog.window!!.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE)
        dialog.show()
    }

    private fun showCreateNewRoomDialog() {
        val alertBuilder = AlertDialog.Builder(context)
        alertBuilder.setTitle(R.string.create_room)
        val dialogView = layoutInflater.inflate(R.layout.dialog_create_room, null)
        alertBuilder.setView(dialogView)
        val inviteFrame = dialogView.findViewById<RelativeLayout>(R.id.invite_users_frame)
        val inviteRecyclerView = dialogView.findViewById<RecyclerView>(R.id.invite_list)
        val addButton = dialogView.findViewById<ImageButton>(R.id.add_user_button)
        val addUsername = dialogView.findViewById<EditText>(R.id.add_user_username)
        val inviteListAdapter = GroupAdapter<GroupieViewHolder>()
        inviteList = arrayListOf()
        inviteListAdapter.setOnItemClickListener(({ item, view ->
            inviteListAdapter.remove(item)
            inviteListAdapter.notifyDataSetChanged()
            inviteList.remove((item as InviteUserRow).user)
        }))
        addButton.setOnClickListener(({
            if (addUsername.text.isNotBlank()) {
                inviteListAdapter.add(InviteUserRow(addUsername.text.toString()))
                inviteListAdapter.notifyDataSetChanged()
                inviteRecyclerView.scrollToPosition(inviteListAdapter.itemCount - 1)
                if (!inviteList.contains(addUsername.text.toString())) {
                    inviteList.add(addUsername.text.toString())
                }
                addUsername.text.clear()
            }
        }))
        inviteRecyclerView.adapter = inviteListAdapter
        inviteFrame.visibility = View.GONE
        val radioGroup = dialogView.findViewById<RadioGroup>(R.id.room_visibility)
        var isPrivate = false
        radioGroup.check(R.id.is_public_room)
        radioGroup.setOnCheckedChangeListener(({ _, checkedId ->
            isPrivate = checkedId == R.id.is_private_room
            val visibility = if (isPrivate) View.VISIBLE else View.GONE
            inviteFrame.visibility = visibility
        }))

        alertBuilder
            .setPositiveButton(R.string.ok) { _, _ ->
                newRoomName = dialogView.findViewById<EditText>(R.id.room_name).text.toString()
                if (newRoomName.isNotBlank()) {
                    SocketHandler.createChatRoom(newRoomName, isPrivate)
                } else {
                    Toast.makeText(context, R.string.error_roomname, Toast.LENGTH_SHORT)
                        .show()                    }
            }
            .setNegativeButton(R.string.cancel) { _, _ -> }
            .setCancelable(false)
        val dialog = alertBuilder.create()
        dialog.window!!.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE)
        dialog.show()
    }
}
