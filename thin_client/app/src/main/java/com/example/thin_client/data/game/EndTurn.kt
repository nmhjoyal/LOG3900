package com.example.thin_client.data.game

data class EndTurn(
    val currentRound: Number,
    val scores: Map<String, UpdateScore>,
    val choices: Array<String>,
    val drawer: String
)