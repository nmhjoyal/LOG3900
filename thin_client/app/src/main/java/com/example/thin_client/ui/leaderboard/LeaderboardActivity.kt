package com.example.thin_client.ui.leaderboard

import android.graphics.Color
import android.os.Bundle
import android.view.View
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import androidx.core.content.ContextCompat
import androidx.fragment.app.FragmentManager
import com.example.thin_client.R
import com.example.thin_client.data.game.GameManager
import com.example.thin_client.ui.game_mode.MyPagerAdapter
import com.google.android.material.tabs.TabLayout
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import com.xwray.groupie.Item
import kotlinx.android.synthetic.main.activity_games_list.*
import kotlinx.android.synthetic.main.activity_leaderboard.*
import kotlinx.android.synthetic.main.leaderboard_item.view.*

class LeaderboardActivity : AppCompatActivity() {

    private var currentTab: Int = 0
    private lateinit var manager: FragmentManager

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        manager = supportFragmentManager
        setContentView(R.layout.activity_leaderboard)
        ranking_viewpager.adapter = MyLeaderboardPagerAdapter(supportFragmentManager)
        setupTabs()
        ranking_tabLayout.addOnTabSelectedListener(object : TabLayout.OnTabSelectedListener {
            override fun onTabSelected(tab: TabLayout.Tab?) {
                if (tab != null && tab.customView != null) {
                    val tabName =
                        tab.customView!!.findViewById(R.id.tab_name) as TextView
                    tabName.setTextColor(Color.WHITE)
                    tab.customView!!.setBackgroundResource(R.drawable.tab_background_selected)
                    currentTab = tab.position
                }
            }

            override fun onTabReselected(tab: TabLayout.Tab?) {
            }

            override fun onTabUnselected(tab: TabLayout.Tab?) {
                if (tab != null && tab.customView != null) {
                    val tabName =
                        tab.customView!!.findViewById(R.id.tab_name) as TextView
                    tabName.setTextColor(ContextCompat.getColor(applicationContext, R.color.colorPrimaryDark))
                    tab.customView!!.setBackgroundResource(R.drawable.tab_background)
                    currentTab = 0
                }
            }
        })

    }


    private fun setupTabs() {
        for (i in 0 until ranking_tabLayout.tabCount) {
            val customView = View.inflate(applicationContext,R.layout.tab_layout, null)
            val tabName = customView.findViewById(R.id.tab_name) as TextView
            tabName.text = LeaderboardManager.leaderboardTabNames[i]
            if (i == currentTab) {
                tabName.setTextColor(Color.WHITE)
                customView.setBackgroundResource(R.drawable.tab_background_selected)
            } else {
                customView.setBackgroundResource(R.drawable.tab_background)
            }
            ranking_tabLayout.getTabAt(i)!!.customView = customView
            ranking_tabLayout.getTabAt(i)!!.view.setPadding(0, 0, 0, -16)
        }
    }
}

class LeaderboardItem(val username:String, val score:Int/*, val position: Long*/): Item<GroupieViewHolder>(){

    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        //viewHolder.itemView.textview_username_ranking.text = username//SocketHandler.user!!.username
        //viewHolder.itemView.textview_score.text = score.toString()
    }

    override fun getLayout(): Int {
        return R.layout.leaderboard_item
    }
}


