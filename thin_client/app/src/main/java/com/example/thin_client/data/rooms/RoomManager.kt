package com.example.thin_client.data.rooms

object RoomManager {
    var roomsJoined: MutableList<String> = arrayListOf()

    fun createRoomList(roomArray: MutableList<String>) {
        roomsJoined = roomArray.toCollection(ArrayList())
    }

    fun addRoom(room: String) {
        roomsJoined.add(room)
    }

    fun leaveRoom(room: String) {
        roomsJoined.remove(room)
    }
}