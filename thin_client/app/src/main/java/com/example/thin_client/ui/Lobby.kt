package com.example.thin_client.ui

import android.app.AlertDialog
import android.content.Context
import android.content.Intent
import android.content.SharedPreferences
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.Menu
import android.view.MenuItem
import android.view.View
import android.view.WindowManager
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.isVisible
import androidx.fragment.app.FragmentManager
import com.example.thin_client.R
import com.example.thin_client.data.Feedback
import com.example.thin_client.data.SignInFeedback
import com.example.thin_client.data.app_preferences.PreferenceHandler
import com.example.thin_client.data.app_preferences.Preferences
import com.example.thin_client.data.game.GameArgs
import com.example.thin_client.data.game.GameManager
import com.example.thin_client.data.game.MatchMode
import com.example.thin_client.data.lifecycle.LoginState
import com.example.thin_client.data.model.MatchInfos
import com.example.thin_client.data.model.User
import com.example.thin_client.data.rooms.JoinRoomFeedback
import com.example.thin_client.data.rooms.RoomArgs
import com.example.thin_client.data.rooms.RoomManager
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.chat.ChatFragment
import com.example.thin_client.ui.chatrooms.ChatRoomsFragment
import com.example.thin_client.ui.game_mode.GameActivity
import com.example.thin_client.ui.game_mode.MatchList
import com.example.thin_client.ui.game_mode.free_draw.FreeDrawActivity
import com.example.thin_client.ui.leaderboard.LeaderboardActivity
import com.example.thin_client.ui.login.LoginActivity
import com.example.thin_client.ui.profile.ProfileActivity
import com.github.nkzawa.socketio.client.Socket
import com.google.gson.Gson
import kotlinx.android.synthetic.main.activity_lobby.*

class Lobby : AppCompatActivity(), MatchList.IGameStarter, LobbyMenuFragment.IStartNewFragment {
    private lateinit var manager: FragmentManager
    private lateinit var prefs: SharedPreferences

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_lobby)
        manager = supportFragmentManager

        val transaction = manager.beginTransaction()
        val lobbyMenuFragment = LobbyMenuFragment()
        transaction.add(R.id.lobby_menu_container, lobbyMenuFragment)
        transaction.commit()

        prefs = this.getSharedPreferences(Preferences.USER_PREFS, Context.MODE_PRIVATE)
        show_rooms_button.setOnClickListener(({
            if (chatrooms_container.isVisible) {
                chatrooms_container.visibility = View.GONE
                show_rooms_button.setImageResource(R.drawable.ic_open)
            } else {
                chatrooms_container.visibility = View.VISIBLE
                show_rooms_button.setImageResource(R.drawable.hide)
            }
        }))

    }

    override fun startGame() {
        val intent = Intent(this, GameActivity::class.java)
        startActivity(intent)
    }

    override fun onStart() {
        super.onStart()
        manager = supportFragmentManager
        SocketHandler.searchMatches()
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

    override fun startFreeDraw() {
        val intent = Intent(this, FreeDrawActivity::class.java)
        startActivity(intent)
    }

    override fun startGameList() {
        val transaction = manager.beginTransaction()
        val gamesList = MatchList()
        transaction.replace(R.id.lobby_menu_container, gamesList)
        transaction.addToBackStack(null)
        transaction.commit()
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
        if(supportFragmentManager.backStackEntryCount > 1 )
            super.onBackPressed()
        else
            return
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
            .on(SocketEvent.UPDATE_MATCHES, ({data ->
                val gson = Gson()
                val matchInfosFeedback=
                    gson.fromJson(data.first().toString(), ArrayList<MatchInfos>()::class.java)
                for(match in matchInfosFeedback) {
                    when(match.matchMode){
                            MatchMode.SOLO -> GameManager.soloModeMatchList.add(match)
                            MatchMode.COLLABORATIVE-> GameManager.collabModeMatchList.add(match)
                            MatchMode.FREE_FOR_ALL -> GameManager.freeForAllMatchList.add(match)
                            else -> {
                                GameManager.oneVsOneMatchList.add(match)
                        }
                    }
                }
            }))
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
                Handler(Looper.getMainLooper()).post(Runnable {
                    val alertDialog = AlertDialog.Builder(this)
                    alertDialog.setTitle(R.string.error_connect_title)
                        .setCancelable(false)
                        .setMessage(R.string.error_connect)
                        .setPositiveButton(R.string.ok) { _, _ -> finishAffinity() }

                    val dialog = alertDialog.create()
                    dialog.window!!.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE)
                    dialog.show()
                })
                SocketHandler.disconnect()
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



}
