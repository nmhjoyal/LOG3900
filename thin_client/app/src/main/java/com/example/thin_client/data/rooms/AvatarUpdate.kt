package com.example.thin_client.data.rooms

import com.example.thin_client.data.model.PublicProfile

data class AvatarUpdate(
    val roomId: String,
    val profile: PublicProfile
)