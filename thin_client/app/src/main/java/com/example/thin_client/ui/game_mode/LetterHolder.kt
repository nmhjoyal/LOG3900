package com.example.thin_client.ui.game_mode

import android.graphics.Color
import com.example.thin_client.R
import com.xwray.groupie.GroupieViewHolder
import com.xwray.groupie.Item
import kotlinx.android.synthetic.main.word_letter.view.*

class LetterHolder(val letter: String, val isVisible: Boolean): Item<GroupieViewHolder>(){

    private val ORANGE: Long = 4294944000

    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        if (letter.isBlank()) {
            viewHolder.itemView.letter_underline.setBackgroundColor(Color.WHITE)
        }
        viewHolder.itemView.letter.text = letter
        if (isVisible) {
            viewHolder.itemView.letter.setTextColor(ORANGE.toInt())
        }
    }

    override fun getLayout(): Int {
        return R.layout.word_letter
    }

}