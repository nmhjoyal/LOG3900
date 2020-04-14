package com.example.thin_client.data.rooms

import com.example.thin_client.data.Feedback
import com.example.thin_client.data.model.Room

data class JoinRoomFeedback(
    val feedback: Feedback,
    val room_joined: Room?,
    val isPrivate: Boolean?
)