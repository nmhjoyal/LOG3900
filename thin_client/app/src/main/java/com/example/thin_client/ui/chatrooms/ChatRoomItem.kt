package com.example.thin_client.ui.chatrooms
import com.example.thin_client.R
import com.xwray.groupie.GroupieViewHolder
import com.xwray.groupie.Item
import kotlinx.android.synthetic.main.chatrooms_row.view.*

class ChatRoomItem(val roomname: String): Item<GroupieViewHolder>(){

    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        viewHolder.itemView.text_view_chatroom_name.text = roomname
    }

    override fun getLayout(): Int {
        return R.layout.chatrooms_row
    }

}
