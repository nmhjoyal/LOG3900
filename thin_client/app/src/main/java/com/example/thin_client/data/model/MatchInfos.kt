package com.example.thin_client.data.model

data class MatchInfos (
    val matchId: String,
    val host: String,
    val nbRounds: Number,
    val timeLimit: Number,
    val matchMode: Int,
    val players: Array<PublicProfile>
)