package com.example.thin_client.data.game

import com.example.thin_client.data.model.MatchInfos
import com.example.thin_client.ui.game_mode.Player

object GameManager {
    var currentGameMode: MatchMode = MatchMode.FREE_FOR_ALL
    var nbRounds: Int = 0
    var timeLimit: Int = 0
    val tabNames = arrayListOf("Collaborative","Free for all", "One-on-One")


    var collabModeMatchList =  ArrayList<MatchInfos>()
    var oneVsOneMatchList = ArrayList<MatchInfos>()
    var freeForAllMatchList = ArrayList<MatchInfos>()

    var playersList = ArrayList<Player>()
}