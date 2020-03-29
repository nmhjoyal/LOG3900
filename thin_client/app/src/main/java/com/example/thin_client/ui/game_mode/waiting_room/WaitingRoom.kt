package com.example.thin_client.ui.game_mode.waiting_room

import android.content.Context
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup

import com.example.thin_client.R
import com.example.thin_client.data.game.GameManager
import com.example.thin_client.data.game.MatchMode
import com.example.thin_client.data.game.Player
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.google.gson.Gson
import kotlinx.android.synthetic.main.waiting_room_fragment.*


class WaitingRoom : Fragment() {

    private var listener: IStartMatch? = null

    interface IStartMatch {
        fun startMatch()
    }

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setupSocketEvents()
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        when (GameManager.currentGameMode) {
            MatchMode.SOLO -> {
                extra_options.visibility = View.GONE
                players.visibility = View.GONE
            }
            MatchMode.COLLABORATIVE -> {}
            MatchMode.FREE_FOR_ALL -> {}
            MatchMode.ONE_ON_ONE -> {}
        }

        start_match.setOnClickListener((({
            listener?.startMatch()
        })))
    }

    override fun onCreateView(
        inflater: LayoutInflater, container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View? {
        return inflater.inflate(R.layout.waiting_room_fragment, container, false)
    }


    override fun onAttach(context: Context) {
        super.onAttach(context)
        if (context is IStartMatch) {
            listener = context
        } else {
            throw RuntimeException(context.toString() + " must implement OnFragmentInteractionListener")
        }
    }

    override fun onDetach() {
        super.onDetach()
        listener = null
    }

    private fun setupSocketEvents() {
        SocketHandler.socket
            ?.on(SocketEvent.UPDATE_PLAYERS, ({ data ->
                val playerUpdate =
                    Gson().fromJson(data.first().toString(), Array<Player>::class.java)
            }))
    }
}
