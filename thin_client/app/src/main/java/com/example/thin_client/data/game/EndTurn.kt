package com.example.thin_client.data.game

data class EndTurn(
    val currentRound: Number,
    val scores: Array<Score>,
    val choices: Array<String>,
    val drawer: String
)