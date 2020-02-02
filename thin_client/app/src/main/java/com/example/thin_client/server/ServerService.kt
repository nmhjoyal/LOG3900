package com.example.thin_client.server

import android.app.Service
import android.content.Intent
import android.os.IBinder
import com.github.nkzawa.socketio.client.IO
import com.github.nkzawa.socketio.client.Socket
import com.squareup.otto.Subscribe

class ServerService : Service() {

    var mInstance: ServerService = this
    lateinit var mSocket: Socket

    fun connect(ipAddress: String, port: String) : Socket {
        mSocket = IO.socket("http://" + ipAddress + ":" + port)
        return mSocket.connect()
    }

    fun disconnect() {
        mSocket.disconnect()
    }

    override fun onBind(intent: Intent): IBinder {
        TODO("Return the communication channel to the service.")
    }

    @Subscribe fun answerLogout(test: String) {
        disconnect()
    }

    override fun onStartCommand(intent: Intent?, flags: Int, startId: Int): Int {
        val ipAddress = intent?.getStringExtra("ipAddress")
        val port = intent?.getStringExtra("port")
        if (ipAddress != null && port != null) {
            connect(ipAddress, port)
        }
        return super.onStartCommand(intent, flags, startId)
    }

    override fun onDestroy() {
        disconnect()
        super.onDestroy()
    }
}
