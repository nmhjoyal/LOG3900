package com.example.thin_client.ui.login

import android.app.Activity

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
import androidx.annotation.StringRes
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProviders
import com.example.thin_client.R
import com.example.thin_client.data.SignedInResponse

import com.example.thin_client.data.model.User
import com.example.thin_client.server.SocketHandler

import com.example.thin_client.ui.chatrooms.ChatRoomsFragment
import com.example.thin_client.ui.createUser.CreateUserActivity
import com.example.thin_client.ui.Lobby
import com.example.thin_client.ui.chat.ChatActivity
import com.github.nkzawa.socketio.client.Socket
import com.google.gson.Gson


class LoginActivity : AppCompatActivity() {

    private lateinit var loginViewModel: LoginViewModel

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_login)

        val username = findViewById<EditText>(R.id.username)
       // val ipAddress = findViewById<EditText>(R.id.ipAddress)
        val login = findViewById<Button>(R.id.login)
        val signup = findViewById<Button>(R.id.signup)
        val loading = findViewById<ProgressBar>(R.id.loading)

        loading.visibility = View.INVISIBLE
        loginViewModel = ViewModelProviders.of(this, LoginViewModelFactory())
            .get(LoginViewModel::class.java)

        loginViewModel.loginFormState.observe(this@LoginActivity, Observer {
            val loginState = it ?: return@Observer

            // disable login button unless both username / password is valid
            login.isEnabled = loginState.isDataValid

            if (loginState.usernameError != null) {
                username.error = getString(loginState.usernameError)
            }

        })

        loginViewModel.loginResult.observe(this@LoginActivity, Observer {
            val loginResult = it ?: return@Observer

            if (loginResult.error != null) {
                showLoginFailed(loginResult.error)
            }
            if (loginResult.success != null) {
                updateUiWithUser(loginResult.success)
            }
            setResult(Activity.RESULT_OK)
        })

        username.afterTextChanged {
            loginViewModel.loginDataChanged(
                username.text.toString()
            )
        }


        login.setOnClickListener {
                loading.visibility = ProgressBar.VISIBLE
                login.isEnabled = false
                val socket = SocketHandler.connect()
                socket.on(Socket.EVENT_CONNECT, ({
                    SocketHandler.login(User(username.text.toString(), "test123"))
                }))
                    .on(Socket.EVENT_CONNECT_ERROR, ({
                    Handler(Looper.getMainLooper()).post(Runnable {
                        Toast.makeText(applicationContext, "Unable to connect", Toast.LENGTH_SHORT).show()
                        loading.visibility = ProgressBar.GONE
                        login.isEnabled = true
                    })
                    SocketHandler.disconnect()
                }))
                .on("user_signed_in", ({ data ->
                    val gson = Gson()
                    val signedInResponse = gson.fromJson(data.first().toString(), SignedInResponse::class.java )

                    if (signedInResponse.signed_in) {
                        val intent = Intent(applicationContext, Lobby::class.java)
                        startActivity(intent)
                        finish()
                    } else {
                        Handler(Looper.getMainLooper()).post(Runnable {
                            Toast.makeText(
                                applicationContext,
                                signedInResponse.log_message,
                                Toast.LENGTH_SHORT
                            ).show()
                            loading.visibility = ProgressBar.GONE
                            login.isEnabled = true
                        })
                        SocketHandler.disconnect()
                    }
                    }))


        }

            signup.setOnClickListener {
            val intent = Intent(this, CreateUserActivity::class.java)
            startActivity(intent)
        }

    }

    private fun updateUiWithUser(model: LoggedInUserView) {
        val welcome = getString(R.string.welcome)
        val displayName = model.displayName
        // TODO : initiate successful logged in experience
        Toast.makeText(
            applicationContext,
            "$welcome $displayName",
            Toast.LENGTH_LONG
        ).show()
    }

    private fun showLoginFailed(@StringRes errorString: Int) {
        Toast.makeText(applicationContext, errorString, Toast.LENGTH_SHORT).show()
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
