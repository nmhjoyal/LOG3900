package com.example.thin_client.ui.game_mode.waitingroom


import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.Toast
import androidx.fragment.app.Fragment
import com.example.thin_client.R
import com.example.thin_client.data.Feedback
import com.example.thin_client.data.game.GameManager
import com.example.thin_client.data.game.Player
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.waitingroom.WaitingRoomItem
import com.google.gson.Gson
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.fragment_waiting_room.*


class WaitingRoom : Fragment() {

    private val adapter = GroupAdapter<GroupieViewHolder>()

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {

        super.onViewCreated(view, savedInstanceState)

        start_match.setOnClickListener((({
            SocketHandler.startMatch()
        })))

        add_vp.setOnClickListener((({
            SocketHandler.addVirtualPlayer()
            adapter.add(WaitingRoomItem("Harry", "pear"))
            GameManager.addVirtualPlayer()
        })))

        remove_vp.setOnClickListener((({
            SocketHandler.removeVirtualPlayer()
            refreshPlayersAdapter()
        })))

        setUpSocketEvents()
        SocketHandler.searchPlayers()
        refreshPlayersAdapter()
        recyclerview_available_players.adapter = adapter

    }

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.fragment_waiting_room, container, false)
    }

    private fun setUpSocketEvents() {
        if (SocketHandler.socket != null) {
            SocketHandler.socket
                ?.on(SocketEvent.UPDATE_PLAYERS, ({ data ->
                    val players = Gson().fromJson(data.first().toString(), Array<Player>::class.java)
                    GameManager.playersList = players.toCollection(ArrayList())
                    }))
                ?.on(SocketEvent.VP_ADDED, ({ data ->
                    val feedback = Gson().fromJson(data.first().toString(), Feedback::class.java)
                    GameManager.addVirtualPlayer()
                    if(!feedback.status){
                        Handler(Looper.getMainLooper()).post(Runnable {
                            Toast.makeText(
                                context,
                                feedback.log_message,
                                Toast.LENGTH_SHORT
                            ).show()
                        })
                    }
                }))
                ?.on(SocketEvent.VP_REMOVED, ({ data ->
                    val feedback = Gson().fromJson(data.first().toString(), Feedback::class.java)
                    GameManager.removeVirtualPlayer()
                    if(!feedback.status){
                        Handler(Looper.getMainLooper()).post(Runnable {
                            Toast.makeText(
                                context,
                                feedback.log_message,
                                Toast.LENGTH_SHORT
                            ).show()
                        })
                    }
                }))
        }

    }

    private fun refreshPlayersAdapter() {
        adapter.clear()
        for (player in GameManager.playersList) {
            adapter.add(WaitingRoomItem(player.user.username, player.user.avatar))
        }
    }


}
