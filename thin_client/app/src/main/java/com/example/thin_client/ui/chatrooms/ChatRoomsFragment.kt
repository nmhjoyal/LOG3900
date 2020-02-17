package com.example.thin_client.ui.chatrooms

import android.content.Intent
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import com.example.thin_client.R
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.chat.ChatActivity
import com.xwray.groupie.GroupAdapter

import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.chatrooms_fragment.*


class ChatRoomsFragment : Fragment() {
    val adapter = GroupAdapter<GroupieViewHolder>()

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        fetchRooms()
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {

        val v = inflater.inflate(R.layout.chatrooms_fragment, container, false)
        return v
    }

    companion object{
        val ROOM_KEY = "ROOM_KEY"
    }


    private fun fetchRooms(){
        adapter.add(ChatRoomItem("Room # 1"))
        adapter.add(ChatRoomItem("Room # 2"))
        adapter.add(ChatRoomItem("Room # 3"))

        adapter.setOnItemClickListener{ item,view ->
            SocketHandler.joinRoom()
            val room = item as ChatRoomItem
            val intent= Intent(view.context, ChatActivity::class.java)
            intent.putExtra(ROOM_KEY,room.roomname)
            startActivity(intent)
        }
        recyclerview_chatrooms.adapter = adapter
    }

}
