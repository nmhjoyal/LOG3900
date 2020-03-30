package com.example.thin_client.data.model

import com.example.thin_client.data.Message

data class Room (
    val id: String,
    val messages: ArrayList<Message>,
    val avatars: MutableMap<String,String>
)