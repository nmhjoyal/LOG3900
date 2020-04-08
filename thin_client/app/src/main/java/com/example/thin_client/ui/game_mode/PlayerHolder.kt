package com.example.thin_client.ui.game_mode

import com.example.thin_client.R
import com.example.thin_client.data.getAvatar
import com.example.thin_client.data.model.PublicProfile
import com.example.thin_client.data.setAvatar
import com.xwray.groupie.GroupieViewHolder
import com.xwray.groupie.Item
import kotlinx.android.synthetic.main.player_holder.view.*


class PlayerHolder(private val user: PublicProfile): Item<GroupieViewHolder>(){

    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        viewHolder.itemView.username.text = user.username
        val avatarId = getAvatar(user.avatar)
        setAvatar(viewHolder.itemView.avatar, avatarId)
    }

    override fun getLayout(): Int {
        return R.layout.player_holder
    }
}