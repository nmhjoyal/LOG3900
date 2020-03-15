package com.example.thin_client.ui.chatrooms

import com.example.thin_client.R
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.xwray.groupie.GroupieViewHolder
import com.xwray.groupie.Item
import kotlinx.android.synthetic.main.invite_row.view.*
import kotlinx.android.synthetic.main.user_invite_row.view.*

class InviteInboxRow(val roomId: String): Item<GroupieViewHolder>(){

    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        viewHolder.itemView.invite_room_name.text = roomId

        viewHolder.itemView.accept_join.setOnClickListener(({
            SocketHandler.joinChatRoom(roomId)
        }))

        viewHolder.itemView.deny_join.setOnClickListener(({

        }))

    }

    override fun getLayout(): Int {
        return R.layout.invite_row
    }
}