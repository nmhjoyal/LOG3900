package com.example.thin_client.ui.waitingroom
import android.view.View
import androidx.core.graphics.drawable.toDrawable
import com.example.thin_client.R
import com.example.thin_client.data.AvatarID
import com.xwray.groupie.GroupieViewHolder
import com.xwray.groupie.Item
import kotlinx.android.synthetic.main.game_item.view.*
import kotlinx.android.synthetic.main.player_in_game_item.view.*


class WaitingRoomItem(val username: String,
                      val avatarID: AvatarID
): Item<GroupieViewHolder>(){

    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        viewHolder.itemView.text_view_username.text = username
        viewHolder.itemView.imageview_avatar.setBackgroundResource(R.drawable.avatar_background)

        when (avatarID) {
            AvatarID.PEAR -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_pear)
            AvatarID.CHERRY -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_cherry)
            AvatarID.LEMON -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_lemon)
            AvatarID.APPLE -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_apple)
            AvatarID.PINEAPPLE -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_pineapple)
            AvatarID.ORANGE -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_orange)
            AvatarID.KIWI -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_kiwi)
            AvatarID.GRAPE -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_grape)
            AvatarID.WATERMELON -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_watermelon)
            AvatarID.STRAWBERRY -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_strawberry)
            AvatarID.BANANA -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_banana)
            AvatarID.AVOCADO -> viewHolder.itemView.imageview_avatar.setImageResource(R.drawable.ic_avocado)
        }

    }

    override fun getLayout(): Int {
        return R.layout.player_in_game_item
    }


}

