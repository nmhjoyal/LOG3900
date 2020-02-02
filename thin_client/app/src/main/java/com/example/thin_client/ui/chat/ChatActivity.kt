package com.example.thin_client.ui.chat

import android.content.Intent
import android.os.Bundle

import android.view.Menu
import android.view.MenuItem


import androidx.appcompat.app.AppCompatActivity
import com.example.thin_client.R
import com.example.thin_client.data.Message
import com.example.thin_client.data.model.User
import com.example.thin_client.ui.login.LoginActivity
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.ViewHolder
import kotlinx.android.synthetic.main.activity_chat.*
import kotlinx.android.synthetic.main.chat_from_row.*


class ChatActivity : AppCompatActivity() {


    val adapter = GroupAdapter<ViewHolder>()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_chat)


       recyclerview_chat.adapter = adapter

        send_button_chat.setOnClickListener {
            sendMessage()
        }
    }


    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        when(item.itemId) {
            R.id.menu_sign_out -> {
                val intent = Intent(this, LoginActivity::class.java)
                intent.flags = Intent.FLAG_ACTIVITY_CLEAR_TASK.or(Intent.FLAG_ACTIVITY_NEW_TASK)
                startActivity(intent)
            }
        }
        return super.onOptionsItemSelected(item)
    }

    override fun onCreateOptionsMenu(menu:Menu): Boolean{
        menuInflater.inflate(R.menu.nav_menu, menu)
        return super.onCreateOptionsMenu(menu)
    }


    private fun sendMessage(){
        val text = editText_chat.text
        val username = User.username
        val message = Message(text.toString(),username,System.currentTimeMillis()/100)

    }


}



