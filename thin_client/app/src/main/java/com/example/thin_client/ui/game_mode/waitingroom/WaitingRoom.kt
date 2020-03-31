package com.example.thin_client.ui.game_mode.waitingroom





import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup

import com.example.thin_client.R
import com.example.thin_client.data.AvatarID
import com.example.thin_client.ui.game_mode.MatchItem
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.collab_gameslist.*
import kotlinx.android.synthetic.main.fragment_waiting_room.*


class WaitingRoom : Fragment() {

    private val adapter = GroupAdapter<GroupieViewHolder>()
    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        recyclerview_available_players.adapter = adapter

    }

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {

        return inflater.inflate(R.layout.fragment_waiting_room, container, false)
    }



}
