package com.example.thin_client.data.model

data class MatchHistory(
    val startTime: Number,
    val endTime: Number,
    val matchMode: Number,
    val playerNames: Array<String>,
    val winner: Rank,
    val myScore: Number

)