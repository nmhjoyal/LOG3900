package com.example.thin_client.ui.game_mode.waitingroom


import android.content.Context
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup

import com.example.thin_client.R
import com.example.thin_client.data.AvatarID
import com.example.thin_client.data.game.GameManager
import com.example.thin_client.data.game.MatchMode
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.game_mode.Player
import com.example.thin_client.ui.waitingroom.WaitingRoomItem
import com.google.gson.Gson
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.fragment_waiting_room.*


class WaitingRoom : Fragment() {

    private var listener: IStartMatch? = null

    interface IStartMatch{
        fun startMatch()
    }
    private val adapter = GroupAdapter<GroupieViewHolder>()
    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {

        super.onViewCreated(view, savedInstanceState)
        adapter.add(WaitingRoomItem("amar", AvatarID.PEAR))
        start_match.setOnClickListener((({listener?.startMatch()})))
        recyclerview_available_players.adapter = adapter
        setUpSocketEvents()

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

    override fun onAttach(context: Context) {
        super.onAttach(context)
        listener = context as? IStartMatch
        if (listener == null) {
        }
    }

    override fun onDetach() {
        super.onDetach()
        listener = null
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
