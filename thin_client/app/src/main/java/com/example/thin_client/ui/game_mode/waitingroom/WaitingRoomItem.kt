package com.example.thin_client.ui.game_mode.waitingroom

import com.example.thin_client.R
import com.example.thin_client.data.getAvatar
import com.example.thin_client.data.setAvatar
import com.xwray.groupie.GroupieViewHolder
import com.xwray.groupie.Item
import kotlinx.android.synthetic.main.player_in_game_item.view.*


class WaitingRoomItem(
    val username: String,
    val avatar: String
) : Item<GroupieViewHolder>() {

    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        viewHolder.itemView.text_view_username.text = username
        viewHolder.itemView.imageview_avatar.setBackgroundResource(R.drawable.avatar_background)
        val avatarId = getAvatar(avatar)
        setAvatar(viewHolder.itemView.imageview_avatar, avatarId)
    }

    override fun getLayout(): Int {
        return R.layout.player_in_game_item
    }


}

