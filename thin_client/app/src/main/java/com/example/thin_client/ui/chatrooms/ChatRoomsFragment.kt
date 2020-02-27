package com.example.thin_client.ui.chatrooms

import android.content.Intent
import android.util.Log
import android.os.Bundle
import android.view.LayoutInflater
import android.view.MenuItem
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import com.example.thin_client.R
import com.example.thin_client.data.Feedback
import com.example.thin_client.data.Message
import com.example.thin_client.data.SignInFeedback
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.chat.ChatActivity
import com.google.gson.Gson
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
        adapter.setOnItemClickListener{ item,view ->
            adapter.add(ChatRoomItem("roomA"))
            SocketHandler.joinChatRoom("roomA")
            SocketHandler.socket?.on(SocketEvent.USER_JOINED_ROOM, ({ data ->
                val gson = Gson()
                val feedback = gson.fromJson(data.first().toString(), Feedback::class.java)
                if (feedback.status) {
                   // val room = item as ChatRoomItem
                    val intent = Intent(view.context, ChatActivity::class.java)
                    intent.putExtra(ROOM_KEY, "roomA")
                    startActivity(intent)
                }

            }))
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
