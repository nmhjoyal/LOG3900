package com.example.thin_client.ui.game_mode


import androidx.fragment.app.Fragment
import androidx.fragment.app.FragmentManager
import androidx.fragment.app.FragmentPagerAdapter




class MyPagerAdapter(fm: FragmentManager) : FragmentPagerAdapter(fm) {

    override fun getItem(position: Int): Fragment {
        return when (position) {
            0 -> {
                Fragment1()
            }
            1 -> Fragment1()
            2 -> Fragment1()
            3 -> Fragment1()
            else -> {
                return Fragment1()
            }
        }
    }

    override fun getCount(): Int {
        return 5
    }

    override fun getPageTitle(position: Int): CharSequence {
        return when (position) {
            0 -> "Solo"
            1 -> "Coop"
            2 -> "Free for All"
            3 -> "One vs One"
            else -> {
                return "Reversed"
            }
        }
    }
}
