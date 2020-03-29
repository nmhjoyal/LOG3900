package com.example.thin_client.data.game

import com.example.thin_client.data.model.PublicProfile

data class Player(
    val user: PublicProfile,
    val isHost: Boolean,
    val isVirtual: Boolean,
    val score: Number
)