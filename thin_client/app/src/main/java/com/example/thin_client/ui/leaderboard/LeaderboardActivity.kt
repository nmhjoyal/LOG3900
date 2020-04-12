package com.example.thin_client.ui.leaderboard

import OkHttpRequest
import android.graphics.Color
import android.os.Bundle
import android.os.Handler
import android.os.Looper
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
import com.google.android.material.tabs.TabLayout
import com.google.gson.Gson
import com.xwray.groupie.GroupieViewHolder
import com.xwray.groupie.Item
import kotlinx.android.synthetic.main.activity_leaderboard.*
import kotlinx.android.synthetic.main.leaderboard_item.view.*
import okhttp3.Call
import java.io.IOException

class LeaderboardActivity : AppCompatActivity() {

    private lateinit var manager: FragmentManager

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        manager = supportFragmentManager
        setContentView(R.layout.activity_leaderboard)
        ranking_viewpager.adapter = MyLeaderboardPagerAdapter(supportFragmentManager)
        getRankings()
        refresh_ranking.setOnClickListener{
            getRankings()
        }

    }



    private fun getRankings() {
        val httpClient = OkHttpRequest(okhttp3.OkHttpClient())
        httpClient.GET(
            HTTPRequest.BASE_URL + HTTPRequest.URL_RANKING + PreferenceHandler(applicationContext).getUser().username + "/" + MatchMode.SOLO.ordinal,
            object : okhttp3.Callback {
                //N'entre pas dans le on failure
                override fun onFailure(call: Call, e: IOException) {
                }

                override fun onResponse(call: Call, response: okhttp3.Response) {
                    val responseData = response.body?.charStream()
                    val soloRankingInfo = Gson().fromJson(responseData, Array<RankClient>::class.java)
                    LeaderboardManager.soloRankingList.clear()
                    LeaderboardManager.soloRankingList.addAll(soloRankingInfo)
                    LeaderboardManager.soloRankingList.sortBy(({ it.pos.toInt() }))
                    Handler(Looper.getMainLooper()).post(({
                        if (ranking_viewpager.adapter != null) {
                            ranking_viewpager.adapter?.notifyDataSetChanged()
                        }
                    }))
                }
            })
        httpClient.GET(
            HTTPRequest.BASE_URL + HTTPRequest.URL_RANKING + PreferenceHandler(applicationContext).getUser().username + "/" + MatchMode.COLLABORATIVE.ordinal,
            object : okhttp3.Callback {
                //N'entre pas dans le on failure
                override fun onFailure(call: Call, e: IOException) {
                }

                override fun onResponse(call: Call, response: okhttp3.Response) {
                    val responseData = response.body?.charStream()
                    val collabRankingInfo = Gson().fromJson(responseData, Array<RankClient>::class.java)
                    LeaderboardManager.collabRankingList.clear()
                    LeaderboardManager.collabRankingList.addAll(collabRankingInfo)
                    LeaderboardManager.collabRankingList.sortBy(({ it.pos.toInt() }))
                    Handler(Looper.getMainLooper()).post(({
                        if (ranking_viewpager.adapter != null) {
                            ranking_viewpager.adapter?.notifyDataSetChanged()
                        }
                    }))
                }
            })
        httpClient.GET(
            HTTPRequest.BASE_URL + HTTPRequest.URL_RANKING + PreferenceHandler(applicationContext).getUser().username + "/" + MatchMode.ONE_ON_ONE.ordinal,
            object : okhttp3.Callback {
                //N'entre pas dans le on failure
                override fun onFailure(call: Call, e: IOException) {
                }

                override fun onResponse(call: Call, response: okhttp3.Response) {
                    val responseData = response.body?.charStream()
                    val oneVsOneRankingInfo = Gson().fromJson(responseData, Array<RankClient>::class.java)
                    LeaderboardManager.oneVsOneRankingList.clear()
                    LeaderboardManager.oneVsOneRankingList.addAll(oneVsOneRankingInfo)
                    LeaderboardManager.oneVsOneRankingList.sortBy(({ it.pos.toInt() }))
                    Handler(Looper.getMainLooper()).post(({
                        if (ranking_viewpager.adapter != null) {
                            ranking_viewpager.adapter?.notifyDataSetChanged()
                        }
                    }))
                }
            })
        httpClient.GET(
            HTTPRequest.BASE_URL + HTTPRequest.URL_RANKING + PreferenceHandler(applicationContext).getUser().username + "/" + MatchMode.FREE_FOR_ALL.ordinal,
            object : okhttp3.Callback {
                //N'entre pas dans le on failure
                override fun onFailure(call: Call, e: IOException) {
                }

                override fun onResponse(call: Call, response: okhttp3.Response) {
                    val responseData = response.body?.charStream()
                    val freeForAllrankingInfo = Gson().fromJson(responseData, Array<RankClient>::class.java)
                    LeaderboardManager.freeForAllRankingList.clear()
                    LeaderboardManager.freeForAllRankingList.addAll(freeForAllrankingInfo)
                    LeaderboardManager.freeForAllRankingList.sortBy(({ it.pos.toInt() }))
                    Handler(Looper.getMainLooper()).post(({
                        if (ranking_viewpager.adapter != null) {
                            ranking_viewpager.adapter?.notifyDataSetChanged()
                        }
                    }))
                }
            })
    }

}

class LeaderboardItem(val ranking: RankClient) : Item<GroupieViewHolder>() {

    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        viewHolder.itemView.position.text = ranking.pos.toString()
        viewHolder.itemView.text_view_ranking_username.text = ranking.username
        viewHolder.itemView.text_view_ranking_score.text = ranking.score.toString()
    }

    override fun getLayout(): Int {
        return R.layout.leaderboard_item
    }
}


