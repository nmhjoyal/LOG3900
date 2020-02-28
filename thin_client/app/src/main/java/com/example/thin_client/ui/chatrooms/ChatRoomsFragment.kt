package com.example.thin_client.ui.chatrooms

import com.example.thin_client.ui.chat.ChatFragment

import android.os.Bundle
import android.view.LayoutInflater
import android.view.MenuItem
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import com.example.thin_client.R
import com.example.thin_client.data.Feedback
import com.example.thin_client.data.model.RoomManager
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.google.gson.Gson
import com.xwray.groupie.GroupAdapter

import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.chatrooms_fragment.*


class ChatRoomsFragment : Fragment() {
    val adapter = GroupAdapter<GroupieViewHolder>()

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        fetchRooms()

        adapter.setOnItemClickListener{ item, v ->
            SocketHandler.socket?.on(SocketEvent.USER_JOINED_ROOM, ({ data ->
                val gson = Gson()
                val feedback = gson.fromJson(data.first().toString(), Feedback::class.java)
                if (feedback.status) {
                    val transaction = fragmentManager!!.beginTransaction()
                    val chatFragment = ChatFragment()
                    transaction.replace(R.id.chatrooms_container, chatFragment)
                    transaction.addToBackStack(null)
                    transaction.commit()
                }

            }))
        }
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {

        return inflater.inflate(R.layout.chatrooms_fragment, container, false)
    }

    companion object{
        val ROOM_KEY = "ROOM_KEY"
    }


    private fun fetchRooms() {
        for (room in RoomManager.roomsJoined) {
            adapter.add(ChatRoomItem(room))
        }
        recyclerview_chatrooms.adapter = adapter
    }
    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        when(item.itemId) {
            R.id.new_room -> {
                createNewRoom("Room #4")
            }
        }
        return super.onOptionsItemSelected(item)
    }

    fun createNewRoom(roomname:String) {
        adapter.add(ChatRoomItem(roomname))
    }


}
