package com.example.thin_client.ui.leaderboard

import android.content.Context
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import com.example.thin_client.R
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.collab_gameslist.*
import kotlinx.android.synthetic.main.collab_ranking_list.*
import kotlinx.android.synthetic.main.solo_ranking_list.*

class RankingSoloMode : Fragment() {


    private val adapter = GroupAdapter<GroupieViewHolder>()

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        refreshRankingAdapter()
        solo_rankinglist.adapter = adapter
    }


    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        return inflater.inflate(R.layout.solo_ranking_list, container, false)

    }

    override fun onStart() {
        super.onStart()
        refreshRankingAdapter()
    }

    private fun refreshRankingAdapter() {
      adapter.clear()
       for (ranking in LeaderboardManager.soloRankingList) {
           // adapter.add(LeaderboardItem("amar",20))
        }
    }

    override fun onAttach(context: Context) {
        super.onAttach(context)

    }

}