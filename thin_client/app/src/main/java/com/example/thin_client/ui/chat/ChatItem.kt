package com.example.thin_client.ui.chat

import com.example.thin_client.R
import com.example.thin_client.data.Message

import com.xwray.groupie.Item
import com.xwray.groupie.ViewHolder
import kotlinx.android.synthetic.main.chat_from_row.view.*
import kotlinx.android.synthetic.main.chat_to_row.view.*

class ChatToItem(val text:String): Item<ViewHolder>(){
    override fun bind(viewHolder: ViewHolder, position: Int) {
        viewHolder.itemView.text_view_to_message.text = "To message......"
    }

    override fun getLayout(): Int {
        return R.layout.chat_to_row
    }
}


class ChatFromItem(val text:String): Item<ViewHolder>(){
    override fun bind(viewHolder: ViewHolder, position: Int) {
       viewHolder.itemView.text_view_from_message.text = "From Message....."
    }

    override fun getLayout(): Int {
        return R.layout.chat_from_row
    }
}