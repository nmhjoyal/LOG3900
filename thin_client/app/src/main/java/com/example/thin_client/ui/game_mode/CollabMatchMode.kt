package com.example.thin_client.ui.game_mode

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import com.example.thin_client.R
import com.example.thin_client.data.game.GameManager
import com.example.thin_client.data.model.MatchInfos
import com.example.thin_client.data.rooms.RoomManager
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.chatrooms.ChatRoomItem
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.chatrooms_fragment.*
import kotlinx.android.synthetic.main.collab_gameslist.*
import kotlinx.android.synthetic.main.onevsone_gameslist.*

class CollabMatchMode: Fragment() {

    private val adapter = GroupAdapter<GroupieViewHolder>()

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        adapter.add(MatchItem("Allllo", 4 ,5))
        available_collab.adapter = adapter
    }


    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        return inflater.inflate(R.layout.collab_gameslist, container, false)

    }


    override fun onStart() {
        super.onStart()
        refreshMatchesAdapter()
    }

    private fun refreshMatchesAdapter() {
        var collabMatchList = GameManager.collabModeMatchList
        for (match in collabMatchList) {
            adapter.add(MatchItem(match.host,match.nbRounds, match.players.size))
        }
    }
}