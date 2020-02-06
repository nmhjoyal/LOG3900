package com.example.thin_client.ui.chat
import android.content.Intent
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.KeyEvent

import android.view.Menu
import android.view.MenuItem
import android.view.View
import android.widget.EditText
import android.widget.ImageView


import androidx.appcompat.app.AppCompatActivity
import com.example.thin_client.R
import com.example.thin_client.data.Message
import com.example.thin_client.data.model.User
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.login.LoginActivity
import com.google.gson.Gson
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.activity_chat.*
import kotlinx.android.synthetic.main.activity_login.*
import kotlinx.android.synthetic.main.chat_from_row.*
import kotlinx.android.synthetic.main.chat_to_row.*


class ChatActivity : AppCompatActivity() {


    val adapter = GroupAdapter<GroupieViewHolder>()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_chat)

        SocketHandler.joinRoom()

        SocketHandler.socket?.on("new_message", ({ data ->
            val jsonData = Gson().fromJson(data.first().toString(), Message::class.java)
            val username = jsonData.author.username
            Handler(Looper.getMainLooper()).post(Runnable {
                if (username == SocketHandler.user!!.username) {
                    showToMessage()
                } else {
                    showFromMessage(jsonData.content,username)
                }
            })
        }))

        recyclerview_chat.adapter = adapter
        val text = editText_chat.text

       send_button_chat.setOnClickListener {
           if(text.isNotEmpty()) {
               send_button_chat.isEnabled = true
               SocketHandler.sendMessage(text.toString())
           }
       }

      editText_chat.setOnKeyListener(View.OnKeyListener { v, keyCode, event ->
          if(text.isNotEmpty()) {
              send_button_chat.isEnabled = true
            if (keyCode == KeyEvent.KEYCODE_ENTER) {
                SocketHandler.sendMessage(text.toString())
                return@OnKeyListener true
            }
          }
            false
        })

    }

    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        when(item.itemId) {
            R.id.menu_sign_out -> {
                SocketHandler.logout()
                val intent = Intent(applicationContext, LoginActivity::class.java)
                startActivity(intent)
                finish()
            }
        }
        return super.onOptionsItemSelected(item)
    }

    override fun onCreateOptionsMenu(menu:Menu): Boolean{
        menuInflater.inflate(R.menu.nav_menu, menu)
        return super.onCreateOptionsMenu(menu)
    }


    private fun showToMessage(){
        val text = editText_chat.text
        adapter.add(ChatToItem(text.toString()))
        text.clear()
    }

    private fun showFromMessage(text: String, author:String) {
        adapter.add(ChatFromItem(text, author))
    }


}



