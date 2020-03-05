package com.example.thin_client.ui

import android.content.Context
import android.content.Intent
import android.content.SharedPreferences
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.Menu
import android.view.MenuItem
import android.view.View
import android.view.WindowManager
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.core.view.isVisible
import com.example.thin_client.R
import com.example.thin_client.data.Feedback
import com.example.thin_client.data.app_preferences.Preferences
import com.example.thin_client.data.SignInFeedback
import com.example.thin_client.data.app_preferences.PreferenceHandler
import com.example.thin_client.data.model.User
import com.example.thin_client.data.rooms.RoomManager
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.chatrooms.ChatRoomsFragment
import com.example.thin_client.ui.game_mode.free_draw.FreeDrawActivity
import com.example.thin_client.ui.login.LoginActivity
import com.example.thin_client.ui.profile.ProfileActivity
import com.github.nkzawa.socketio.client.Socket
import com.google.gson.Gson
import kotlinx.android.synthetic.main.activity_lobby.*

class Lobby : AppCompatActivity() {
    private val manager = supportFragmentManager
    private lateinit var prefs: SharedPreferences

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_lobby)

        prefs = this.getSharedPreferences(Preferences.USER_PREFS, Context.MODE_PRIVATE)

        if (SocketHandler.socket == null) {
            SocketHandler.connect()
        }

        setupSocketEvents()

        if (!prefs.getBoolean(Preferences.LOGGED_IN_KEY, false)) {
            val intent = Intent(applicationContext, LoginActivity::class.java)
            startActivity(intent)
            SocketHandler.disconnect()
        } else if (!SocketHandler.isLoggedIn){
            val user = PreferenceHandler(applicationContext).getUser()
            SocketHandler.login(User(user.username, user.password))
            SocketHandler.isLoggedIn = true
        } else {
            showChatRoomsFragment()
        }

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

    private fun setupSocketEvents() {
        SocketHandler.socket!!
            .on(SocketEvent.USER_SIGNED_IN, ({ data ->
                val gson = Gson()
                val signInFeedback =
                    gson.fromJson(data.first().toString(), SignInFeedback::class.java)
                if (signInFeedback.feedback.status) {
                    RoomManager.createRoomList(signInFeedback.rooms_joined)
                    showChatRoomsFragment()
                } else {
                    runOnUiThread(({
                        Toast.makeText(applicationContext, R.string.error_logging_in, Toast.LENGTH_LONG).show()
                        val intent = Intent(applicationContext, LoginActivity::class.java)
                        startActivity(intent)
                    }))
                    SocketHandler.disconnect()
                }
            }))
            .on(Socket.EVENT_CONNECT_ERROR, ({
                runOnUiThread(({
                    val alertDialog = AlertDialog.Builder(this)
                    alertDialog.setTitle(R.string.error_connect_title)
                        .setCancelable(false)
                        .setMessage(R.string.error_connect)
                        .setPositiveButton(R.string.ok) { _, _ -> finishAffinity() }

                    val dialog = alertDialog.create()
                    dialog.window!!.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE)
                    dialog.show()
                }))
                SocketHandler.disconnect()
            }))
            .on(SocketEvent.USER_SIGNED_OUT, ({ data ->
                val gson = Gson()
                val feedback = gson.fromJson(data.first().toString(),Feedback::class.java)
                if (feedback.status) {
                    PreferenceHandler(this).resetUserPrefs()
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
    }
}
