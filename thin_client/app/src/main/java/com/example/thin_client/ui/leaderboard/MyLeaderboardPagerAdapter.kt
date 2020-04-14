package com.example.thin_client.ui.leaderboard

import androidx.fragment.app.Fragment
import androidx.fragment.app.FragmentManager
import androidx.fragment.app.FragmentPagerAdapter
import androidx.viewpager.widget.PagerAdapter
import com.example.thin_client.ui.leaderboard.LeaderboardManager.leaderboardTabNames

class MyLeaderboardPagerAdapter(fm: FragmentManager) : FragmentPagerAdapter(fm) {

    override fun getItem(position: Int): Fragment {
        return when (position) {
            0 -> RankingCollabMode()
            1 -> RankingFreeForAllMode()
            2 -> RankingOneVsOneMode()
            else -> {
                return RankingSoloMode()
            }
        }
    }

    override fun getCount(): Int {
        return 4
    }

    override fun getItemPosition(`object`: Any): Int {
        return PagerAdapter.POSITION_NONE
    }

    override fun getPageTitle(position: Int): CharSequence {
      return if (position < leaderboardTabNames.size) leaderboardTabNames[position] else leaderboardTabNames[leaderboardTabNames.lastIndex]
    }
}
