package com.example.thin_client.ui

import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import com.example.thin_client.R
import com.example.thin_client.ui.chat.ChatActivity
import com.example.thin_client.ui.game_mode.free_draw.FreeDrawActivity
import kotlinx.android.synthetic.main.activity_lobby.*

class Lobby : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_lobby)

        free_draw.setOnClickListener(({
            val intent = Intent(applicationContext, FreeDrawActivity::class.java)
            startActivity(intent)
        }))
    }
}
