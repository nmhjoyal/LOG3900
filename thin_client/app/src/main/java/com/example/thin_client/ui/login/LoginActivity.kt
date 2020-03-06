package com.example.thin_client.ui.login

import OkHttpRequest
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
import com.example.thin_client.data.Feedback
import com.example.thin_client.data.app_preferences.Preferences
import com.example.thin_client.data.SignInFeedback
import com.example.thin_client.data.app_preferences.PreferenceHandler
import com.example.thin_client.data.model.PrivateProfile
import com.example.thin_client.data.rooms.RoomManager

import com.example.thin_client.data.model.User
import com.example.thin_client.data.server.HTTPRequest
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.Lobby

import com.example.thin_client.ui.createUser.CreateUserActivity
import com.github.nkzawa.socketio.client.Socket
import com.google.gson.Gson
import kotlinx.android.synthetic.main.activity_createuser.*
import net.yslibrary.android.keyboardvisibilityevent.util.UIUtil
import okhttp3.Call
import java.io.IOException


class LoginActivity : AppCompatActivity() {


    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_login)
        UIUtil.hideKeyboard(this)
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
                        val httpClient = OkHttpRequest(okhttp3.OkHttpClient())
                        httpClient.GET(HTTPRequest.BASE_URL + HTTPRequest.URL_PRIVATE + user.username,
                            object: okhttp3.Callback {
                                //N'entre pas dans le on failure
                                override fun onFailure(call: Call, e: IOException) {
                                }
                                override fun onResponse(call: Call, response: okhttp3.Response) {
                                    val responseData = response.body?.charStream()
                                    val profileInfo = Gson().fromJson(responseData, PrivateProfile::class.java)
                                    runOnUiThread(({
                                        PreferenceHandler(applicationContext).setUser(profileInfo)
                                        val intent = Intent(applicationContext, Lobby::class.java)
                                        startActivity(intent)
                                        SocketHandler.isLoggedIn = true
                                        SocketHandler.socket!!.off(SocketEvent.USER_SIGNED_IN)
                                        finish()
                                    }))
                                }
                            }
                        )
                    } else {
                        Handler(Looper.getMainLooper()).post(Runnable {
                            Toast.makeText(
                                applicationContext,
                                signInFeedback.feedback.log_message,
                                Toast.LENGTH_SHORT
                            ).show()
                            loading.visibility = ProgressBar.GONE
                            login.isEnabled = true
                            SocketHandler.disconnect()
                        })
                    }
                }))
        }

        signup.setOnClickListener {
            val intent = Intent(this, CreateUserActivity::class.java)
            startActivity(intent)
        }

    }

    override fun onDestroy() {
        super.onDestroy()
        turnOffSocketEvents()
    }

    private fun showLoginFailed() {
        Toast.makeText(applicationContext, R.string.login_failed, Toast.LENGTH_SHORT).show()
    }

    override fun onBackPressed() {
        // Disable native back
    }

    private fun turnOffSocketEvents() {
        SocketHandler.socket!!.off(SocketEvent.USER_SIGNED_IN)
            .off(Socket.EVENT_CONNECT)
            .off(Socket.EVENT_CONNECT_ERROR)
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
