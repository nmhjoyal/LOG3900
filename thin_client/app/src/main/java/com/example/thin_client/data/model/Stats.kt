package com.example.thin_client.data.model

data class Stats(
    val username: String,
    val matchCount: Number,
    val victoryPerc: Number,
    val averageTime: Number,
    val totalTime: Number,
    val bestSSS: Number,
    val connections: Array<Number>,
    val disconnections: Array<Number>,
    val matchesHistory: Array<MatchHistory>
)