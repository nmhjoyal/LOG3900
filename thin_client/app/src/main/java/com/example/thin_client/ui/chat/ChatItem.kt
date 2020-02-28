package com.example.thin_client.ui.chat

import android.content.res.Resources
import android.provider.Settings.Global.getString
import androidx.core.content.ContextCompat
import com.example.thin_client.R
import com.example.thin_client.server.SocketHandler

import com.xwray.groupie.Item
import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.chat_from_row.view.*
import kotlinx.android.synthetic.main.chat_to_row.view.*
import kotlinx.android.synthetic.main.chat_user_joinedorleft.view.*
import java.text.SimpleDateFormat
import java.util.*
import kotlin.coroutines.coroutineContext

const val stringDate = "[HH:mm:ss]"
val simpleDateFormat = SimpleDateFormat(stringDate, Locale.US)

class ChatToItem(val text: String, val date: Long): Item<GroupieViewHolder>(){

    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        viewHolder.itemView.text_view_to_message.text = text
        viewHolder.itemView.text_view_username_to.text =  SocketHandler.user!!.username
        viewHolder.itemView.timestamp_to.text =  simpleDateFormat.format(Date(date * 1000))
    }

    override fun getLayout(): Int {
        return R.layout.chat_to_row
    }
}

class ChatFromItem(val text:String, val author:String, val date: Long): Item<GroupieViewHolder>(){
    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        viewHolder.itemView.text_view_from_message.text = text
        viewHolder.itemView.text_view_username_from.text = author
        viewHolder.itemView.timestamp_from.text =  simpleDateFormat.format(Date(date * 1000))
    }

    override fun getLayout(): Int {
        return R.layout.chat_from_row
    }
}

class ChatUserJoined(val author: String, val hasJoined: Boolean): Item<GroupieViewHolder>(){
    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        if (hasJoined) {
            viewHolder.itemView.text_joined_or_left.text = Resources.getSystem().getString(R.string.user_joined, author)
        } else {
            viewHolder.itemView.text_joined_or_left.text = Resources.getSystem().getString(R.string.user_left, author)
        }
    }

    override fun getLayout(): Int {
        return R.layout.chat_from_row
    }
}