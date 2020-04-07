package com.example.thin_client.ui.game_mode
import android.view.View
import androidx.core.view.isVisible
import com.example.thin_client.R
import com.example.thin_client.data.model.MatchInfos
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import com.xwray.groupie.Item
import kotlinx.android.synthetic.main.game_item.view.*


class MatchItem(val match: MatchInfos): Item<GroupieViewHolder>(){

    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        viewHolder.itemView.match_id.text = match.matchId
        viewHolder.itemView.text_view_hostname.text = match.host
        viewHolder.itemView.text_view_rounds.text = match.nbRounds.toString()
        viewHolder.itemView.text_view_numberplayers.text = match.players.size.toString()
        val adapter = GroupAdapter<GroupieViewHolder>()
        for (player in match.players) {
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
        return R.layout.game_item
    }
}


