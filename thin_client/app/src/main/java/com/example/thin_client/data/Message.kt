package com.example.thin_client.data

data class Message(
    val username: String,
    val content: String,
    val date: Long,
    val roomId: String
)
