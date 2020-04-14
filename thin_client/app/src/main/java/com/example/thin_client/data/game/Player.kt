package com.example.thin_client.data.game

import com.example.thin_client.data.model.PublicProfile

data class Player(
    val user: PublicProfile,
    val score: UpdateScore,
    val isVirtual: Boolean
)