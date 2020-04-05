package com.example.thin_client.data.game

data class EndTurn(
    val currentRound: Number,
    val players: Array<Player>,
    val choices: Array<String>,
    val drawer: String
)