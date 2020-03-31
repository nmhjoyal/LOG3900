package com.example.thin_client.ui.game_mode

import android.app.Dialog
import android.content.Context
import android.content.SharedPreferences
import android.os.Bundle
import android.os.CountDownTimer
import android.os.Handler
import android.os.Looper
import android.view.View
import android.view.WindowManager
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import androidx.fragment.app.FragmentManager
import androidx.recyclerview.widget.RecyclerView
import com.example.thin_client.R
import com.example.thin_client.data.app_preferences.PreferenceHandler
import com.example.thin_client.data.app_preferences.Preferences
import com.example.thin_client.data.game.*
import com.example.thin_client.data.game.GameArgs
import com.example.thin_client.data.game.GameManager
import com.example.thin_client.data.game.MatchMode
import com.example.thin_client.data.lifecycle.LoginState
import com.example.thin_client.data.rooms.RoomArgs
import com.example.thin_client.data.rooms.RoomManager
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.chat.ChatFragment
import com.example.thin_client.ui.game_mode.free_draw.DrawerFragment
import com.example.thin_client.ui.game_mode.free_draw.ObserverFragment
import com.example.thin_client.ui.game_mode.free_draw.WordHolder
import com.example.thin_client.ui.game_mode.waitingroom.WaitingRoom
import com.google.gson.Gson
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.activity_game.*
import java.text.SimpleDateFormat
import java.util.*

class GameActivity : AppCompatActivity(), ChatFragment.IGuessWord {
    private lateinit var manager: FragmentManager
    private lateinit var prefs: SharedPreferences
    private var wordToGuess: String = ""
    private var currentUser = ""
    private var isHost = false
    private val lettersAdapter = GroupAdapter<GroupieViewHolder>()
    private var timer: CountDownTimer? = null
    private var isGameStarted = false
    private var currentDrawer = ""
    private var nbTries = 0
    private var firstTurnStarted = false
    private var currentRound = 1

    private val SECOND_INTERVAL: Long = 1000
    private val TIME_PATTERN = "mm:ss"
    private val POINT_SCREEN_TIMEOUT: Long = 5000
    private val ROUND_SCREEN_TIMEOUT: Long = 2000

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_game)
        prefs = this.getSharedPreferences(Preferences.USER_PREFS, Context.MODE_PRIVATE)

        currentUser = PreferenceHandler(this).getUser().username
        draw_view_container.bringToFront()
    }

    override fun onStart() {
        super.onStart()
        manager = supportFragmentManager
        setupSocket()
        when (GameManager.currentGameMode) {
            MatchMode.SOLO -> {
            }
            MatchMode.COLLABORATIVE-> {}
            MatchMode.FREE_FOR_ALL -> {
                if (!isGameStarted) {
                    val transaction = manager.beginTransaction()
                    val waitingRoom = WaitingRoom()
                    transaction.replace(R.id.draw_view_container, waitingRoom)
                    transaction.addToBackStack(null)
                    transaction.commitAllowingStateLoss()
                }
            }
            MatchMode.ONE_ON_ONE -> {}

        }
    }

    override fun onStop() {
        super.onStop()
        turnOffSocketEvents()
    }

    override fun guessSent() {
        nbTries--
        nb_guesses.text = nbTries.toString()
        if (nbTries == 0) {
            GameManager.canGuess = false
        }
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
            }
        }
    }

    private fun showChatFragment() {
        val transaction = manager.beginTransaction()
        val chatFragment = ChatFragment()
        val bundle = Bundle()
        bundle.putString(RoomArgs.ROOM_ID, RoomManager.currentRoom)
        bundle.putBoolean(GameArgs.IS_GAME_CHAT, true)
        chatFragment.arguments = bundle
        transaction.replace(R.id.chatrooms_container, chatFragment)
        transaction.addToBackStack(null)
        transaction.commitAllowingStateLoss()
    }

    private fun showDrawerFragment() {
        val transaction = manager.beginTransaction()
        val drawerFragment = DrawerFragment()
        transaction.replace(R.id.draw_view_container, drawerFragment)
        transaction.addToBackStack(null)
        transaction.commitAllowingStateLoss()
        points_view.visibility = View.GONE
        draw_view_container.bringToFront()
    }

    private fun showObserverFragment() {
        val transaction = manager.beginTransaction()
        val observerFragment = ObserverFragment()
        transaction.replace(R.id.draw_view_container, observerFragment)
        transaction.addToBackStack(null)
        transaction.commitAllowingStateLoss()
        points_view.visibility = View.VISIBLE
        showUserChoosingWord()
    }

    private fun startCountdown(totalTime: Long) {
        val timePattern = TIME_PATTERN
        val simpleDateFormat = SimpleDateFormat(timePattern, Locale.US)
        time_text.text = simpleDateFormat.format(Date(totalTime))
        timer = object : CountDownTimer(totalTime, SECOND_INTERVAL) {
            override fun onTick(millisUntilFinished: Long) {
                time_text.text = simpleDateFormat.format(Date(millisUntilFinished))
            }

            override fun onFinish() {
                time_text.text = simpleDateFormat.format(Date(0))
            }
        }
        timer?.start()
    }

    private fun setupWordHolder() {
        for (letter in wordToGuess) {
            lettersAdapter.add(LetterHolder(letter.toString(), isHost))
        }
        word_letters.adapter = lettersAdapter
    }


    private fun showWordsSelection(words: Array<String>) {
        val dialog = Dialog(this)
        dialog.setCancelable(false)
        dialog.setContentView(R.layout.dialog_choose_word)
        val adapter = GroupAdapter<GroupieViewHolder>()
        val wordRecycler = dialog.findViewById<RecyclerView>(R.id.word_choices)
        for (word in words) {
            adapter.add(WordHolder(word))
        }

        adapter.setOnItemClickListener(({ item, v ->
            val selectedWord = (item as WordHolder).text
            wordToGuess = selectedWord
            SocketHandler.startTurn(selectedWord)
            showDrawerFragment()
            dialog.dismiss()
        }))
        wordRecycler.adapter = adapter
        dialog.show()
    }

    private fun resetTurn(drawer: String, currentUserPoints: Number?) {
        if (timer != null) {
            timer?.cancel()
            timer?.onFinish()
        }
        lettersAdapter.clear()
        wordToGuess = ""
        message.text = ""
        nbTries = 3
        GameManager.canGuess = true
        isHost = drawer.equals(currentUser)
        total_points.text = if (currentUserPoints != null) currentUserPoints.toString() else "0"
        currentDrawer = drawer
    }

    private fun delegateViews(choices: Array<String>) {
        if (isHost) {
            showWordsSelection(choices)
        } else {
            showObserverFragment()
        }
    }


    override fun onBackPressed() {
        val alertDialog = AlertDialog.Builder(this)
        alertDialog.setTitle(R.string.leave_match)
            .setMessage(R.string.leave_match_ask)
            .setPositiveButton(R.string.yes) { _, _ ->
                SocketHandler.leaveMatch()
                finish()
            }
            .setNegativeButton(R.string.cancel) { _, _ -> }

        val dialog = alertDialog.create()
        dialog.window!!.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE)
        dialog.show()
    }

    private fun turnOffSocketEvents() {
        if (SocketHandler.socket != null) {
            SocketHandler.socket!!
                .off(SocketEvent.TURN_ENDED)
                .off(SocketEvent.TURN_STARTED)
                .off(SocketEvent.MATCH_LEFT)
                .off(SocketEvent.MATCH_STARTED)
        }
    }

    private fun setupSocketEvents() {
        if (SocketHandler.socket != null) {
            SocketHandler.socket!!
                .on(SocketEvent.TURN_ENDED, ({ data ->
                    val turnParams = Gson().fromJson(data.first().toString(), EndTurn::class.java)
                    Handler(Looper.getMainLooper()).post(({
                        resetTurn(turnParams.drawer, turnParams.scores[currentUser]?.scoreTotal)
                        user_block.bringToFront()
                        if (!firstTurnStarted) {
                            message.text = String.format(resources.getString(R.string.round), turnParams.currentRound.toInt())
                            user_points.visibility = View.GONE
                            Handler().postDelayed({
                                delegateViews(turnParams.choices)
                            }, ROUND_SCREEN_TIMEOUT)
                        } else if (turnParams.currentRound.toInt() != currentRound) {
                            currentRound = turnParams.currentRound.toInt()
                            val pointsAdapter = GroupAdapter<GroupieViewHolder>()
                            message.text = String.format(resources.getString(R.string.word_was), wordToGuess)
                            for (score in turnParams.scores) {
                                pointsAdapter.add(PlayerPointHolder(score.key, score.value.scoreTurn.toInt()))
                            }
                            pointsAdapter.add(PlayerPointHolder("user", 20))
                            pointsAdapter.add(PlayerPointHolder("user", 10))
                            pointsAdapter.add(PlayerPointHolder("user", 0))
                            user_points.adapter = pointsAdapter
                            user_points.visibility = View.VISIBLE
                            Handler().postDelayed({
                                user_points.visibility = View.GONE
                                message.text = String.format(resources.getString(R.string.round), turnParams.currentRound.toInt())
                                Handler().postDelayed({
                                    delegateViews(turnParams.choices)
                                }, ROUND_SCREEN_TIMEOUT)
                            }, POINT_SCREEN_TIMEOUT)
                        } else {
                            message.text = String.format(resources.getString(R.string.word_was), wordToGuess)
                            user_points.visibility = View.GONE
                            Handler().postDelayed({
                                delegateViews(turnParams.choices)
                            }, POINT_SCREEN_TIMEOUT)
                        }
                    }))
                }))
                .on(SocketEvent.TURN_STARTED, ({ data ->
                    Handler(Looper.getMainLooper()).post(Runnable {
                        firstTurnStarted = true
                        draw_view_container.bringToFront()
                        val time = Gson().fromJson(data.first().toString(), Number::class.java)
                        startCountdown(time.toLong() * SECOND_INTERVAL)
                        setupWordHolder()
                    })
                }))
                .on(SocketEvent.MATCH_STARTED, ({ data ->
                    val feedback =
                        Gson().fromJson(data.first().toString(), StartMatchFeedback::class.java)
                    if (feedback.feedback.status) {
                        isGameStarted = true
                        Handler(Looper.getMainLooper()).post(Runnable {
                            user_block.bringToFront()
                        })
                    } else {
                        Handler(Looper.getMainLooper()).post(Runnable {
                            Toast.makeText(this, feedback.feedback.log_message, Toast.LENGTH_LONG)
                                .show()
                        })
                    }
                }))
        }
    }

    private fun showUserChoosingWord() {
        message.text = String.format(resources.getString(R.string.user_choosing_word), currentDrawer)
        user_points.visibility = View.GONE
        user_block.bringToFront()
    }
}
