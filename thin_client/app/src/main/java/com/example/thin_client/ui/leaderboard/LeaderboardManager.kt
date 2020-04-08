package com.example.thin_client.ui.leaderboard

import com.example.thin_client.data.model.MatchInfos
import com.example.thin_client.data.model.Rank


object LeaderboardManager {

    val leaderboardTabNames = arrayListOf("Collaborative" ,"Free-for-all", "One-on-one","Solo")

    var soloRankingList =  ArrayList<Rank>()
    var collabRankingList =  ArrayList<Rank>()
    var oneVsOneRankingList = ArrayList<Rank>()
    var freeForAllRankingList = ArrayList<Rank>()


    fun resetRankingLists() {
        soloRankingList.clear()
        collabRankingList.clear()
        oneVsOneRankingList.clear()
        freeForAllRankingList.clear()
    }
}