package com.example.thin_client.ui

import android.content.Context
import android.os.Bundle
import androidx.fragment.app.Fragment
import android.view.LayoutInflater
import android.view.View
import android.view.ViewGroup

import com.example.thin_client.R
import com.example.thin_client.ui.helpers.DEFAULT_INTERVAL
import com.example.thin_client.ui.helpers.setOnClickListener
import kotlinx.android.synthetic.main.lobby_menu_fragment.*

class LobbyMenuFragment : Fragment() {

    interface IStartNewFragment {
        fun startGameList()
        fun startFreeDraw()
    }

    private var startNewFragment: IStartNewFragment? = null

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        return inflater.inflate(R.layout.lobby_menu_fragment, container, false)
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        free_draw.setOnClickListener(DEFAULT_INTERVAL) {
            startNewFragment?.startFreeDraw()
        }

        join_match.setOnClickListener(DEFAULT_INTERVAL) {
            startNewFragment?.startGameList()
        }
    }

    override fun onAttach(context: Context) {
        super.onAttach(context)
        startNewFragment = context as? IStartNewFragment
        if (startNewFragment == null) {
        }
    }


}
