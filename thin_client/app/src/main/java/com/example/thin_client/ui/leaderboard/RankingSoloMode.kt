package com.example.thin_client.ui.leaderboard

import android.content.Context
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import androidx.fragment.app.Fragment
import com.example.thin_client.R
import com.example.thin_client.data.app_preferences.PreferenceHandler
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.collab_ranking_list.*
import kotlinx.android.synthetic.main.solo_ranking_list.*
import kotlinx.android.synthetic.main.solo_ranking_list.linearLayout

class RankingSoloMode : Fragment() {


    private val adapter = GroupAdapter<GroupieViewHolder>()

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        for (ranking in LeaderboardManager.soloRankingList) {
            if(ranking.username == PreferenceHandler(context!!).getUser().username){
                linearLayout.visibility= View.VISIBLE
                text_view_solo_current_username.text = LeaderboardManager.soloCurrentPlayer.username
                solo_current_user_position.text = LeaderboardManager.soloCurrentPlayer.pos.toString()
                text_view_solo_ranking_score.text = LeaderboardManager.soloCurrentPlayer.score.toString()
            }
        }
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
            if(ranking.username == PreferenceHandler(context!!).getUser().username){
                linearLayout.visibility= View.VISIBLE
                text_view_solo_current_username.text = ranking.username
                solo_current_user_position.text = ranking.pos.toString()
                text_view_solo_ranking_score.text = ranking.score.toString()
            }
                adapter.add(LeaderboardItem(ranking))
        }
    }

    override fun onAttach(context: Context) {
        super.onAttach(context)

    }

}