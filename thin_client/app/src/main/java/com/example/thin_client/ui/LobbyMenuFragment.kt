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




    }


}
