package com.example.thin_client.ui.game_mode

import android.content.Context
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import com.example.thin_client.R
import com.example.thin_client.data.game.GameManager
import com.example.thin_client.ui.LobbyMenuFragment
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.collab_gameslist.*
import java.util.logging.SocketHandler

class CollabMatchMode : Fragment() {


    private var startNewFragment: LobbyMenuFragment.IStartNewFragment? = null

    private val adapter = GroupAdapter<GroupieViewHolder>()

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        adapter.add(MatchItem("A","Allllo", 4 ,5))
        //faire la verification du matchID
        adapter.setOnItemClickListener(({ _, _ ->
            startNewFragment?.startWaitingRoom()
        }))

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
            adapter.add(MatchItem(match.matchId, match.host, match.nbRounds, match.players.size))
        }
    }

    override fun onAttach(context: Context) {
        super.onAttach(context)
        startNewFragment = context as? LobbyMenuFragment.IStartNewFragment
        if (startNewFragment == null) {
        }
    }

}