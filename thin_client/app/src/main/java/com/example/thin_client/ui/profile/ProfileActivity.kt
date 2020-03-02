package com.example.thin_client.ui.profile

import android.os.Bundle
import android.view.View
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.isVisible
import com.example.thin_client.R
import kotlinx.android.synthetic.main.activity_profile.*

class ProfileActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_profile)

        user_stats_button.setOnClickListener(({
            if (user_stats_table.isVisible) {
                user_stats_table.visibility = View.GONE
            } else {
                user_stats_table.visibility = View.VISIBLE
            }
        }))

        game_stats_button.setOnClickListener(({
            if (game_stats_table.isVisible) {
                game_stats_table.visibility = View.GONE
            } else {
                game_stats_table.visibility = View.VISIBLE
            }
        }))
    }

    override fun onBackPressed() {
        // Disable native back
    }
}