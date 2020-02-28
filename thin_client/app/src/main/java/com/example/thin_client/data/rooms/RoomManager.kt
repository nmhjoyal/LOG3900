package com.example.thin_client.data.rooms

object RoomManager {
    var roomsJoined: ArrayList<String> = ArrayList()

    fun createRoomList(roomArray: Array<String>) {
        roomsJoined = roomArray.toCollection(ArrayList())
    }

    fun addRoom(room: String) {
        roomsJoined.add(room)
    }

    fun leaveRoom(room: String) {
        roomsJoined.remove(room)
    }
}