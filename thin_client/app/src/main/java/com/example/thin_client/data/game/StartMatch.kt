package com.example.thin_client.data.game

data class StartMatch(
    val matchId: String,
    val letterReveal: Boolean,
    val timeLimit: Number,
    val nbVirtualPlayer: Number
)