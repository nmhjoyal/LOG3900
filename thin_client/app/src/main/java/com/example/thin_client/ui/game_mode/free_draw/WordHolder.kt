package com.example.thin_client.ui.game_mode.free_draw

import com.example.thin_client.R
import com.xwray.groupie.GroupieViewHolder
import com.xwray.groupie.Item
import kotlinx.android.synthetic.main.word_choice.view.*

class WordHolder(val text: String): Item<GroupieViewHolder>(){

    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        viewHolder.itemView.choice.text = text
    }

    override fun getLayout(): Int {
        return R.layout.word_choice
    }
}