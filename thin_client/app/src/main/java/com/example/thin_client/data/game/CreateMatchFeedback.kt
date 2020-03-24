package com.example.thin_client.data.game

import com.example.thin_client.data.Feedback

data class CreateMatchFeedback(
    val feedback: Feedback,
    val matchId: String
)