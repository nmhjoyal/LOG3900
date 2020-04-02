package com.example.thin_client.ui.waitingroom

import com.example.thin_client.R
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

        when (avatar) {
            "pear" -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_pear)
            "cherry" -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_cherry)
            "lemon" -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_lemon)
            "apple" -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_apple)
            "pineapple" -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_pineapple)
            "orange" -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_orange)
            "kiwi" -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_kiwi)
            "grape" -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_grape)
            "watermelon" -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_watermelon)
            "strawberry" -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_strawberry)
            "banana" -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_banana)
            "avocado" -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_avocado)
        }

    }

    override fun getLayout(): Int {
        return R.layout.player_in_game_item
    }


}

