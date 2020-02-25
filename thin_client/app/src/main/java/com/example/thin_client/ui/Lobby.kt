package com.example.thin_client.ui

import android.content.Context
import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.Menu
import android.view.MenuItem
import com.example.thin_client.R
import com.example.thin_client.data.Preferences
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.chat.ChatActivity
import com.example.thin_client.ui.chatrooms.ChatRoomsFragment
import com.example.thin_client.ui.game_mode.free_draw.FreeDrawActivity
import com.example.thin_client.ui.login.LoginActivity
import kotlinx.android.synthetic.main.activity_lobby.*

class Lobby : AppCompatActivity() {
    val manager = supportFragmentManager

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_lobby)

        free_draw.setOnClickListener(({
            val intent = Intent(applicationContext, FreeDrawActivity::class.java)
            startActivity(intent)
        }))

        showChatRoomsFragment()
    }


    fun showChatRoomsFragment(){
        val transaction = manager.beginTransaction()
        val chatroomsFragment = ChatRoomsFragment()
        transaction.replace(R.id.chatrooms_container, chatroomsFragment)
        transaction.addToBackStack(null)
        transaction.commit()
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

}
