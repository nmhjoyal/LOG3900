package com.example.thin_client.data.server

object SocketEvent {
    const val USER_SIGNED_IN = "user_signed_in"
    const val USER_SIGNED_OUT = "user_signed_out"
    const val NEW_MESSAGE = "new_message"
    const val USER_JOINED_ROOM = "user_joined_room"
    const val USER_LEFT_ROOM = "user_left_room"

    const val SIGN_IN = "sign_in"
    const val SIGN_OUT = "sign_out"
    const val SEND_MESSAGE = "send_message"
    const val JOIN_ROOM = "join_chat_room"
    const val LEAVE_ROOM = "leave_chat_room"
    const val CREATE_ROOM = "create_chat_room"
    const val ROOM_CREATED = "room_created"
}