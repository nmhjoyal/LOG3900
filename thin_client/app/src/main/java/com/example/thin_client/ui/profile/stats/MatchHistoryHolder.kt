package com.example.thin_client.ui.profile.stats
import android.view.View
import androidx.core.view.isVisible
import com.example.thin_client.R
import com.example.thin_client.data.model.MatchHistory
import com.example.thin_client.ui.leaderboard.LeaderboardManager
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import com.xwray.groupie.Item
import kotlinx.android.synthetic.main.match_history_layout.view.*
import java.text.SimpleDateFormat
import java.util.*


const val stringDateMatchHistory = "MM/dd/yyyy HH:mm:ss"
val simpleDateFormatMatchHistory = SimpleDateFormat(stringDateMatchHistory, Locale.US)

class MatchHistoryHolder(private val match: MatchHistory): Item<GroupieViewHolder>(){

    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        viewHolder.itemView.match_mode.text = LeaderboardManager.leaderboardTabNames[match.matchMode.toInt()]
        viewHolder.itemView.score.text = match.myScore.toString()
        viewHolder.itemView.start_time.text = simpleDateFormatMatchHistory.format(Date(match.startTime.toLong()))
        viewHolder.itemView.end_time.text = simpleDateFormatMatchHistory.format(Date(match.endTime.toLong()))
        viewHolder.itemView.winner_user.text = match.winner.username
        viewHolder.itemView.winner_score.text = match.winner.score.toString()

        val adapter = GroupAdapter<GroupieViewHolder>()
        for (player in match.playerNames) {
            adapter.add(PlayerHolder(player))
        }
        viewHolder.itemView.player_list.adapter = adapter

        viewHolder.itemView.show_players.setOnClickListener {
            if (viewHolder.itemView.player_list.isVisible) {
                viewHolder.itemView.player_list.visibility = View.GONE
                viewHolder.itemView.show_players.setImageResource(R.drawable.ic_down)
            } else {
                viewHolder.itemView.player_list.visibility = View.VISIBLE
                viewHolder.itemView.show_players.setImageResource(R.drawable.ic_up)
            }
        }

    }

    override fun getLayout(): Int {
        return R.layout.match_history_layout
    }
}


