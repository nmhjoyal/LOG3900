package com.example.thin_client.ui.game_mode.waitingroom


import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup

import com.example.thin_client.R
import com.example.thin_client.data.AvatarID
import com.example.thin_client.data.game.GameManager
import com.example.thin_client.data.game.MatchMode
import com.example.thin_client.data.game.Player
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.helpers.DebounceClickListener
import com.example.thin_client.ui.waitingroom.WaitingRoomItem
import com.google.gson.Gson
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.fragment_waiting_room.*


class WaitingRoom : Fragment() {

    private val adapter = GroupAdapter<GroupieViewHolder>()

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {

        super.onViewCreated(view, savedInstanceState)
        adapter.add(WaitingRoomItem("amar", AvatarID.PEAR))

        recyclerview_available_players.adapter = adapter
        setUpSocketEvents()

        start_match.setOnClickListener((({
            SocketHandler.startMatch()
        })))

    }

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        when(GameManager.currentGameMode){
            MatchMode.SOLO -> {}
        }

        return inflater.inflate(R.layout.fragment_waiting_room, container, false)
    }

    private fun setUpSocketEvents(){
        if(SocketHandler.socket != null){
            SocketHandler.socket?.on(SocketEvent.UPDATE_PLAYERS, ({ data ->
                val players = Gson().fromJson(data.first().toString(),Array<Player>::class.java)
                GameManager.playersList = players.toCollection(ArrayList())
            }))
        }

    }


}
