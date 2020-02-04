package com.example.thin_client.data

import com.example.thin_client.data.model.User

data class Message(
    val content: String,
    val author: User
)
