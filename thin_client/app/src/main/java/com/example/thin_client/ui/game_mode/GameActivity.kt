package com.example.thin_client.ui.game_mode

import android.content.Context
import android.content.SharedPreferences
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import androidx.fragment.app.FragmentManager
import com.example.thin_client.R
import com.example.thin_client.data.app_preferences.Preferences
import com.example.thin_client.data.game.GameArgs
import com.example.thin_client.data.lifecycle.LoginState
import com.example.thin_client.data.rooms.RoomArgs
import com.example.thin_client.data.rooms.RoomManager
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.chat.ChatFragment
import com.example.thin_client.ui.game_mode.free_draw.DrawerFragment
import com.example.thin_client.ui.game_mode.free_draw.ObserverFragment

class GameActivity : AppCompatActivity() {
    private lateinit var manager: FragmentManager
    private lateinit var prefs: SharedPreferences

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_game)
        prefs = this.getSharedPreferences(Preferences.USER_PREFS, Context.MODE_PRIVATE)
    }

    override fun onStart() {
        super.onStart()
        manager = supportFragmentManager
        setupSocket()
    }

    override fun onStop() {
        super.onStop()
        turnOffSocketEvents()
    }

    override fun onDestroy() {
        super.onDestroy()
        SocketHandler.disconnectOnlineDraw()
    }

    private fun setupSocket() {
        if (!SocketHandler.isConnected()) {
            SocketHandler.connect()
        }

        setupSocketEvents()

        when (SocketHandler.getLoginState(prefs)) {
            LoginState.FIRST_LOGIN -> {}
            LoginState.LOGIN_WITH_EXISTING -> {
                finish()
            }
            LoginState.LOGGED_IN -> {
                showChatFragment()
                SocketHandler.connectOnlineDraw()
            }
        }
    }

    private fun showChatFragment() {
        val transaction = manager.beginTransaction()
        val chatFragment = ChatFragment()
        val bundle = Bundle()
        if (!RoomManager.roomsJoined.containsKey("placeholderRoom")) {
            RoomManager.roomsJoined.put("placeholderRoom", arrayListOf())
            RoomManager.roomAvatars.put("placeholderRoom", mapOf())
        }
        bundle.putString(RoomArgs.ROOM_ID, "placeholderRoom")
        bundle.putBoolean(GameArgs.IS_GAME_CHAT, true)
        chatFragment.arguments = bundle
        transaction.replace(R.id.chatrooms_container, chatFragment)
        transaction.addToBackStack(null)
        transaction.commitAllowingStateLoss()
    }


    override fun onBackPressed() {
    }

    private fun turnOffSocketEvents() {
        if (SocketHandler.socket != null) {
            SocketHandler.socket!!
                .off(SocketEvent.OBSERVER)
                .off(SocketEvent.DRAWER)
        }
    }

    private fun setupSocketEvents() {
        SocketHandler.socket!!
            .on(SocketEvent.DRAWER, ({
                Handler(Looper.getMainLooper()).post(Runnable {
                    val transaction = manager.beginTransaction()
                    val drawerFragment = DrawerFragment()
                    transaction.replace(R.id.draw_view_container, drawerFragment)
                    transaction.addToBackStack(null)
                    transaction.commitAllowingStateLoss()
                })
            })).on(SocketEvent.OBSERVER, ({
                Handler(Looper.getMainLooper()).post(Runnable {
                    val transaction = manager.beginTransaction()
                    val observerFragment = ObserverFragment()
                    transaction.replace(R.id.draw_view_container, observerFragment)
                    transaction.addToBackStack(null)
                    transaction.commitAllowingStateLoss()
                })
            }))

    }
}
