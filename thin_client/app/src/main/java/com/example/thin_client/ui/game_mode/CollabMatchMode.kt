package com.example.thin_client.ui.game_mode

import android.content.Context
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import androidx.fragment.app.FragmentManager
import com.example.thin_client.R
import com.example.thin_client.data.game.GameManager
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.collab_gameslist.*
import kotlinx.android.synthetic.main.game_item.*
import kotlinx.android.synthetic.main.lobby_menu_fragment.*

class CollabMatchMode : Fragment() {

    interface IStartNewFragment {
        fun startWaitingRoom()
    }

    private var startNewFragment: IStartNewFragment? = null

    private val adapter = GroupAdapter<GroupieViewHolder>()
    private lateinit var manager: FragmentManager

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {

        manager = childFragmentManager

        super.onViewCreated(view, savedInstanceState)

        adapter.add(MatchItem("B", "Allllo", 4, 5))

        adapter.setOnItemClickListener(({ _, _->
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
        startNewFragment = context as? IStartNewFragment
        if (startNewFragment == null) {
        }
    }

}