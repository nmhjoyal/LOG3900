package com.example.thin_client.ui.game_mode

import com.example.thin_client.R
import com.xwray.groupie.GroupieViewHolder
import com.xwray.groupie.Item
import kotlinx.android.synthetic.main.player_point_layout.view.*

class PlayerPointHolder(val user: String, val points: Int): Item<GroupieViewHolder>(){

    private val RED: Long = 4294922834

    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        viewHolder.itemView.username.text = user
        viewHolder.itemView.points.text = points.toString()
        if (points == 0) {
            viewHolder.itemView.points.text = points.toString()
            viewHolder.itemView.points.setTextColor(RED.toInt())
        } else {
            viewHolder.itemView.points.text = "+" + points.toString()
        }
    }

    override fun getLayout(): Int {
        return R.layout.player_point_layout
    }
}