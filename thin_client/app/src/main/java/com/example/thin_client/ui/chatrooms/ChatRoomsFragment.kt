package com.example.thin_client.ui.chatrooms

import android.app.AlertDialog
import android.content.res.Resources
import com.example.thin_client.ui.chat.ChatFragment

import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.view.WindowManager
import android.widget.EditText
import android.widget.Toast
import androidx.core.view.isNotEmpty
import androidx.fragment.app.Fragment
import androidx.recyclerview.widget.ItemTouchHelper
import androidx.recyclerview.widget.RecyclerView
import com.example.thin_client.R
import com.example.thin_client.data.Feedback
import com.example.thin_client.data.rooms.RoomArgs
import com.example.thin_client.data.rooms.RoomManager
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.google.gson.Gson
import com.xwray.groupie.GroupAdapter

import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.chatrooms_fragment.*
import kotlinx.android.synthetic.main.room_row.*
import okhttp3.internal.notify
import java.net.Socket


class ChatRoomsFragment : Fragment() {
    val adapter = GroupAdapter<GroupieViewHolder>()
    private var selectedRoom : String = ""
    private var newRoomName : String = ""

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        adapter.setOnItemClickListener{ item, v ->
            selectedRoom = (item as ChatRoomItem).roomname
            SocketHandler.joinChatRoom(selectedRoom)
        }


        SocketHandler.socket!!.on(SocketEvent.USER_JOINED_ROOM, ({
            val bundle = Bundle()
            bundle.putString(RoomArgs.ROOM_ID, selectedRoom)
            val transaction = fragmentManager!!.beginTransaction()
            val chatFragment = ChatFragment()
            chatFragment.arguments = bundle
            transaction.replace(R.id.chatrooms_container, chatFragment)
            transaction.addToBackStack(null)
            transaction.commit()
        }))
            .on(SocketEvent.ROOM_CREATED, ({ data ->
                 val roomCreateFeedback = Gson().fromJson(data.first().toString(), Feedback::class.java)
                if (roomCreateFeedback.status) {
                    adapter.add(ChatRoomItem(newRoomName))
                } else {
                    Handler(Looper.getMainLooper()).post(Runnable {
                        Toast.makeText(context, roomCreateFeedback.log_message, Toast.LENGTH_SHORT).show()
                    })
                }
            }))

        add_room.setOnClickListener(({
            val alertBuilder = AlertDialog.Builder(context)
            alertBuilder.setTitle(R.string.create_room)
            val roomname = EditText(context)
            roomname.setHint(R.string.room_name)
            alertBuilder.setView(roomname)
            alertBuilder
                .setPositiveButton(R.string.ok) { _, _ ->
                    newRoomName = roomname.text.toString()
                    if (newRoomName.isNotBlank()) {
                        SocketHandler.createChatRoom(newRoomName)
                        adapter.add(ChatRoomItem(newRoomName))
                        RoomManager.addRoom((newRoomName))
                    } else {
                        roomname.error = Resources.getSystem().getString(R.string.error_roomname)
                    }
                }
                .setNegativeButton(R.string.cancel) { _, _ -> }
            val dialog = alertBuilder.create()
            dialog.window!!.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE)
            dialog.show()
        }))

        val itemTouchHelperCallBack = object : ItemTouchHelper.SimpleCallback(0, ItemTouchHelper.LEFT){
            override fun onSwiped(viewHolder: RecyclerView.ViewHolder, position: Int) {
                adapter.remove(ChatRoomItem(selectedRoom))
            }

            override fun onMove(
                recyclerView: RecyclerView,
                viewHolder: RecyclerView.ViewHolder,
                target: RecyclerView.ViewHolder
            ): Boolean {
                return false
            }
        }

        val itemTouchHelper = ItemTouchHelper(itemTouchHelperCallBack)
        itemTouchHelper.attachToRecyclerView(recyclerview_chatrooms)

        fetchRooms()
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {

        return inflater.inflate(R.layout.chatrooms_fragment, container, false)
    }

    private fun fetchRooms() {
        for (room in RoomManager.roomsJoined) {
            adapter.add(ChatRoomItem(room))
//            SocketHandler.joinChatRoom(room)
        }
        recyclerview_chatrooms.adapter = adapter
    }

}
