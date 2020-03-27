package com.example.thin_client.data.game

import com.example.thin_client.data.model.MatchInfos

object GameManager {
    var currentGameMode: MatchMode = MatchMode.FREE_FOR_ALL
    var nbRounds: Int = 0
    var timeLimit: Int = 0
    val tabNames = arrayListOf("Collaborative", "Solo" ,"Free-for-all", "One-on-one")


    var soloModeMatchList =  ArrayList<MatchInfos>()
    var collabModeMatchList =  ArrayList<MatchInfos>()
    var oneVsOneMatchList = ArrayList<MatchInfos>()
    var freeForAllMatchList = ArrayList<MatchInfos>()

}