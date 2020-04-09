package com.example.thin_client.data.game

data class UpdateSprint(
    val guess: Number,
    val time: Number,
    val word: String,
    val players: Array<Player>
)