package com.example.thin_client.ui.game_mode

import android.content.Context
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import com.example.thin_client.R
import com.example.thin_client.data.game.GameManager
import com.example.thin_client.data.game.MatchMode
import com.example.thin_client.data.model.MatchInfos
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.ui.LobbyMenuFragment
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import com.example.thin_client.server.*
import com.google.gson.Gson
import kotlinx.android.synthetic.main.freeforall_gameslist.*

class FreeForAllMatchMode : Fragment() {

    private val adapter = GroupAdapter<GroupieViewHolder>()
    private var startNewFragment: LobbyMenuFragment.IStartNewFragment? = null

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        adapter.setOnItemClickListener{ item, _ ->
            val matchId = (item as MatchItem).match.matchId
            GameManager.currentGameMode = MatchMode.FREE_FOR_ALL
            SocketHandler.joinMatch(matchId)
        }
        refreshMatchesAdapter()
        availablefree_for_all.adapter = adapter
    }


    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        return inflater.inflate(R.layout.freeforall_gameslist, container, false)

    }


    override fun onStart() {
        super.onStart()
        refreshMatchesAdapter()
    }

    private fun refreshMatchesAdapter() {
        adapter.clear()
        for (match in GameManager.freeForAllMatchList) {
            adapter.add(MatchItem(match))
        }
    }

    override fun onAttach(context: Context) {
        super.onAttach(context)
        startNewFragment = context as? LobbyMenuFragment.IStartNewFragment
        if (startNewFragment == null) {
        }
    }
}