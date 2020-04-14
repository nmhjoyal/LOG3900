package com.example.thin_client.data.game

data class CreateMatch(
    val nbRounds: Number,
    val timeLimit: Number,
    val matchMode: Int
)