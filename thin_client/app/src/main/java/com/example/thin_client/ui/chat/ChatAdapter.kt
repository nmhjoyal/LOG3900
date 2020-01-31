package com.example.thin_client.ui.chat

import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.EditText
import android.widget.TextView
import androidx.recyclerview.widget.RecyclerView
import com.example.thin_client.R
import com.example.thin_client.data.Message
import com.example.thin_client.data.model.LoggedInUser

class ChatAdapter(val messageList: ArrayList<Message>): RecyclerView.Adapter<ChatAdapter.ViewHolder>(){

    override fun onCreateViewHolder(parent: ViewGroup, viewType: Int): ViewHolder {
        val v = LayoutInflater.from(parent?.context).inflate(R.layout.chat_items, parent, false)
        return ViewHolder(v)
    }

    override fun getItemCount(): Int {
       return messageList.size
    }

    override fun onBindViewHolder(holder: ViewHolder, position: Int) {
        val message: Message = messageList[position]
        holder?.textViewUsername?.text = message.username
        holder?.textViewMessage?.text = message.text
        holder?.textViewDate?.text = message.date
    }

    class ViewHolder(itemView: View) : RecyclerView.ViewHolder(itemView){
        val textViewUsername = itemView.findViewById(R.id.textViewUsername) as TextView
        val textViewMessage = itemView.findViewById(R.id.textViewMessage) as TextView
        val textViewDate = itemView.findViewById(R.id.textViewDate) as TextView
    }
}