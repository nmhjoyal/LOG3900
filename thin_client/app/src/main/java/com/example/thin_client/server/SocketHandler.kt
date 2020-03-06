package com.example.thin_client.server

import com.example.thin_client.data.ClientMessage
import com.example.thin_client.data.drawing.DrawPoint
import com.example.thin_client.data.model.PrivateProfile
import com.example.thin_client.data.model.User
import com.example.thin_client.data.rooms.CreateRoom
import com.example.thin_client.data.server.HTTPRequest
import com.example.thin_client.data.server.SocketEvent
import com.github.nkzawa.socketio.client.IO
import com.github.nkzawa.socketio.client.Socket
import com.google.gson.Gson

object SocketHandler {
    var user: User? = null
    var socket: Socket? = null
    var isLoggedIn = false


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
        isLoggedIn = false
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

    fun deleteChatRoom(roomid: String) {
        socket!!.emit(SocketEvent.DELETE_ROOM, roomid)
    }

    fun createChatRoom(roomid: String, isPrivate: Boolean) {
        val newRoom = Gson().toJson(CreateRoom(roomid,  isPrivate))
        socket!!.emit(SocketEvent.CREATE_ROOM, newRoom)
    }

    fun updateProfile(privateProfile: PrivateProfile) {
        val gson = Gson()
        val args = gson.toJson(privateProfile)
        socket!!.emit(SocketEvent.UPDATE_PROFILE, args)
    }

    fun connectOnlineDraw() {
        socket!!.emit(SocketEvent.CONNECT_FREE_DRAW)
    }

    fun disconnectOnlineDraw() {
        socket!!.emit(SocketEvent.DISCONNECT_FREE_DRAW)
    }

    fun drawPoint(drawPoint: DrawPoint) {
        val gson = Gson()
        val args = gson.toJson(drawPoint)
        socket!!.emit(SocketEvent.DRAW_TEST, args)
    }
}