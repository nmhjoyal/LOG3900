package com.example.thin_client.data

import com.example.thin_client.data.model.User
import java.util.*

data class Message(
    val content: String,
    val author: User,
    val date: Date
)
