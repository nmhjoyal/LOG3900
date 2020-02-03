package com.example.thin_client.ui.chat
import android.content.Intent
import android.os.Bundle

import android.view.Menu
import android.view.MenuItem


import androidx.appcompat.app.AppCompatActivity
import com.example.thin_client.R
import com.example.thin_client.data.Message
import com.example.thin_client.data.model.User
import com.example.thin_client.server.GlobalBus
import com.example.thin_client.server.LogoutEvent
import com.example.thin_client.ui.login.LoginActivity
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.activity_chat.*


class ChatActivity : AppCompatActivity() {


    val adapter = GroupAdapter<GroupieViewHolder>()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_chat)


       recyclerview_chat.adapter = adapter

        send_button_chat.setOnClickListener {
            sendMessage()
        }
    }

    override fun onStart() {
        super.onStart()
        GlobalBus.bus?.register(this)
    }

    override fun onStop() {
        super.onStop()
        GlobalBus.bus?.unregister(this)
    }

    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        when(item.itemId) {
            R.id.menu_sign_out -> {
                GlobalBus.bus?.post(LogoutEvent("logout"))
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


    private fun sendMessage(){
        val text = editText_chat.text
//        val username = User.username
//        val message = Message(text.toString(),username,System.currentTimeMillis()/100)
        text.clear()
    }


}



