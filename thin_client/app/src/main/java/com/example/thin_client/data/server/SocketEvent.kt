package com.example.thin_client.data.server

object SocketEvent {
    const val USER_SIGNED_IN = "user_signed_in"
    const val USER_SIGNED_OUT = "user_signed_out"
    const val SIGN_IN = "sign_in"
    const val SIGN_OUT = "sign_out"

    const val NEW_MESSAGE = "new_message"
    const val USER_JOINED_ROOM = "user_joined_room"
    const val USER_LEFT_ROOM = "user_left_room"
    const val SEND_MESSAGE = "send_message"
    const val JOIN_ROOM = "join_chat_room"
    const val LEAVE_ROOM = "leave_chat_room"
    const val DELETE_ROOM = "delete_chat_room"
    const val ROOM_DELETED = "room_deleted"
    const val CREATE_ROOM = "create_chat_room"
    const val ROOM_CREATED = "room_created"
    const val GET_ROOMS = "get_rooms"
    const val ROOMS = "rooms_retrieved"
    const val SEND_INVITE = "send_invite"
    const val USER_SENT_INVITE = "user_sent_invite"
    const val RECEIVE_INVITE = "receive_invite"
    const val LOAD_HISTORY = "load_history"

    const val UPDATE_PROFILE = "update_profile"
    const val PROFILE_UPDATED = "profile_updated"

    const val DRAWER = "drawer"
    const val OBSERVER = "observer"
    const val DRAW_POINT = "drawPoint"
    const val DRAW_TEST = "drawTest"
    const val TOUCH_DOWN = "touchDown"
    const val TOUCH_UP = "touchUp"
    const val START_TRACE = "start_trace"
    const val STOP_TRACE = "stop_trace"
    const val CONNECT_FREE_DRAW = "connect_free_draw"
    const val DISCONNECT_FREE_DRAW = "disconnect_free_draw"
}