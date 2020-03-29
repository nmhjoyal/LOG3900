package com.example.thin_client.ui.game_mode

import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import com.example.thin_client.R
import com.example.thin_client.data.game.GameManager
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.freeforall_gameslist.*

class FreeForAllMatchMode : Fragment() {

    private val adapter = GroupAdapter<GroupieViewHolder>()

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        adapter.add(MatchItem("Allllo", 4 ,5))
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
        var freeForAllMatchList = GameManager.freeForAllMatchList
        for (match in freeForAllMatchList) {
            adapter.add(MatchItem(match.host, match.nbRounds, match.players.size))
        }
    }
}