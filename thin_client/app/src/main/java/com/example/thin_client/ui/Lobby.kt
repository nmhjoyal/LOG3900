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
import android.widget.RadioGroup
import android.widget.Toast
import androidx.core.view.isVisible
import androidx.fragment.app.FragmentManager
import com.example.thin_client.R
import com.example.thin_client.data.Feedback
import com.example.thin_client.data.app_preferences.Preferences
import com.example.thin_client.data.SignInFeedback
import com.example.thin_client.data.app_preferences.PreferenceHandler
import com.example.thin_client.data.game.GameArgs
import com.example.thin_client.data.game.GameManager
import com.example.thin_client.data.game.GameMode
import com.example.thin_client.data.lifecycle.LoginState
import com.example.thin_client.data.model.User
import com.example.thin_client.data.rooms.JoinRoomFeedback
import com.example.thin_client.data.rooms.RoomArgs
import com.example.thin_client.data.rooms.RoomManager
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.chat.ChatFragment
import com.example.thin_client.ui.chatrooms.ChatRoomsFragment
import com.example.thin_client.ui.game_mode.GameActivity
import com.example.thin_client.ui.game_mode.WaitingRoom
import com.example.thin_client.ui.game_mode.free_draw.FreeDrawActivity
import com.example.thin_client.ui.leaderboard.LeaderboardActivity
import com.example.thin_client.ui.login.LoginActivity
import com.example.thin_client.ui.profile.ProfileActivity
import com.github.nkzawa.socketio.client.Socket
import com.google.gson.Gson
import kotlinx.android.synthetic.main.activity_lobby.*

class Lobby : AppCompatActivity() {
    private lateinit var manager: FragmentManager
    private lateinit var prefs: SharedPreferences

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_lobby)
        manager = supportFragmentManager

        prefs = this.getSharedPreferences(Preferences.USER_PREFS, Context.MODE_PRIVATE)

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
                show_rooms_button.setImageResource(R.drawable.hide)
            }
        }))

        join_match.setOnClickListener(({
            val intent = Intent(applicationContext, WaitingRoom::class.java)
            startActivity(intent)
        }))

        create_match.setOnClickListener(({
            showCreateMatchDialog()
        }))
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

    private fun setupSocket() {
        if (!SocketHandler.isConnected()) {
            SocketHandler.connect()
        }

        setupSocketEvents()

        when (SocketHandler.getLoginState(prefs)) {
            LoginState.FIRST_LOGIN -> {
                val intent = Intent(applicationContext, LoginActivity::class.java)
                startActivity(intent)
                SocketHandler.disconnect()
            }
            LoginState.LOGIN_WITH_EXISTING -> {
                val user = PreferenceHandler(applicationContext).getUser()
                SocketHandler.login(User(user.username, user.password))
                SocketHandler.isLoggedIn = true
            }
            LoginState.LOGGED_IN -> {
                showChatRoomsFragment()
            }

        }
    }

    private fun showChatRoomsFragment() {
        val transaction = manager.beginTransaction()
        val chatroomsFragment = ChatRoomsFragment()
        transaction.replace(R.id.chatrooms_container, chatroomsFragment)
        transaction.addToBackStack(null)
        transaction.commitAllowingStateLoss()
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
            R.id.leaderboard -> {
                val intent = Intent(applicationContext, LeaderboardActivity::class.java)
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

    private fun turnOffSocketEvents() {
        if (SocketHandler.socket != null) {
            SocketHandler.socket!!
                .off(SocketEvent.USER_SIGNED_OUT)
                .off(Socket.EVENT_ERROR)
        }
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
            .on(SocketEvent.USER_SIGNED_OUT, ({ data ->
                val feedback = Gson().fromJson(data.first().toString(),Feedback::class.java)
                    PreferenceHandler(this).resetUserPrefs()
                    val intent = Intent(applicationContext, LoginActivity::class.java)
                    startActivity(intent)
                    SocketHandler.disconnect()
                    Handler(Looper.getMainLooper()).post(Runnable {
                        Toast.makeText(
                            applicationContext,
                            feedback.log_message,
                            Toast.LENGTH_SHORT
                        ).show()
                    })
            }))
            .on(SocketEvent.USER_JOINED_ROOM, ({ data ->
                val feedback = Gson().fromJson(data.first().toString(), JoinRoomFeedback::class.java)
                if (feedback.feedback.status) {
                    RoomManager.addRoom(feedback.room_joined!!)
                }
                val roomID = if (RoomManager.currentRoom == "") "General" else RoomManager.currentRoom
                Handler(Looper.getMainLooper()).post(Runnable {
                    val bundle = Bundle()
                    bundle.putString(RoomArgs.ROOM_ID, roomID)
                    bundle.putBoolean(GameArgs.IS_GAME_CHAT, false)
                    val transaction = manager.beginTransaction()
                    val chatFragment = ChatFragment()
                    chatFragment.arguments = bundle
                    transaction.replace(R.id.chatrooms_container, chatFragment)
                    transaction.addToBackStack(null)
                    transaction.commitAllowingStateLoss()
                })

            }))
            .on(Socket.EVENT_DISCONNECT, ({
                SocketHandler.socket = null
                SocketHandler.isLoggedIn = false
            }))
    }

    private fun showCreateMatchDialog() {
        val alertBuilder = android.app.AlertDialog.Builder(this)
        alertBuilder.setTitle(R.string.create_match)
        val dialogView = layoutInflater.inflate(R.layout.dialog_create_match, null)
        alertBuilder.setView(dialogView)
        val gameRadioGroup = dialogView.findViewById<RadioGroup>(R.id.game_mode_selection)
        gameRadioGroup.check(R.id.is_solo_mode)

        alertBuilder
            .setPositiveButton(R.string.start) { _, _ ->
                when(gameRadioGroup.checkedRadioButtonId) {
                    R.id.is_solo_mode -> {
                        GameManager.currentGameMode = GameMode.SOLO
                    }
                    R.id.is_collab_mode -> {
                        GameManager.currentGameMode = GameMode.COLLAB
                    }
                    R.id.is_general_mode -> {
                        GameManager.currentGameMode = GameMode.GENERAL
                    }
                    R.id.is_one_on_one_mode -> {
                        GameManager.currentGameMode = GameMode.ONE_V_ONE
                    }
                    R.id.is_inverse_mode -> {
                        GameManager.currentGameMode = GameMode.REVERSE
                    }
                }
                val intent = Intent(applicationContext, GameActivity::class.java)
                startActivity(intent)
            }
            .setNegativeButton(R.string.cancel) { _, _ -> }
        val dialog = alertBuilder.create()
        dialog.window!!.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE)
        dialog.show()
    }
}
