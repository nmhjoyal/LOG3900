package com.example.thin_client.data

import com.example.thin_client.data.model.PublicProfile

data class Message(
    val author: PublicProfile,
    val content: String,
    val date: Long,
    val roomId: String
)
