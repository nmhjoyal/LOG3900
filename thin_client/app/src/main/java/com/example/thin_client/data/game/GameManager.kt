package com.example.thin_client.data.game

import com.example.thin_client.data.model.MatchInfos
import com.example.thin_client.data.model.PublicProfile

object GameManager {
    var currentGameMode: MatchMode = MatchMode.FREE_FOR_ALL
    var nbRounds: Int = 0
    var timeLimit: Int = 0
    var canGuess = true
    var virtualPlayer =  Player(PublicProfile("Harry", "pear"), false, true, 30)
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

    fun removeVirtualPlayer() {
       for(player in playersList){
           if(player.isVirtual)
               playersList.drop(1)
       }
    }

    fun addVirtualPlayer() {
        playersList.add(virtualPlayer)
    }

}