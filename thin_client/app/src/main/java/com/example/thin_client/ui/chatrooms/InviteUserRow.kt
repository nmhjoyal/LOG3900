package com.example.thin_client.ui.chatrooms

import com.example.thin_client.R
import com.xwray.groupie.GroupieViewHolder
import com.xwray.groupie.Item
import kotlinx.android.synthetic.main.user_invite_row.view.*

class InviteUserRow(val user: String): Item<GroupieViewHolder>(){

    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        viewHolder.itemView.add_user_username.text = user
    }

    override fun getLayout(): Int {
        return R.layout.user_invite_row
    }
}