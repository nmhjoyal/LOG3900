package com.example.thin_client.ui.game_mode

import androidx.fragment.app.Fragment
import androidx.fragment.app.FragmentManager
import androidx.fragment.app.FragmentPagerAdapter
import com.example.thin_client.data.game.GameManager.tabNames

class MyPagerAdapter(fm: FragmentManager) : FragmentPagerAdapter(fm) {

    override fun getItem(position: Int): Fragment {
        return when (position) {
            0 -> Fragment1()
            1 -> Fragment1()
            2 -> Fragment1()
            else -> {
                return Fragment1()
            }
        }
    }

    override fun getCount(): Int {
        return 4
    }

    override fun getPageTitle(position: Int): CharSequence {
        return if (position < tabNames.size) tabNames[position] else tabNames[tabNames.lastIndex]
    }
}
