package com.example.thin_client.ui.chatrooms

import android.content.Intent
import android.os.Bundle
import android.widget.Button
import androidx.appcompat.app.AppCompatActivity
import com.example.thin_client.R
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.chat.ChatActivity
import com.xwray.groupie.GroupAdapter

import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.activity_chatrooms.*
import kotlinx.android.synthetic.main.chatrooms_row.*


class ChatRoomsActivity : AppCompatActivity() {
    val adapter = GroupAdapter<GroupieViewHolder>()
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_chatrooms)

        fetchRooms()

    }

    private fun fetchRooms(){
        adapter.add(ChatRoomItem("Room # 1"))
        adapter.add(ChatRoomItem("Room # 2"))
        adapter.add(ChatRoomItem("Room # 3"))

        adapter.setOnItemClickListener{ item,view ->
            SocketHandler.joinRoom()
            val intent= Intent(view.context, ChatActivity::class.java)
            startActivity(intent)
            finish()
        }
        recyclerview_chatrooms.adapter = adapter
    }

}
