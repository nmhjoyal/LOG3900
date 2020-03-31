package com.example.thin_client.ui.game_mode
import android.view.View
import com.example.thin_client.R
import com.xwray.groupie.GroupieViewHolder
import com.xwray.groupie.Item
import kotlinx.android.synthetic.main.game_item.view.*


class MatchItem(val id: String, val hostname: String,
                val rounds : Number,
                val numberplayers: Number): Item<GroupieViewHolder>(){

    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        viewHolder.itemView.match_id.text = id
        viewHolder.itemView.text_view_hostname.text = hostname
        viewHolder.itemView.text_view_rounds.text = rounds.toString()
        viewHolder.itemView.text_view_numberplayers.text = numberplayers.toString()

    }

    override fun getLayout(): Int {
        return R.layout.game_item
    }


}


