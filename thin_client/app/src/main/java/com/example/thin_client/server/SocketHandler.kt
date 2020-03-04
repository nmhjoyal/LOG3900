package com.example.thin_client.server

import com.example.thin_client.data.ClientMessage
import com.example.thin_client.data.model.PrivateProfile
import com.example.thin_client.data.model.User
import com.example.thin_client.data.server.HTTPRequest
import com.example.thin_client.data.server.SocketEvent
import com.github.nkzawa.socketio.client.IO
import com.github.nkzawa.socketio.client.Socket
import com.google.gson.Gson

object SocketHandler {
    var user: User? = null
    var socket: Socket? = null


    fun connect(): Socket {
        socket = IO.socket(HTTPRequest.BASE_URL)
        return socket!!.connect()
    }

    fun disconnect() {
        if (socket != null) {
            socket!!.off()
            socket!!.disconnect()
            socket = null
        }
    }

    fun login(user: User) {
        this.user = user
        val gson = Gson()
        val jsonUser = gson.toJson(user)
        socket!!.emit(SocketEvent.SIGN_IN, jsonUser)
    }

    fun logout() {
        if (socket != null) {
            socket!!.emit(SocketEvent.SIGN_OUT)
        }
    }

    fun sendMessage(text: String, roomid: String) {
        val gson = Gson()
        val message = gson.toJson(ClientMessage( text,  roomid))
        socket!!.emit(SocketEvent.SEND_MESSAGE, message)
    }

    fun joinChatRoom(roomid: String) {
        socket!!.emit(SocketEvent.JOIN_ROOM, roomid)
    }

    fun leaveChatRoom(roomid: String) {
        socket!!.emit(SocketEvent.LEAVE_ROOM, roomid)
    }

    fun createChatRoom(roomid: String) {
        socket!!.emit(SocketEvent.CREATE_ROOM, roomid)
    }

    fun updateProfile(privateProfile: PrivateProfile) {
        val gson = Gson()
        val args = gson.toJson(privateProfile)
        socket!!.emit(SocketEvent.UPDATE_PROFILE, args)
    }
}