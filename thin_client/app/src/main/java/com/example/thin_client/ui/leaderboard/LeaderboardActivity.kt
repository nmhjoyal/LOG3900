package com.example.thin_client.ui.leaderboard

import android.os.Bundle
import androidx.appcompat.app.AppCompatActivity
import com.example.thin_client.R
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import com.xwray.groupie.Item
import kotlinx.android.synthetic.main.activity_leaderboard.*
import kotlinx.android.synthetic.main.leaderboard_item.view.*

class LeaderboardActivity : AppCompatActivity() {

    private val soloAdapter = GroupAdapter<GroupieViewHolder>()
    private val coopAdapter = GroupAdapter<GroupieViewHolder>()
    private val oneVsOneAdapter = GroupAdapter<GroupieViewHolder>()
    private val freeForAllAdapter = GroupAdapter<GroupieViewHolder>()
    private val reversedAdapter = GroupAdapter<GroupieViewHolder>()
    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_leaderboard)
        soloGameMode.adapter = soloAdapter
        cooperativeGameMode.adapter = coopAdapter
        oneVsOne.adapter = oneVsOneAdapter
        reversedGameMode.adapter = reversedAdapter
        freeforallMode.adapter = freeForAllAdapter
//en attendant la requête du serveur
        showSoloRanking("Amar", 20)
        showCoopRanking("Nicole", 30)
        showReversedRanking("Karima", 0)
        showFreeForAllRanking("Zak", 40)
        showOneVsOneRanking("Amar", 50)

    }
    private fun showSoloRanking(username:String, score: Int){
        soloAdapter.add(LeaderboardItem(username,score))
        if (soloGameMode != null){
            soloGameMode.scrollToPosition(soloAdapter.itemCount - 1)
        }
    }
    private fun showCoopRanking(username:String, score: Int){
        coopAdapter.add(LeaderboardItem(username,score))
        if (cooperativeGameMode != null){
            cooperativeGameMode.scrollToPosition(coopAdapter.itemCount - 1)
        }
    }
    private fun showOneVsOneRanking(username:String, score: Int){
        oneVsOneAdapter.add(LeaderboardItem(username,score))
        if (oneVsOne != null){
            oneVsOne.scrollToPosition(oneVsOneAdapter.itemCount - 1)
        }
    }
    private fun showFreeForAllRanking(username:String, score: Int){
        freeForAllAdapter.add(LeaderboardItem(username,score))
        if (freeforallMode != null){
            freeforallMode.scrollToPosition(freeForAllAdapter.itemCount - 1)
        }
    }
    private fun showReversedRanking(username:String, score: Int){
        reversedAdapter.add(LeaderboardItem(username,score))
        if (reversedGameMode!= null){
            reversedGameMode.scrollToPosition(reversedAdapter.itemCount - 1)
        }
    }
}

class LeaderboardItem(val username:String, val score:Int/*, val position: Long*/): Item<GroupieViewHolder>(){

    override fun bind(viewHolder: GroupieViewHolder, position: Int) {
        viewHolder.itemView.textview_username_ranking.text = username//SocketHandler.user!!.username
        viewHolder.itemView.textview_score.text = score.toString()
    }

    override fun getLayout(): Int {
        return R.layout.leaderboard_item
    }
}


