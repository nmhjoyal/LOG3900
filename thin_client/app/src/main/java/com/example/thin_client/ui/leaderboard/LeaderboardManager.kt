package com.example.thin_client.ui.leaderboard

import com.example.thin_client.data.model.Rank


object LeaderboardManager {

    val leaderboardTabNames = arrayListOf("Free-for-all" , "Solo", "Collaborative", "One-on-one")

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