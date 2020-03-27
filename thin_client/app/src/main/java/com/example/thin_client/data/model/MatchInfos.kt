package com.example.thin_client.data.model

import com.example.thin_client.data.game.MatchMode


data class MatchInfos (
    val matchId: String,
    val host: String,
    val nbRounds: Number,
    val matchMode: MatchMode,
    val players: ArrayList<PublicProfile>
)