package com.example.thin_client.data

import com.example.thin_client.data.model.PublicProfile

data class Message(
    val username: String,
    val content: String,
    val date: Long,
    val roomId: String
)
