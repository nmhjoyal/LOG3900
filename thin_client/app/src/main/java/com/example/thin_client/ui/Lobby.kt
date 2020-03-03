package com.example.thin_client.ui

import android.content.Context
import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.Menu
import android.view.MenuItem
import android.view.View
import android.widget.Toast
import androidx.core.view.isVisible
import com.example.thin_client.R
import com.example.thin_client.data.Feedback
import com.example.thin_client.data.Preferences
import com.example.thin_client.data.SignInFeedback
import com.example.thin_client.data.model.User
import com.example.thin_client.data.rooms.RoomManager
import com.example.thin_client.data.server.HTTPRequest
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.chatrooms.ChatRoomsFragment
import com.example.thin_client.ui.game_mode.free_draw.FreeDrawActivity
import com.example.thin_client.ui.login.LoginActivity
import com.example.thin_client.ui.profile.ProfileActivity
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
                SocketHandler.socket!!.on(SocketEvent.USER_SIGNED_IN, ({ data ->
                    val gson = Gson()
                    val signInFeedback =
                        gson.fromJson(data.first().toString(), SignInFeedback::class.java)
                    if (signInFeedback.feedback.status) {
                        RoomManager.createRoomList(signInFeedback.rooms_joined)
                        showChatRoomsFragment()
                    } else {
                        runOnUiThread(({
                            SocketHandler.socket!!.off(SocketEvent.USER_SIGNED_IN)
                            Toast.makeText(applicationContext, R.string.error_logging_in, Toast.LENGTH_LONG).show()
                            val intent = Intent(applicationContext, LoginActivity::class.java)
                            startActivity(intent)
                            SocketHandler.socket!!.disconnect()
                        }))
                    }
                }))
                SocketHandler.login(User(username, password))
            }
        }

        SocketHandler.socket?.on(SocketEvent.USER_SIGNED_OUT, ({ data ->
            val gson = Gson()
            val feedback = gson.fromJson(data.first().toString(),Feedback::class.java)
            if (feedback.status) {
                prefs.edit().putBoolean(Preferences.LOGGED_IN_KEY, false).apply()
                val intent = Intent(applicationContext, LoginActivity::class.java)
                startActivity(intent)
                SocketHandler.disconnect()
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


        free_draw.setOnClickListener(({
            val intent = Intent(applicationContext, FreeDrawActivity::class.java)
            startActivity(intent)
        }))

        show_rooms_button.setOnClickListener(({
            if (chatrooms_container.isVisible) {
                chatrooms_container.visibility = View.GONE
                show_rooms_button.setImageResource(R.drawable.ic_open)
            } else {
                chatrooms_container.visibility = View.VISIBLE
                show_rooms_button.setImageResource(R.drawable.ic_close)
            }
        }))
        showChatRoomsFragment()
    }


    private fun showChatRoomsFragment() {
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
            }
            R.id.menu_profile -> {
                val intent = Intent(applicationContext, ProfileActivity::class.java)
                startActivity(intent)
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
