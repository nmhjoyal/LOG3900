package com.example.thin_client.server

import com.example.thin_client.data.Message
import com.example.thin_client.data.model.User
import com.github.nkzawa.socketio.client.IO
import com.github.nkzawa.socketio.client.Socket
import com.google.gson.Gson

object SocketHandler {
    var socket: Socket? = null
    var user: User? = null

    fun connect(ipAddress: String, port: String): Socket {
        if (socket == null) {
            socket = IO.socket("http://" + ipAddress + ":" + port)

        }
        socket!!.io().reconnection(false)
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
        socket!!.emit("sign_in", jsonUser)
    }

    fun logout() {
        socket!!.emit("sign_out")
        disconnect()
    }

    fun sendMessage(text: String) {
        val gson = Gson()
        val message = gson.toJson(Message(text, user!!))
        socket!!.emit("send_message", message)
    }

    fun joinRoom() {
        socket!!.emit("join_chat_room")
    }

}