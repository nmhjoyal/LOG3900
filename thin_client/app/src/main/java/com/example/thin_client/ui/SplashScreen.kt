package com.example.thin_client.ui

import android.content.Context
import android.content.Intent
import android.content.SharedPreferences
import android.os.Bundle
import android.os.Handler
import android.view.View
import android.view.WindowManager
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import com.example.thin_client.R
import com.example.thin_client.data.SignInFeedback
import com.example.thin_client.data.app_preferences.PreferenceHandler
import com.example.thin_client.data.app_preferences.Preferences
import com.example.thin_client.data.lifecycle.LoginState
import com.example.thin_client.data.model.User
import com.example.thin_client.data.rooms.RoomManager
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.login.LoginActivity
import com.github.nkzawa.socketio.client.Socket
import com.google.gson.Gson
import kotlinx.android.synthetic.main.activity_splash_screen.*

class SplashScreen: AppCompatActivity() {

    private lateinit var prefs: SharedPreferences
    private val SPLASH_TIME_OUT: Long = 3000 // 3 sec

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_splash_screen)
        prefs = this.getSharedPreferences(Preferences.USER_PREFS, Context.MODE_PRIVATE)

        Handler().postDelayed({
            loading.visibility = View.VISIBLE
            setupSocket()
        }, SPLASH_TIME_OUT)
    }

    private fun setupSocket() {
        if (!SocketHandler.isConnected()) {
            SocketHandler.connect()
        }

        setupSocketEvents()
    }

    override fun onDestroy() {
        super.onDestroy()
        turnOffSocketEvents()
    }

    private fun startLobby() {
        val intent = Intent(applicationContext, Lobby::class.java)
        startActivity(intent)
        finish()
    }

    private fun startLogin() {
        val intent = Intent(applicationContext, LoginActivity::class.java)
        startActivity(intent)
        finish()
        SocketHandler.disconnect()
    }

    private fun turnOffSocketEvents() {
        if (SocketHandler.socket != null) {
            SocketHandler.socket!!
                .off(SocketEvent.USER_SIGNED_IN)
                .off(Socket.EVENT_CONNECT_ERROR)
        }
    }

    override fun onBackPressed() {
    }

    private fun setupSocketEvents() {
        SocketHandler.socket!!
            .on(SocketEvent.USER_SIGNED_IN, ({ data ->
                val signInFeedback = Gson().fromJson(data.first().toString(), SignInFeedback::class.java)
                if (signInFeedback.feedback.status) {
                    RoomManager.createRoomList(signInFeedback.rooms_joined)
                    startLobby()
                } else {
                    runOnUiThread(({
                        Toast.makeText(applicationContext, signInFeedback.feedback.log_message, Toast.LENGTH_LONG).show()
                    }))
                    startLogin()
                }
            }))
            .on(Socket.EVENT_CONNECT, ({
                runOnUiThread(({
                    when (SocketHandler.getLoginState(prefs)) {
                        LoginState.FIRST_LOGIN -> {
                            startLogin()
                        }
                        LoginState.LOGIN_WITH_EXISTING -> {
                            val user = PreferenceHandler(applicationContext).getUser()
                            SocketHandler.login(User(user.username, user.password))
                            SocketHandler.isLoggedIn = true
                        }
                        LoginState.LOGGED_IN -> {
                            startLobby()
                        }
                    }
                }))
            }))
            .on(Socket.EVENT_CONNECT_ERROR, ({
                runOnUiThread(({
                    loading.visibility = View.GONE
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
    }
}