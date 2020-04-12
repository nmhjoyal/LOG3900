package com.example.thin_client.ui.leaderboard

import android.content.Context
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import com.example.thin_client.R
import com.example.thin_client.data.app_preferences.PreferenceHandler
import com.example.thin_client.data.model.RankClient

import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.collab_ranking_list.*
import kotlinx.android.synthetic.main.collab_ranking_list.linearLayout
import kotlinx.android.synthetic.main.onevsone_ranking_list.*

class RankingCollabMode : Fragment() {


    private val adapter = GroupAdapter<GroupieViewHolder>()

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        for (ranking in LeaderboardManager.collabRankingList) {
            if(ranking.username == PreferenceHandler(context!!).getUser().username){
                linearLayout.visibility= View.VISIBLE
                text_view_collab_current_username.text = LeaderboardManager.collabCurrentPlayer.username
                collab_current_user_position.text = LeaderboardManager.collabCurrentPlayer.pos.toString()
                text_view_collab_ranking_score.text = LeaderboardManager.collabCurrentPlayer.score.toString()
            }
        }
        refreshRankingAdapter()
        collab_rankinglist.adapter = adapter

    }


    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        return inflater.inflate(R.layout.collab_ranking_list, container, false)

    }

    override fun onStart() {
        super.onStart()
        refreshRankingAdapter()
    }

    private fun refreshRankingAdapter() {
      adapter.clear()
       for (ranking in LeaderboardManager.collabRankingList) {
               adapter.add(LeaderboardItem(ranking))
        }
    }

    override fun onAttach(context: Context) {
        super.onAttach(context)

    }

}