package com.example.thin_client.ui.chatrooms

import android.os.Bundle
import com.google.android.material.snackbar.Snackbar
import androidx.appcompat.app.AppCompatActivity
import com.example.thin_client.R

import kotlinx.android.synthetic.main.activity_chat_rooms.*

class ChatRoomsActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_chat_rooms)
        setSupportActionBar(toolbar)


    }

}
