package com.example.thin_client.ui.chat

import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.KeyEvent
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import com.example.thin_client.R
import com.example.thin_client.data.Message
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.login.afterTextChanged
import com.google.gson.Gson
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.activity_chat.*

class ChatFragment : Fragment() {

    private val adapter = GroupAdapter<GroupieViewHolder>()

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        recyclerview_chat.adapter = adapter

        SocketHandler.socket?.on(SocketEvent.NEW_MESSAGE, ({ data ->
            val jsonData = Gson().fromJson(data.first().toString(), Message::class.java)
            val username = jsonData.author.username
            val timestamp = jsonData.date
            Handler(Looper.getMainLooper()).post(Runnable {
                if (username == SocketHandler.user!!.username) {
                    showToMessage(jsonData.content, timestamp)
                } else {
                    showFromMessage(jsonData.content, username, timestamp)
                }
            })
        }))

        send_button_chat.setOnClickListener {
            if (editText_chat.text.isNotBlank()) {
                send_button_chat.isEnabled = true
                SocketHandler.sendMessage(editText_chat.text.toString())
                editText_chat.setText("")
            }
        }

        editText_chat.setOnKeyListener(View.OnKeyListener { v, keyCode, event ->
            if(editText_chat.text.isNotBlank()) {
                send_button_chat.isEnabled = true
                if (keyCode == KeyEvent.KEYCODE_ENTER && event.action == KeyEvent.ACTION_UP) {
                    SocketHandler.sendMessage(editText_chat.text.replace("\\n".toRegex(), ""))
                    editText_chat.setText("")
                    return@OnKeyListener true
                }
            }
            false
        })

        editText_chat.afterTextChanged {
            send_button_chat.isEnabled = editText_chat.text.isNotBlank()
        }
    }

    private fun showToMessage(text: String, date: Long){
        adapter.add(ChatToItem(text.replace("\\n".toRegex(), ""), date))
        if (recyclerview_chat != null){
            recyclerview_chat.scrollToPosition(adapter.itemCount - 1)
        }
    }

    private fun showFromMessage(text: String, author:String, date: Long) {
        adapter.add(ChatFromItem(text, author, date))
        if (recyclerview_chat != null){
            recyclerview_chat.scrollToPosition(adapter.itemCount - 1)
        }
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {

        return inflater.inflate(R.layout.activity_chat, container, false)
    }
}