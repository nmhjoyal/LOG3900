package com.example.thin_client.ui.game_mode

import android.content.Context
import android.graphics.Color
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.util.Log
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.view.WindowManager
import android.widget.*
import androidx.core.content.ContextCompat
import androidx.fragment.app.Fragment
import com.example.thin_client.R
import com.example.thin_client.data.Feedback
import com.example.thin_client.data.game.CreateMatch
import com.example.thin_client.data.game.CreateMatchFeedback
import com.example.thin_client.data.game.GameManager
import com.example.thin_client.data.game.GameManager.tabNames
import com.example.thin_client.data.game.MatchMode
import com.example.thin_client.data.model.MatchInfos
import com.example.thin_client.data.rooms.RoomManager
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.google.android.material.tabs.TabLayout
import com.google.gson.Gson
import kotlinx.android.synthetic.main.activity_games_list.*
import kotlinx.android.synthetic.main.chatrooms_fragment.*
import java.util.*


class MatchList : Fragment() {

    interface IGameStarter {
        fun startGame()
    }

    var gameStartedListener: IGameStarter? = null

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        viewpager.adapter = MyPagerAdapter(childFragmentManager)
        create_match.setOnClickListener {
            context?.let {context ->
                showCreateMatchDialog(context)
            }
        }
        setupTabs()
        tabLayout.addOnTabSelectedListener(object : TabLayout.OnTabSelectedListener {
            override fun onTabSelected(tab: TabLayout.Tab?) {
                val tabName =
                    tab!!.customView!!.findViewById(R.id.tab_name) as TextView
                tabName.setTextColor(Color.WHITE)
                tab.customView!!.setBackgroundResource(R.drawable.tab_background_selected)
            }

            override fun onTabReselected(tab: TabLayout.Tab?) {
            }

            override fun onTabUnselected(tab: TabLayout.Tab?) {
                val tabName =
                    tab!!.customView!!.findViewById(R.id.tab_name) as TextView
                tabName.setTextColor(ContextCompat.getColor(context!!, R.color.colorPrimaryDark))
                tab.customView!!.setBackgroundResource(R.drawable.tab_background)
            }
        })

        setupSocketEvents()
    }

    override fun onAttach(context: Context) {
        super.onAttach(context)
        gameStartedListener = context as? IGameStarter
        if (gameStartedListener == null) {
            Toast.makeText(context, "Cannot start a new game at this time.", Toast.LENGTH_LONG).show()
        }
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {

        return inflater.inflate(R.layout.activity_games_list, container, false)

    }

   private fun setupTabs() {
        for (i in 0 until tabLayout.tabCount) {
            val customView = View.inflate(context,R.layout.tab_layout, null)
            val tabName = customView.findViewById(R.id.tab_name) as TextView
            tabName.text = tabNames[i]
            if (i == 0) {
                tabName.setTextColor(Color.WHITE)
                customView.setBackgroundResource(R.drawable.tab_background_selected)
            } else {
                customView.setBackgroundResource(R.drawable.tab_background)
            }
            tabLayout.getTabAt(i)!!.customView = customView
            tabLayout.getTabAt(i)!!.view.setPadding(0, 0, 0, -16)
        }
    }

    private fun showCreateMatchDialog(context: Context) {
        val alertBuilder = android.app.AlertDialog.Builder(context)
        alertBuilder.setTitle(R.string.create_match)
        val dialogView = layoutInflater.inflate(R.layout.dialog_create_match, null)
        alertBuilder.setView(dialogView)
        val gameSelectionSpinner = dialogView.findViewById<Spinner>(R.id.game_mode_selection)
        val nbRoundsSpinner = dialogView.findViewById<Spinner>(R.id.nb_rounds)
        val timeLimitSpinner = dialogView.findViewById<Spinner>(R.id.time_limit)
        ArrayAdapter.createFromResource(context, R.array.nb_rounds_array, R.layout.spinner_selected)
            .also { adapter ->
                adapter.setDropDownViewResource(R.layout.spinner_item)
                nbRoundsSpinner.adapter = adapter
            }

        ArrayAdapter.createFromResource(context, R.array.time_limit_array, R.layout.spinner_selected)
            .also { adapter ->
                adapter.setDropDownViewResource(R.layout.spinner_item)
                timeLimitSpinner.adapter = adapter
            }
        ArrayAdapter.createFromResource(context, R.array.game_modes, R.layout.spinner_selected)
            .also { adapter ->
                adapter.setDropDownViewResource(R.layout.spinner_item)
                gameSelectionSpinner.adapter = adapter
            }

        alertBuilder
            .setPositiveButton(R.string.start) { _, _ ->
                val stringMatchMode = gameSelectionSpinner.selectedItem.toString().replace("-", "_").toUpperCase(Locale.CANADA)
                GameManager.currentGameMode = MatchMode.valueOf(stringMatchMode)
                GameManager.nbRounds = nbRoundsSpinner.selectedItem.toString().toInt()
                GameManager.timeLimit = timeLimitSpinner.selectedItem.toString().toInt()
                SocketHandler.createMatch(CreateMatch(GameManager.nbRounds, GameManager.timeLimit,
                    GameManager.currentGameMode.ordinal))
            }
            .setNegativeButton(R.string.cancel) { _, _ -> }
        val dialog = alertBuilder.create()
        dialog.window!!.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE)
        dialog.show()
    }

    private fun setupSocketEvents() {
        SocketHandler.socket!!
            .on(SocketEvent.MATCH_CREATED, ({ data ->
                val feedback = Gson().fromJson(data.first().toString(), CreateMatchFeedback::class.java)
                if (feedback.feedback.status) {
                    RoomManager.currentRoom = feedback.matchId
                    gameStartedListener?.startGame()
                } else {
                    Handler(Looper.getMainLooper()).post(({
                        Toast.makeText(context, feedback.feedback.log_message, Toast.LENGTH_LONG).show()
                    }))
                }
            }))
    }
}
