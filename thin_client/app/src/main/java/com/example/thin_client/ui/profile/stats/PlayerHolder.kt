package com.example.thin_client.ui.profile.stats

import android.view.View
import com.example.thin_client.R
import com.xwray.groupie.GroupieViewHolder
import com.xwray.groupie.Item
import kotlinx.android.synthetic.main.player_holder.view.*


class PlayerHolder(private val user: String): Item<GroupieViewHolder>(){

    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        viewHolder.itemView.username.text = user
        viewHolder.itemView.avatar.visibility = View.GONE
    }

    override fun getLayout(): Int {
        return R.layout.player_holder
    }
}