package com.example.thin_client.ui

import android.content.Context
import android.content.Intent
import android.net.Uri
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.view.WindowManager
import android.widget.Button
import android.widget.EditText
import android.widget.RadioGroup
import androidx.appcompat.widget.ContentFrameLayout
import androidx.fragment.app.FragmentManager
import androidx.fragment.app.ListFragment

import com.example.thin_client.R
import com.example.thin_client.data.game.GameManager
import com.example.thin_client.data.game.GameMode
import com.example.thin_client.ui.chatrooms.ChatRoomsFragment
import com.example.thin_client.ui.game_mode.GameActivity
import com.example.thin_client.ui.game_mode.GamesList
import com.example.thin_client.ui.game_mode.free_draw.FreeDrawActivity
import kotlinx.android.synthetic.main.lobby_menu_fragment.*

class LobbyMenuFragment : Fragment() {

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {

        val view:View = inflater.inflate(R.layout.lobby_menu_fragment, container, false)
        view.isFocusableInTouchMode = true
        view.requestFocus()
        val joinMatch = view.findViewById<Button>(R.id.join_match)

        joinMatch.setOnClickListener(object : View.OnClickListener{
            override fun onClick(v: View?) {
                val transaction = fragmentManager!!.beginTransaction()
                val gamesList = GamesList()
                transaction.replace(R.id.lobby_menu_container, gamesList)
                transaction.addToBackStack(null)
                transaction.commit()
            }})
        return view
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        free_draw.setOnClickListener {
            context?.let {context ->
                val intent = Intent(context, FreeDrawActivity::class.java)
                startActivity(intent)
            }
        }

       create_match.setOnClickListener {
            context?.let {context ->
                showCreateMatchDialog(context)
            }
        }


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
