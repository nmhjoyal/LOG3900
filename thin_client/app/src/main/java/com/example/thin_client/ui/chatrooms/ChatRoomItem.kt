package com.example.thin_client.ui.chatrooms
import com.example.thin_client.R
import com.example.thin_client.data.rooms.RoomManager
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.helpers.DEFAULT_INTERVAL
import com.example.thin_client.ui.helpers.setOnClickListener
import com.xwray.groupie.GroupieViewHolder
import com.xwray.groupie.Item
import kotlinx.android.synthetic.main.room_row.view.*

class ChatRoomItem(val roomname: String): Item<GroupieViewHolder>(){

    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        viewHolder.itemView.room_name.text = roomname

        viewHolder.itemView.leave_button.setOnClickListener(DEFAULT_INTERVAL) {
            RoomManager.roomToRemove = roomname
            SocketHandler.leaveChatRoom(roomname)
            SocketHandler.deleteChatRoom(roomname)
        }
    }

    override fun getLayout(): Int {
        return R.layout.room_row
    }

}
