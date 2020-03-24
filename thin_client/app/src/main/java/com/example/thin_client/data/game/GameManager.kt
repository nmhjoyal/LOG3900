package com.example.thin_client.data.game

object GameManager {
    var currentGameMode: MatchMode = MatchMode.FREE_FOR_ALL
    var nbRounds: Int = 0
    val tabNames = arrayListOf("Collaborative", "Free-for-all", "One-on-one", "Reverse")
    var roomName: String = ""
}