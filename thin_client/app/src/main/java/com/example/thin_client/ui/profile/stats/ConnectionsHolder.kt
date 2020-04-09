package com.example.thin_client.ui.profile.stats

import com.example.thin_client.R
import com.xwray.groupie.GroupieViewHolder
import com.xwray.groupie.Item
import kotlinx.android.synthetic.main.connections_layout.view.*
import java.text.SimpleDateFormat
import java.util.*


const val stringDate = "MM/dd/yyyy HH:mm:ss"
val simpleDateFormat = SimpleDateFormat(stringDate, Locale.US)

class ConnectionsHolder(private val connection: Long, private val disconnection: Long): Item<GroupieViewHolder>(){

    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        viewHolder.itemView.connection.text = simpleDateFormat.format(Date(connection))
        viewHolder.itemView.disconnection.text = simpleDateFormat.format(Date(disconnection))
    }

    override fun getLayout(): Int {
        return R.layout.connections_layout
    }
}