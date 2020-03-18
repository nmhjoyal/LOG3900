package com.example.thin_client.ui.game_mode.free_draw

import android.content.Context
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import com.example.thin_client.R
import com.example.thin_client.data.app_preferences.PreferenceHandler
import com.example.thin_client.data.app_preferences.Preferences
import com.example.thin_client.data.drawing.Stroke
import com.example.thin_client.data.drawing.StylusPoint
import com.example.thin_client.data.lifecycle.LoginState
import com.example.thin_client.data.model.User
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.google.gson.Gson
import kotlinx.android.synthetic.main.observer_fragment.*


class ObserverFragment : Fragment() {


    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        draw_view.isDrawer = false
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        return inflater.inflate(R.layout.observer_fragment, container, false)
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

        val prefs = context!!.getSharedPreferences(Preferences.USER_PREFS, Context.MODE_PRIVATE)
        when (SocketHandler.getLoginState(prefs)) {
            LoginState.FIRST_LOGIN -> {}
            LoginState.LOGIN_WITH_EXISTING -> {
                val user = PreferenceHandler(context!!).getUser()
                SocketHandler.login(User(user.username, user.password))
                SocketHandler.isLoggedIn = true
            }
            LoginState.LOGGED_IN -> {}

        }
    }

    private fun setupSocketEvents() {
        SocketHandler.socket!!
            .on(SocketEvent.NEW_POINT, ({ data ->
                val drawPoint = Gson().fromJson(data.first().toString(), StylusPoint::class.java)
                draw_view.addPath(drawPoint)
            }))
            .on(SocketEvent.NEW_STROKE, ({ data ->
                draw_view.stopTrace()
                val drawPoint = Gson().fromJson(data.first().toString(), Stroke::class.java)
                draw_view.startTrace(drawPoint)
            }))
    }

    private fun turnOffSocketEvents() {
        SocketHandler.socket!!
            .off(SocketEvent.NEW_POINT)
            .off(SocketEvent.NEW_STROKE)
    }
}
