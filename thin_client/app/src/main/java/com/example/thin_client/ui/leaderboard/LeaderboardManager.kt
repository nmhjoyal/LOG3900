package com.example.thin_client.ui.leaderboard

import com.example.thin_client.data.model.RankClient


object LeaderboardManager {

    val leaderboardTabNames = arrayListOf("Free-for-all" , "Solo", "Collaborative", "One-on-one")

    var soloRankingList =  ArrayList<RankClient>()
    var collabRankingList =  ArrayList<RankClient>()
    var oneVsOneRankingList = ArrayList<RankClient>()
    var freeForAllRankingList = ArrayList<RankClient>()


    fun resetRankingLists() {
        soloRankingList.clear()
        collabRankingList.clear()
        oneVsOneRankingList.clear()
        freeForAllRankingList.clear()
    }
}