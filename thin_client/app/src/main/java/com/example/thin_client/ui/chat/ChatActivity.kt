package com.example.thin_client.ui.chat
import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.KeyEvent

import android.view.Menu
import android.view.MenuItem
import android.view.View


import androidx.appcompat.app.AppCompatActivity
import com.example.thin_client.R
import com.example.thin_client.data.Message
import com.example.thin_client.data.Preferences
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.chatrooms.ChatRoomsActivity
import com.example.thin_client.ui.login.LoginActivity
import com.example.thin_client.ui.login.afterTextChanged
import com.google.gson.Gson
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.activity_chat.*


class ChatActivity : AppCompatActivity() {


    val adapter = GroupAdapter<GroupieViewHolder>()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_chat)
        val roomname = intent.getStringExtra(ChatRoomsActivity.ROOM_KEY)
        supportActionBar?.title = roomname
       // SocketHandler.joinRoom()
        recyclerview_chat.adapter = adapter

        SocketHandler.socket?.on("new_message", ({ data ->
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


    override fun onBackPressed() {
        // Disable native back
    }

    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        when(item.itemId) {
            R.id.menu_sign_out -> {
                SocketHandler.logout()
                val prefs = this.getSharedPreferences(Preferences.USER_PREFS, Context.MODE_PRIVATE)
                prefs.edit().putBoolean(Preferences.LOGGED_IN_KEY, false).apply()
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


    private fun showToMessage(text: String, date: Long){
        adapter.add(ChatToItem(text.replace("\\n".toRegex(), ""), date))
    }

    private fun showFromMessage(text: String, author:String, date: Long) {
        adapter.add(ChatFromItem(text, author, date))
    }


}



