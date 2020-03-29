package com.example.thin_client.ui.game_mode

import androidx.fragment.app.Fragment
import androidx.fragment.app.FragmentManager
import androidx.fragment.app.FragmentPagerAdapter
import com.example.thin_client.data.game.GameManager.tabNames

class MyPagerAdapter(fm: FragmentManager) : FragmentPagerAdapter(fm) {

    override fun getItem(position: Int): Fragment {
        return when (position) {
            0 -> CollabMatchMode()
            1 -> FreeForAllMatchMode()
            else -> {
                return OneVsOneMatchMode()
            }
        }
    }

    override fun getCount(): Int {
        return 3
    }

    override fun getPageTitle(position: Int): CharSequence {
        return if (position < tabNames.size) tabNames[position] else tabNames[tabNames.lastIndex]
    }
}
