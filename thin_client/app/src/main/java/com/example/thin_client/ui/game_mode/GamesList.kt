package com.example.thin_client.ui.game_mode

import android.content.Context
import android.content.Intent
import android.os.Bundle
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.view.WindowManager
import android.widget.RadioGroup
import androidx.fragment.app.Fragment
import com.example.thin_client.R
import com.example.thin_client.data.game.GameManager
import com.example.thin_client.data.game.GameMode
import kotlinx.android.synthetic.main.activity_games_list.*


class GamesList : Fragment() {

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        viewpager.adapter = MyPagerAdapter(childFragmentManager)
        create_match.setOnClickListener {
            context?.let {context ->
                showCreateMatchDialog(context)
            }
        }
    }


    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {

        return inflater.inflate(R.layout.activity_games_list, container, false)

    }

    private fun showCreateMatchDialog(context: Context) {
        val alertBuilder = android.app.AlertDialog.Builder(context)
        alertBuilder.setTitle(R.string.create_match)
        val dialogView = layoutInflater.inflate(R.layout.dialog_create_match, null)
        alertBuilder.setView(dialogView)
        val gameRadioGroup = dialogView.findViewById<RadioGroup>(R.id.game_mode_selection)
        gameRadioGroup.check(R.id.is_solo_mode)

        alertBuilder
            .setPositiveButton(R.string.start) { _, _ ->
                when(gameRadioGroup.checkedRadioButtonId) {
                    R.id.is_solo_mode -> {
                        GameManager.currentGameMode = GameMode.SOLO
                    }
                    R.id.is_collab_mode -> {
                        GameManager.currentGameMode = GameMode.COLLAB
                    }
                    R.id.is_general_mode -> {
                        GameManager.currentGameMode = GameMode.GENERAL
                    }
                    R.id.is_one_on_one_mode -> {
                        GameManager.currentGameMode = GameMode.ONE_V_ONE
                    }
                    R.id.is_inverse_mode -> {
                        GameManager.currentGameMode = GameMode.REVERSE
                    }
                }
                val intent = Intent(context, GameActivity::class.java)
                startActivity(intent)

            }
            .setNegativeButton(R.string.cancel) { _, _ -> }
        val dialog = alertBuilder.create()
        dialog.window!!.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE)
        dialog.show()
    }

}
