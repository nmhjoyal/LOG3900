package com.example.thin_client.ui

import android.content.Context
import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.Menu
import android.view.MenuItem
import android.widget.Toast
import com.example.thin_client.R
import com.example.thin_client.data.Feedback
import com.example.thin_client.data.Preferences
import com.example.thin_client.data.model.User
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.chatrooms.ChatRoomsFragment
import com.example.thin_client.ui.game_mode.free_draw.FreeDrawActivity
import com.example.thin_client.ui.login.LoginActivity
import com.google.gson.Gson
import kotlinx.android.synthetic.main.activity_lobby.*

class Lobby : AppCompatActivity() {
    private val manager = supportFragmentManager

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_lobby)

        val prefs = this.getSharedPreferences(Preferences.USER_PREFS, Context.MODE_PRIVATE)
        if (!prefs.getBoolean(Preferences.LOGGED_IN_KEY, false)) {
            val intent = Intent(applicationContext, LoginActivity::class.java)
            startActivity(intent)
        } else if (SocketHandler.socket == null) {
            SocketHandler.connect()
            val username = prefs.getString(Preferences.USERNAME, "")
            val password = prefs.getString(Preferences.PASSWORD, "")
            if (username!!.isNotBlank() && password!!.isNotBlank()) {
                SocketHandler.login(User(username, password))
            } else {
                Toast.makeText(applicationContext, R.string.error_logging_in, Toast.LENGTH_LONG).show()
                val intent = Intent(applicationContext, LoginActivity::class.java)
                startActivity(intent)
                finish()
            }
        }


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
                SocketHandler.socket?.on(SocketEvent.USER_SIGNED_OUT, ({ data ->
                    val gson = Gson()
                    val feedback = gson.fromJson(data.first().toString(),Feedback::class.java)
                    if (feedback.status) {
                        val prefs = this.getSharedPreferences(Preferences.USER_PREFS, Context.MODE_PRIVATE)
                        prefs.edit().putBoolean(Preferences.LOGGED_IN_KEY, false).apply()
                        val intent = Intent(applicationContext, LoginActivity::class.java)
                        startActivity(intent)
                        finish()
                    } else {
                        Handler(Looper.getMainLooper()).post(Runnable {
                            Toast.makeText(
                                applicationContext,
                                feedback.log_message,
                                Toast.LENGTH_SHORT
                            ).show()
                        })
                    }
                }))
            }
        }
        return super.onOptionsItemSelected(item)
    }

    override fun onCreateOptionsMenu(menu:Menu): Boolean{
        menuInflater.inflate(R.menu.nav_menu, menu)
        return super.onCreateOptionsMenu(menu)
    }

    override fun onBackPressed() {
    }

}
