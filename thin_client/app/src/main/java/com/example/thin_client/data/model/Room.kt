package com.example.thin_client.data.model

import com.example.thin_client.data.Message

data class Room (
    val name: String,
    val messages: ArrayList<Message>,
    val avatars: Map<String,String>
)