package com.example.thin_client.server.connect

import com.github.nkzawa.socketio.client.IO
import com.github.nkzawa.socketio.client.Socket

class ServerConnect {

    lateinit var mSocket: Socket

    fun connect(ipAddress: String, port: String) : Socket {
        mSocket = IO.socket("http://" + ipAddress + ":" + port)
        return mSocket.connect()
    }

    fun disconnect() {
        mSocket.disconnect()
    }

}