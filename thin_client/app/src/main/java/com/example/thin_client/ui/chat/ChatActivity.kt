package com.example.thin_client.ui.chat

import android.os.Bundle
import android.view.KeyEvent
import android.view.View


import androidx.appcompat.app.AppCompatActivity
import androidx.recyclerview.widget.LinearLayoutManager
import androidx.recyclerview.widget.RecyclerView
import com.example.thin_client.R
import com.example.thin_client.data.Message
import com.example.thin_client.data.model.LoggedInUser
import kotlinx.android.synthetic.main.activity_chat.*
import kotlinx.android.synthetic.main.activity_login.*
import java.lang.ClassCastException

class ChatActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_chat)

        val mRecyclerView = findViewById(R.id.recyclerview) as RecyclerView

        mRecyclerView.layoutManager = LinearLayoutManager(this, RecyclerView.HORIZONTAL, false)

        val messages = ArrayList<Message>()
        messages.add(Message("1", editTextMessage.text.toString(), "Amar", "", "2"))

        val adapter = ChatAdapter(messages)
        mRecyclerView.adapter=adapter

        sendButton.setOnClickListener {
            messages.add(Message("1","Allo", "Amar", "", ""))
        }

        editTextMessage.setOnKeyListener(View.OnKeyListener {v, keyCode, event ->
            if (keyCode == KeyEvent.KEYCODE_ENTER) {
                messages.add(Message("1",editTextMessage.text.toString(), "Amaaaaaaaaa", "", ""))
                return@OnKeyListener true
            }
            false
        })
    }

    private fun sendMessage() {
       // var message = Message("1",editTextMessage.text.toString(), username.text.toString(), "", "")

    }



}


