package com.example.thin_client.ui.game_mode

import android.content.Context
import android.content.SharedPreferences
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.os.CountDownTimer
import android.os.Handler
import android.os.Looper
import android.widget.Toast
import androidx.fragment.app.FragmentManager
import com.example.thin_client.R
import com.example.thin_client.data.Feedback
import com.example.thin_client.data.app_preferences.Preferences
import com.example.thin_client.data.game.GameArgs
import com.example.thin_client.data.game.GameManager
import com.example.thin_client.data.game.MatchMode
import com.example.thin_client.data.game.StartMatch
import com.example.thin_client.data.lifecycle.LoginState
import com.example.thin_client.data.rooms.RoomArgs
import com.example.thin_client.data.rooms.RoomManager
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.chat.ChatFragment
import com.example.thin_client.ui.game_mode.free_draw.DrawerFragment
import com.example.thin_client.ui.game_mode.free_draw.ObserverFragment
import com.github.nkzawa.socketio.client.Socket
import com.google.gson.Gson
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.activity_game.*
import java.text.SimpleDateFormat
import java.util.*
import kotlin.collections.ArrayList

class GameActivity : AppCompatActivity(), ChatFragment.IWordGuessing {
    private lateinit var manager: FragmentManager
    private lateinit var prefs: SharedPreferences
    private val SECOND_INTERVAL: Long = 1000
    private var currentWordIndex = 0
    private var words = ArrayList<String>()
    private val lettersAdapter = GroupAdapter<GroupieViewHolder>()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_game)
        prefs = this.getSharedPreferences(Preferences.USER_PREFS, Context.MODE_PRIVATE)

        words.add("Dog")
        words.add("Champion")
        words.add("Professor")
        words.add("Medal")
        words.add("Lawn mower")

        get_drawing_button.setOnClickListener(({
            SocketHandler.getDrawing()
        }))

    }

    override fun onStart() {
        super.onStart()
        manager = supportFragmentManager
        setupSocket()
        when (GameManager.currentGameMode) {
            MatchMode.COLLABORATIVE-> {}
            MatchMode.FREE_FOR_ALL -> {}
            MatchMode.ONE_ON_ONE -> {}

        }
//        startGame()
    }

    override fun onStop() {
        super.onStop()
        turnOffSocketEvents()
    }

    override fun onDestroy() {
        super.onDestroy()
//        SocketHandler.disconnectOnlineDraw()
    }

    // Called when gameStarted and someone uses chat
    override fun sendGuess(word: String) {
    }

    private fun startGame() {
        for (letter in words[currentWordIndex]) {
            lettersAdapter.add(LetterHolder(letter.toString()))
        }
        word_letters.adapter = lettersAdapter
        startCountdown(10000)
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
//                SocketHandler.connectOnlineDraw()
            }
        }
    }

    private fun showChatFragment() {
        val transaction = manager.beginTransaction()
        val chatFragment = ChatFragment()
        val bundle = Bundle()
        if (!RoomManager.roomsJoined.containsKey(RoomManager.currentRoom)) {
            RoomManager.roomsJoined.put(RoomManager.currentRoom, arrayListOf())
            RoomManager.roomAvatars.put(RoomManager.currentRoom, mapOf())
        }
        bundle.putString(RoomArgs.ROOM_ID, RoomManager.currentRoom)
        bundle.putBoolean(GameArgs.IS_GAME_CHAT, true)
        chatFragment.arguments = bundle
        transaction.replace(R.id.chatrooms_container, chatFragment)
        transaction.addToBackStack(null)
        transaction.commitAllowingStateLoss()
    }

    private fun startCountdown(totalTime: Long) {
        val timePattern = "mm:ss"
        val simpleDateFormat = SimpleDateFormat(timePattern, Locale.US)
        time_text.text = simpleDateFormat.format(Date(totalTime))
        val timer = object : CountDownTimer(totalTime, SECOND_INTERVAL) {
            override fun onTick(millisUntilFinished: Long) {
                time_text.text = simpleDateFormat.format(Date(millisUntilFinished))
            }

            override fun onFinish() {
                time_text.text = simpleDateFormat.format(Date(0))
                currentWordIndex++
                if (currentWordIndex < words.size) {
                    lettersAdapter.clear()
                    startGame()
                }
            }
        }
        timer.start()
    }


    override fun onBackPressed() {
        finish()
    }

    private fun turnOffSocketEvents() {
        if (SocketHandler.socket != null) {
            SocketHandler.socket!!
                .off(SocketEvent.OBSERVER)
                .off(SocketEvent.DRAWER)
                .off(SocketEvent.MATCH_STARTED)
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
            }))
            .on(SocketEvent.OBSERVER, ({
                Handler(Looper.getMainLooper()).post(Runnable {
                    val transaction = manager.beginTransaction()
                    val observerFragment = ObserverFragment()
                    transaction.replace(R.id.draw_view_container, observerFragment)
                    transaction.addToBackStack(null)
                    transaction.commitAllowingStateLoss()
                })
            }))
            .on(SocketEvent.MATCH_STARTED, ({ data ->
                val feedback = Gson().fromJson(data.first().toString(), Feedback::class.java)
                if (feedback.status) {
//                    SocketHandler.getDrawing()
                } else {
                    Handler(Looper.getMainLooper()).post(Runnable {
                        Toast.makeText(this, feedback.log_message, Toast.LENGTH_LONG).show()
                    })
                }
            }))
    }


}
