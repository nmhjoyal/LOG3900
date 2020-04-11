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
import androidx.core.view.isVisible
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
import com.example.thin_client.ui.helpers.DEFAULT_INTERVAL
import com.example.thin_client.ui.helpers.setOnClickListener
import com.google.gson.Gson
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.activity_game.*
import java.text.SimpleDateFormat
import java.util.*


const val SECOND_INTERVAL: Long = 1000
const val TIME_PATTERN = "mm:ss"
const val POINT_SCREEN_TIMEOUT: Long = 5000
const val ROUND_SCREEN_TIMEOUT: Long = 2000

class GameActivity : AppCompatActivity(), ChatFragment.IGuessWord {
    private lateinit var manager: FragmentManager
    private lateinit var prefs: SharedPreferences
    private var wordBeingDrawn: String = ""
    private var currentUser = ""
    private var isHost = false
    private val lettersAdapter = GroupAdapter<GroupieViewHolder>()
    private var playerPointsAdapter = GroupAdapter<GroupieViewHolder>()
    private var timer: CountDownTimer? = null
    private var isGameStarted = false
    private var isWaitingRoomShowing = false
    private var currentDrawer = ""
    private var nbTries = 0
    private var firstTurnStarted = false
    private var isTurnStarted = false
    private var currentRound = 1
    private var wordWasString = ""
    private var nonVirtualPlayers = ArrayList<Player>()

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_game)
        prefs = this.getSharedPreferences(Preferences.USER_PREFS, Context.MODE_PRIVATE)

        currentUser = PreferenceHandler(this).getUser().username
        back_to_lobby.setOnClickListener(DEFAULT_INTERVAL){
            cleanupAndFinish()
        }

        show_points_button.setOnClickListener(({
            if (user_points_toolbar.isVisible) {
                user_points_toolbar.visibility = View.GONE
                show_points_button.setImageResource(R.drawable.hide)
            } else {
                user_points_toolbar.visibility = View.VISIBLE
                show_points_button.setImageResource(R.drawable.ic_open)
            }
        }))

        if (GameManager.currentGameMode == MatchMode.FREE_FOR_ALL ||
                GameManager.currentGameMode == MatchMode.ONE_ON_ONE) {
            attempts.visibility = View.GONE
        }

        draw_view_container.bringToFront()
    }

    override fun onStart() {
        super.onStart()
        manager = supportFragmentManager
        setupSocket()
        if (!isWaitingRoomShowing) {
            toolbar.visibility = View.GONE
            user_points_toolbar.visibility = View.GONE
            val transaction = manager.beginTransaction()
            val waitingRoom = WaitingRoom()
            transaction.replace(R.id.draw_view_container, waitingRoom)
            transaction.addToBackStack(null)
            transaction.commitAllowingStateLoss()
            isWaitingRoomShowing = true
        }
    }

    override fun onStop() {
        super.onStop()
        turnOffSocketEvents()
    }

    private fun cleanupAndFinish() {
        GameManager.isGameStarted = false
        RoomManager.roomsJoined.remove(RoomManager.currentRoom)
        RoomManager.roomAvatars.remove(RoomManager.currentRoom)
        RoomManager.currentRoom = ""
        finish()
    }

    override fun guessSent() {
        nbTries--
        nb_guesses.text = nbTries.toString()
        if (nbTries == 0) {
            GameManager.canGuess = false
        }
    }

    override fun sendWord(text: String) {
        wordWasString = text
    }

    private fun setupSocket() {
        if (!SocketHandler.isConnected()) {
            SocketHandler.connect()
        }

        setupSocketEvents()

        when (SocketHandler.getLoginState(prefs)) {
            LoginState.FIRST_LOGIN -> {}
            LoginState.LOGIN_WITH_EXISTING -> {
                cleanupAndFinish()
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
        draw_view_container.bringToFront()
    }

    private fun showObserverFragment() {
        val transaction = manager.beginTransaction()
        val observerFragment = ObserverFragment()
        transaction.replace(R.id.draw_view_container, observerFragment)
        transaction.addToBackStack(null)
        transaction.commitAllowingStateLoss()
        if (!isTurnStarted) {
            showUserChoosingWord()
        }
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
        for (letter in wordBeingDrawn) {
            lettersAdapter.add(LetterHolder(letter.toString(), isHost))
        }
        word_letters.adapter = lettersAdapter
    }

    private fun refreshPlayerPointsToolbar() {
        playerPointsAdapter.clear()
        for (player in nonVirtualPlayers) {
            playerPointsAdapter.add(PlayerPointToolbarHolder(player.user, player.score.scoreTotal.toInt()))
        }
        user_points_total.adapter = playerPointsAdapter
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
            wordBeingDrawn = selectedWord
            SocketHandler.startTurn(selectedWord)
            showDrawerFragment()
            dialog.dismiss()
        }))
        wordRecycler.adapter = adapter
        dialog.show()
    }

    private fun resetTurn(drawer: String) {
        if (timer != null) {
            timer?.cancel()
            timer?.onFinish()
        }
        lettersAdapter.clear()
        wordBeingDrawn = ""
        message.text = ""
        nbTries = 3
        GameManager.canGuess = true
        isHost = drawer.equals(currentUser)
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
                cleanupAndFinish()
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
                .off(SocketEvent.UPDATE_SPRINT)
                .off(SocketEvent.UNEXPECTED_LEAVE)
        }
    }

    private fun setupSocketEvents() {
        if (SocketHandler.socket != null) {
            SocketHandler.socket!!
                .on(SocketEvent.UPDATE_SPRINT, ({data ->
                    isTurnStarted = true
                    val sprintParams = Gson().fromJson(data.first().toString(), UpdateSprint::class.java)
                    getNonVirtualPlayers(sprintParams.players)
                    Handler(Looper.getMainLooper()).post(({
                        draw_view_container.bringToFront()
                        refreshPlayerPointsToolbar()
                        if (timer != null) {
                            timer?.cancel()
                            timer?.onFinish()
                        }
                        startCountdown(sprintParams.time.toLong() * SECOND_INTERVAL)
                        if(sprintParams.guess == 0){
                            GameManager.canGuess = false
                            nb_guesses.text = ""
                        }
                        GameManager.canGuess = true
                        nbTries = sprintParams.guess.toInt()
                        nb_guesses.text = sprintParams.guess.toString()
                        message.text = sprintParams.word
                        for (player in sprintParams.players) {
                            if (player.isVirtual) {
                                currentDrawer = player.user.username
                                break
                            }
                        }
                    }))
                }))
                .on(SocketEvent.TURN_ENDED, ({ data ->
                    isTurnStarted = false
                    val turnParams = Gson().fromJson(data.first().toString(), EndTurn::class.java)
                    getNonVirtualPlayers(turnParams.players)
                    Handler(Looper.getMainLooper()).post(({
                        refreshPlayerPointsToolbar()
                        resetTurn(turnParams.drawer)
                        user_block.bringToFront()
                        if (GameManager.currentGameMode == MatchMode.ONE_ON_ONE) {
                            if (nonVirtualPlayers.size > 1) {
                                val player1 = nonVirtualPlayers[0]
                                val player2 = nonVirtualPlayers[1]
                                one_vs_one_title.text = String.format(
                                    resources.getString(R.string.one_vs_one_title),
                                    player1.user.username, player2.user.username
                                )
                                one_vs_one_title.visibility = View.VISIBLE
                            }
                        }
                        if (!firstTurnStarted) {
                            message.text = String.format(resources.getString(R.string.round), turnParams.currentRound.toInt())
                            user_points.visibility = View.GONE
                            Handler().postDelayed({
                                delegateViews(turnParams.choices)
                            }, ROUND_SCREEN_TIMEOUT)
                        } else {
                            val pointsAdapter = GroupAdapter<GroupieViewHolder>()
                            message.text = wordWasString
                            nonVirtualPlayers.sortByDescending(({ it.score.scoreTurn.toInt() }))
                            for (score in nonVirtualPlayers) {
                                pointsAdapter.add(PlayerPointHolder(score.user.username, score.score.scoreTurn.toInt()))
                            }
                            user_points.adapter = pointsAdapter
                            user_points.visibility = View.VISIBLE
                            Handler().postDelayed({
                                if (turnParams.currentRound.toInt() != currentRound) {
                                    user_points.visibility = View.GONE
                                    message.text = String.format(resources.getString(R.string.round), turnParams.currentRound.toInt())
                                    Handler().postDelayed({
                                        delegateViews(turnParams.choices)
                                    }, ROUND_SCREEN_TIMEOUT)
                                } else {
                                    delegateViews(turnParams.choices)
                                }
                            }, POINT_SCREEN_TIMEOUT)
                            currentRound = turnParams.currentRound.toInt()
                        }
                    }))
                }))
                .on(SocketEvent.TURN_STARTED, ({ data ->
                    Handler(Looper.getMainLooper()).post(Runnable {
                        isTurnStarted = true
                        firstTurnStarted = true
                        draw_view_container.bringToFront()
                        val turnStart = Gson().fromJson(data.first().toString(), StartTurn::class.java)
                        startCountdown(turnStart.timeLimit.toLong() * SECOND_INTERVAL)
                        if (!isHost) {
                            wordBeingDrawn = turnStart.word.replace("\"", "")
                        }
                        setupWordHolder()
                    })
                }))
                .on(SocketEvent.MATCH_STARTED, ({ data ->
                    val feedback =
                        Gson().fromJson(data.first().toString(), StartMatchFeedback::class.java)
                    if (feedback.feedback.status) {
                        isGameStarted = true
                        Handler(Looper.getMainLooper()).post(Runnable {
                            toolbar.visibility = View.VISIBLE
                            if (GameManager.currentGameMode == MatchMode.SOLO ||
                                GameManager.currentGameMode == MatchMode.COLLABORATIVE) {
                                showObserverFragment()
                            } else {
                                user_block.bringToFront()
                            }
                        })
                    } else {
                        Handler(Looper.getMainLooper()).post(Runnable {
                            Toast.makeText(this, feedback.feedback.log_message, Toast.LENGTH_LONG)
                                .show()
                        })
                    }
                }))
                .on(SocketEvent.MATCH_ENDED, ({ data ->
                    Handler(Looper.getMainLooper()).post(Runnable {
                        turnOffSocketEvents()
                        user_block.bringToFront()
                        user_points.visibility = View.GONE
                        message.text = resources.getText(R.string.game_over)
                        back_to_lobby.visibility = View.VISIBLE
                    })
                }))
                .on(SocketEvent.UNEXPECTED_LEAVE, ({
                    Handler(Looper.getMainLooper()).post(Runnable {
                        turnOffSocketEvents()
                        user_block.bringToFront()
                        user_points.visibility = View.GONE
                        message.text = resources.getText(R.string.unexpected_leave)
                        back_to_lobby.visibility = View.VISIBLE
                    })
                }))
        }
    }

    private fun showUserChoosingWord() {
        message.text = String.format(resources.getString(R.string.user_choosing_word), currentDrawer)
        user_points.visibility = View.GONE
        user_block.bringToFront()
    }

    private fun getNonVirtualPlayers(players: Array<Player>) {
        nonVirtualPlayers.clear()
        for (player in players) {
            if (!player.isVirtual) {
                nonVirtualPlayers.add(player)
            }
        }
    }
}
