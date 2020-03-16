package com.example.thin_client.data.rooms

import com.example.thin_client.data.Message
import com.example.thin_client.data.model.Room

object RoomManager {
    var roomsJoined: MutableMap<String, ArrayList<Message>> = mutableMapOf()
    var roomAvatars: MutableMap<String, Map<String, String>> = mutableMapOf()
    var invites: ArrayList<String> = arrayListOf()
    var currentRoom: String = ""
    var roomToRemove: String = ""

    fun createRoomList(rooms: MutableList<Room>) {
        for(room in rooms){
            roomsJoined.put(room.id, room.messages)
            roomAvatars.put(room.id, room.avatars)
        }

    }

    fun addRoom(room: Room) {
        roomsJoined.put(room.id, room.messages)
        roomAvatars.put(room.id, room.avatars)
    }

    fun loadHistory(roomId:String, messagesList: ArrayList<Message>){
        roomsJoined.put(roomId, messagesList)
    }

    fun leaveRoom() {
        roomsJoined.remove(roomToRemove)
        roomAvatars.remove(roomToRemove)
        roomToRemove = ""
    }
}