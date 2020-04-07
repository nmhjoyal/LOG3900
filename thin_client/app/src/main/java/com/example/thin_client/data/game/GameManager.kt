package com.example.thin_client.data.game

import com.example.thin_client.data.model.MatchInfos

object GameManager {
    var currentGameMode: MatchMode = MatchMode.FREE_FOR_ALL
    var isGameStarted = false
    var nbRounds: Int = 0
    var timeLimit: Int = 0
    var canGuess = true
    val tabNames = arrayListOf("Collaborative" ,"Free-for-all", "One-on-one")


    var collabModeMatchList =  ArrayList<MatchInfos>()
    var oneVsOneMatchList = ArrayList<MatchInfos>()
    var freeForAllMatchList = ArrayList<MatchInfos>()

    var playersList = ArrayList<Player>()

    fun resetMatchLists() {
        collabModeMatchList.clear()
        oneVsOneMatchList.clear()
        freeForAllMatchList.clear()
    }
}