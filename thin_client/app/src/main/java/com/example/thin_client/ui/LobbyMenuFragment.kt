package com.example.thin_client.ui

import android.content.Context
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup
import android.widget.LinearLayout

import com.example.thin_client.R

import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.game_mode.MatchList
import kotlinx.android.synthetic.main.lobby_menu_fragment.*

class LobbyMenuFragment : Fragment() {

    interface IStartNewFragment {
        fun startGameList()
        fun startFreeDraw()
        fun startWaitingRoom()
    }

    private var startNewFragment: IStartNewFragment? = null

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        val view:View = inflater.inflate(R.layout.lobby_menu_fragment, container, false)
        view.isFocusableInTouchMode = true
        view.requestFocus()
        val joinMatch = view.findViewById<LinearLayout>(R.id.join_match)

        joinMatch.setOnClickListener(object : View.OnClickListener{
            override fun onClick(v: View?) {
                val transaction = fragmentManager!!.beginTransaction()
                val gamesList = MatchList()
                transaction.replace(R.id.lobby_menu_container, gamesList)
                transaction.addToBackStack(null)
                transaction.commit()
                SocketHandler.searchMatches()
            }})
        return view
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        free_draw.setOnClickListener(({
            startNewFragment?.startFreeDraw()
        }))

        join_match.setOnClickListener(({
            startNewFragment?.startGameList()
        }))
    }

    override fun onAttach(context: Context) {
        super.onAttach(context)
        startNewFragment = context as? IStartNewFragment
        if (startNewFragment == null) {
        }
    }


}
