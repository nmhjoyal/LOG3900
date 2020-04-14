package com.example.thin_client.data

import com.example.thin_client.data.model.Room


data class SignInFeedback(
    val feedback: Feedback,
    val rooms_joined: MutableList<Room>
)