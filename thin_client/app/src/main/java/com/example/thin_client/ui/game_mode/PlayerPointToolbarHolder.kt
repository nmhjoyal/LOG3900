package com.example.thin_client.ui.game_mode

import com.example.thin_client.R
import com.example.thin_client.data.getAvatar
import com.example.thin_client.data.model.PublicProfile
import com.example.thin_client.data.setAvatar
import com.xwray.groupie.GroupieViewHolder
import com.xwray.groupie.Item
import kotlinx.android.synthetic.main.player_in_game_item.view.*
import kotlinx.android.synthetic.main.player_point_layout.view.*

class PlayerPointToolbarHolder(val user: PublicProfile, val totalPoints: Int): Item<GroupieViewHolder>(){


    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        viewHolder.itemView.username.text = user.username
        viewHolder.itemView.points.text = totalPoints.toString()
        val avatarId = getAvatar(user.avatar)
        setAvatar(viewHolder.itemView.imageview_avatar, avatarId)
    }

    override fun getLayout(): Int {
        return R.layout.player_point_toolbar_layout
    }
}