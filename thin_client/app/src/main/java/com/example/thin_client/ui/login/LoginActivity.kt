package com.example.thin_client.ui.login

import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.text.Editable
import android.text.TextWatcher
import android.view.View
import android.widget.Button
import android.widget.EditText
import android.widget.ProgressBar
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import com.example.thin_client.R
import com.example.thin_client.data.Preferences
import com.example.thin_client.data.SignInFeedback
import com.example.thin_client.data.rooms.RoomManager

import com.example.thin_client.data.model.User
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.Lobby

import com.example.thin_client.ui.createUser.CreateUserActivity
import com.github.nkzawa.socketio.client.Socket
import com.google.gson.Gson
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder


class LoginActivity : AppCompatActivity() {


    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_login)

        val username = findViewById<EditText>(R.id.username)
        val password = findViewById<EditText>(R.id.password)
        val login = findViewById<Button>(R.id.login)
        val signup = findViewById<Button>(R.id.signup)
        val loading = findViewById<ProgressBar>(R.id.loading)

        loading.visibility = View.INVISIBLE

        login.setOnClickListener {
            loading.visibility = ProgressBar.VISIBLE
            login.isEnabled = false
            val socket = SocketHandler.connect()
            val user = User(username.text.toString(), password.text.toString())
            socket
                .on(Socket.EVENT_CONNECT, ({
                    SocketHandler.login(user)
                }))
                .on(Socket.EVENT_CONNECT_ERROR, ({
                    Handler(Looper.getMainLooper()).post(Runnable {
                        showLoginFailed()
                        loading.visibility = ProgressBar.GONE
                        login.isEnabled = true
                    })
                    SocketHandler.disconnect()
                }))
                .on(SocketEvent.USER_SIGNED_IN, ({ data ->
                    val gson = Gson()
                    val signInFeedback = gson.fromJson(data.first().toString(), SignInFeedback::class.java)
                    if (signInFeedback.feedback.status) {
                        RoomManager.createRoomList(signInFeedback.rooms_joined)
                        val prefs = this.getSharedPreferences(Preferences.USER_PREFS, Context.MODE_PRIVATE)
                        prefs.edit().putBoolean(Preferences.LOGGED_IN_KEY, true).apply()
                        prefs.edit().putString(Preferences.USERNAME, user.username).apply()
                        prefs.edit().putString(Preferences.PASSWORD, user.password).apply()
                        val intent = Intent(applicationContext, Lobby::class.java)
                        startActivity(intent)
                        finish()
                    } else {
                        Handler(Looper.getMainLooper()).post(Runnable {
                            Toast.makeText(
                                applicationContext,
                                signInFeedback.feedback.log_message,
                                Toast.LENGTH_SHORT
                            ).show()
                            loading.visibility = ProgressBar.GONE
                            login.isEnabled = true
                            SocketHandler.socket!!.disconnect()
                        })
                    }
                }))
        }

        signup.setOnClickListener {
            val intent = Intent(this, CreateUserActivity::class.java)
            startActivity(intent)
        }

    }

    private fun showLoginFailed() {
        Toast.makeText(applicationContext, R.string.login_failed, Toast.LENGTH_SHORT).show()
    }

    override fun onBackPressed() {
        // Disable native back
    }
}

/**
 * Extension function to simplify setting an afterTextChanged action to EditText components.
 */
fun EditText.afterTextChanged(afterTextChanged: (String) -> Unit) {
    this.addTextChangedListener(object : TextWatcher {
        override fun afterTextChanged(editable: Editable?) {
            afterTextChanged.invoke(editable.toString())
        }

        override fun beforeTextChanged(s: CharSequence, start: Int, count: Int, after: Int) {}

        override fun onTextChanged(s: CharSequence, start: Int, before: Int, count: Int) {}
    })
}
