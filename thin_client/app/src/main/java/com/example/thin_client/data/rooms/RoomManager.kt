package com.example.thin_client.data.rooms

import com.example.thin_client.data.Message
import com.example.thin_client.data.model.Room

object RoomManager {
    var roomsJoined: MutableMap<String, ArrayList<Message>> = mutableMapOf()
    var currentRoom: String = ""
    var roomToDelete: String = ""

    fun createRoomList(rooms: MutableList<Room>) {
        for(room in rooms){
            roomsJoined.put(room.id, room.messages)
        }

    }

    fun addRoom(roomId: String) {
        roomsJoined.put(roomId, arrayListOf())
    }

    fun loadHistory(roomId:String, messagesList: ArrayList<Message>){
        roomsJoined.put(roomId, messagesList)
    }

    fun leaveRoom() {
        roomsJoined.remove(roomToDelete)
        roomToDelete = ""
    }
}