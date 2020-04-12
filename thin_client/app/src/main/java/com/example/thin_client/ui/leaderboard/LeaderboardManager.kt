package com.example.thin_client.ui.leaderboard

import com.example.thin_client.data.model.RankClient


object LeaderboardManager {

    val leaderboardTabNames = arrayListOf("Collaborative","Free-for-all" , "One-on-one","Solo")

    var soloRankingList =  ArrayList<RankClient>()
    var collabRankingList =  ArrayList<RankClient>()
    var oneVsOneRankingList = ArrayList<RankClient>()
    var freeForAllRankingList = ArrayList<RankClient>()
    var collabCurrentPlayer: RankClient = RankClient("",0,0)
    var soloCurrentPlayer: RankClient = RankClient("",0,0)
    var onevsoneCurrentPlayer: RankClient = RankClient("",0,0)
    var freeforallCurrentPlayer: RankClient = RankClient("",0,0)

    fun resetRankingLists() {
        soloRankingList.clear()
        collabRankingList.clear()
        oneVsOneRankingList.clear()
        freeForAllRankingList.clear()
    }
}