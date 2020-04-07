package com.example.thin_client.ui.leaderboard

import com.example.thin_client.data.model.MatchInfos


object LeaderboardManager {

    val leaderboardTabNames = arrayListOf("Collaborative" ,"Free-for-all", "One-on-one","Solo")

    var soloRankingList =  ArrayList<MatchInfos>()
    var collabRankingList =  ArrayList<MatchInfos>()
    var oneVsOneRankingList = ArrayList<MatchInfos>()
    var freeForAllRankingList = ArrayList<MatchInfos>()


    fun resetRankingLists() {
        soloRankingList.clear()
        collabRankingList.clear()
        oneVsOneRankingList.clear()
        freeForAllRankingList.clear()
    }
}