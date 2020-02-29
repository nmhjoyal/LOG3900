package com.example.thin_client.data


data class SignInFeedback(
    val feedback: Feedback,
    val rooms_joined: MutableList<String>
)