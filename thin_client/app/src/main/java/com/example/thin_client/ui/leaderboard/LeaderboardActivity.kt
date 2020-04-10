package com.example.thin_client.ui.leaderboard

import OkHttpRequest
import android.graphics.Color
import android.os.Bundle
import android.view.View
import android.widget.TextView
import androidx.appcompat.app.AppCompatActivity
import androidx.core.content.ContextCompat
import androidx.fragment.app.FragmentManager
import com.example.thin_client.R
import com.example.thin_client.data.app_preferences.PreferenceHandler
import com.example.thin_client.data.game.MatchMode
import com.example.thin_client.data.model.RankClient
import com.example.thin_client.data.server.HTTPRequest
import com.example.thin_client.ui.helpers.DEFAULT_INTERVAL
import com.google.android.material.tabs.TabLayout
import com.google.gson.Gson
import com.xwray.groupie.GroupieViewHolder
import com.xwray.groupie.Item
import kotlinx.android.synthetic.main.activity_leaderboard.*
import kotlinx.android.synthetic.main.leaderboard_item.view.*
import okhttp3.Call
import java.io.IOException

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
                    tabName.setTextColor(
                        ContextCompat.getColor(
                            applicationContext,
                            R.color.colorPrimaryDark
                        )
                    )
                    tab.customView!!.setBackgroundResource(R.drawable.tab_background)
                    currentTab = 0
                }
            }
        })

        getRankings()
        refresh_ranking.setOnClickListener{
            getRankings()
        }

    }


    private fun setupTabs() {
        for (i in 0 until ranking_tabLayout.tabCount) {
            val customView = View.inflate(applicationContext, R.layout.tab_layout, null)
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

    private fun getRankings() {
        val httpClient = OkHttpRequest(okhttp3.OkHttpClient())
        httpClient.GET(
            HTTPRequest.BASE_URL + HTTPRequest.URL_RANKING + PreferenceHandler(applicationContext).getUser() + MatchMode.SOLO,
            object : okhttp3.Callback {
                //N'entre pas dans le on failure
                override fun onFailure(call: Call, e: IOException) {
                }

                override fun onResponse(call: Call, response: okhttp3.Response) {
                    val responseData = response.body?.charStream()
                    val soloRankingInfo = Gson().fromJson(responseData, Array<RankClient>::class.java)
                    runOnUiThread(({
                        for (rank in soloRankingInfo) {
                            if (!LeaderboardManager.soloRankingList.contains(rank) && LeaderboardManager.soloRankingList.size <= 10) {
                                LeaderboardManager.soloRankingList.add(rank)
                            }
                        }
                    }))
                }
            })
        httpClient.GET(
            HTTPRequest.BASE_URL + HTTPRequest.URL_RANKING + PreferenceHandler(applicationContext).getUser() + MatchMode.COLLABORATIVE,
            object : okhttp3.Callback {
                //N'entre pas dans le on failure
                override fun onFailure(call: Call, e: IOException) {
                }

                override fun onResponse(call: Call, response: okhttp3.Response) {
                    val responseData = response.body?.charStream()
                    val collabRankingInfo = Gson().fromJson(responseData, Array<RankClient>::class.java)
                    runOnUiThread(({
                        for (rank in collabRankingInfo) {
                            if (!LeaderboardManager.collabRankingList.contains(rank) && LeaderboardManager.collabRankingList.size <=10) {
                                LeaderboardManager.collabRankingList.add(rank)
                            }
                        }
                    }))
                }
            })
        httpClient.GET(
            HTTPRequest.BASE_URL + HTTPRequest.URL_RANKING + PreferenceHandler(applicationContext).getUser() + MatchMode.ONE_ON_ONE,
            object : okhttp3.Callback {
                //N'entre pas dans le on failure
                override fun onFailure(call: Call, e: IOException) {
                }

                override fun onResponse(call: Call, response: okhttp3.Response) {
                    val responseData = response.body?.charStream()
                    val oneVsOneRankingInfo = Gson().fromJson(responseData, Array<RankClient>::class.java)
                    runOnUiThread(({
                        for (rank in oneVsOneRankingInfo) {
                            if (!LeaderboardManager.oneVsOneRankingList.contains(rank) && LeaderboardManager.oneVsOneRankingList.size <= 10) {
                                LeaderboardManager.oneVsOneRankingList.add(rank)
                            }
                        }
                    }))
                }
            })
        httpClient.GET(
            HTTPRequest.BASE_URL + HTTPRequest.URL_RANKING + PreferenceHandler(applicationContext).getUser() + MatchMode.FREE_FOR_ALL,
            object : okhttp3.Callback {
                //N'entre pas dans le on failure
                override fun onFailure(call: Call, e: IOException) {
                }

                override fun onResponse(call: Call, response: okhttp3.Response) {
                    val responseData = response.body?.charStream()
                    val freeForAllrankingInfo = Gson().fromJson(responseData, Array<RankClient>::class.java)
                    runOnUiThread(({
                        for (rank in freeForAllrankingInfo) {
                            if (!LeaderboardManager.freeForAllRankingList.contains(rank) && LeaderboardManager.freeForAllRankingList.size<=10) {
                                LeaderboardManager.freeForAllRankingList.add(rank)
                            }
                        }
                    }))
                }
            })
    }

}

class LeaderboardItem(val ranking:RankClient) : Item<GroupieViewHolder>() {

    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        viewHolder.itemView.text_view_ranking_username.text = ranking.username
        viewHolder.itemView.text_view_ranking_score.text = ranking.score.toString()
    }

    override fun getLayout(): Int {
        return R.layout.leaderboard_item
    }
}


