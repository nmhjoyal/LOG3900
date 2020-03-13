package com.example.thin_client.ui.game_mode.free_draw

import android.content.Context
import android.os.Bundle
import android.view.MenuItem
import androidx.appcompat.app.AppCompatActivity
import com.example.thin_client.R
import com.example.thin_client.data.app_preferences.PreferenceHandler
import com.example.thin_client.data.app_preferences.Preferences
import com.example.thin_client.data.drawing.DrawPoint
import com.example.thin_client.data.lifecycle.LoginState
import com.example.thin_client.data.model.User
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.google.gson.Gson
import kotlinx.android.synthetic.main.activity_observer.*


class TestOnlineDrawActivity : AppCompatActivity() {


    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_observer)
        draw_view.isDrawer = false
    }

    override fun onStart() {
        super.onStart()
        setupSocket()
    }

    override fun onStop() {
        super.onStop()
        turnOffSocketEvents()

    }

    private fun setupSocket() {
        if (!SocketHandler.isConnected()) {
            SocketHandler.connect()
        }

        setupSocketEvents()

        val prefs = this.getSharedPreferences(Preferences.USER_PREFS, Context.MODE_PRIVATE)
        when (SocketHandler.getLoginState(prefs)) {
            LoginState.FIRST_LOGIN -> {}
            LoginState.LOGIN_WITH_EXISTING -> {
                val user = PreferenceHandler(applicationContext).getUser()
                SocketHandler.login(User(user.username, user.password))
                SocketHandler.isLoggedIn = true
            }
            LoginState.LOGGED_IN -> {}

        }
    }

    private fun setupSocketEvents() {
        SocketHandler.socket!!
            .on(SocketEvent.DRAW_POINT, ({ data ->
                val drawPoint = Gson().fromJson(data.first().toString(), DrawPoint::class.java)
                draw_view.addPath(drawPoint)
            }))
            .on(SocketEvent.START_TRACE, ({ data ->
                val drawPoint = Gson().fromJson(data.first().toString(), DrawPoint::class.java)
                draw_view.startTrace(drawPoint)
            }))
            .on(SocketEvent.STOP_TRACE, ({
                draw_view.stopTrace()
            }))
    }

    private fun turnOffSocketEvents() {
        SocketHandler.socket!!
            .off(SocketEvent.DRAW_POINT)
            .off(SocketEvent.START_TRACE)
            .off(SocketEvent.STOP_TRACE)
    }

    override fun onBackPressed() {
        // Disable native back
    }

    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        when (item.itemId) {
            android.R.id.home -> {
                SocketHandler.disconnectOnlineDraw()
                finish()
                return true
            }
        }
        return super.onOptionsItemSelected(item)
    }
}
