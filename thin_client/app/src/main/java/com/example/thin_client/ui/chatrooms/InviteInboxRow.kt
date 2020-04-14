package com.example.thin_client.ui.chatrooms

import android.view.View
import com.example.thin_client.R
import com.example.thin_client.data.rooms.RoomManager
import com.example.thin_client.server.SocketHandler
import com.xwray.groupie.GroupieViewHolder
import com.xwray.groupie.Item
import kotlinx.android.synthetic.main.invite_row.view.*

class InviteInboxRow(val roomId: String): Item<GroupieViewHolder>(){

    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        viewHolder.itemView.invite_room_name.text = roomId

        viewHolder.itemView.accept_join.setOnClickListener(({
            RoomManager.invites.remove(roomId)
            SocketHandler.joinChatRoom(roomId)
            viewHolder.itemView.setBackgroundResource(R.drawable.background_joined)
            viewHolder.itemView.invite_room_name.text = "JOINED"
            viewHolder.itemView.accept_join.visibility = View.GONE
            viewHolder.itemView.deny_join.visibility = View.GONE
        }))

        viewHolder.itemView.deny_join.setOnClickListener(({
            RoomManager.invites.remove(roomId)
            viewHolder.itemView.visibility = View.GONE
        }))

    }

    override fun getLayout(): Int {
        return R.layout.invite_row
    }
}