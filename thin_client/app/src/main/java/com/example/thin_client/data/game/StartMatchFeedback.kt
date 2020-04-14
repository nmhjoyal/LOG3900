package com.example.thin_client.data.game

import com.example.thin_client.data.Feedback

data class StartMatchFeedback(
    val feedback: Feedback,
    val nbRounds: Number
)