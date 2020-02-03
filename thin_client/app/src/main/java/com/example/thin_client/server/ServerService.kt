package com.example.thin_client.server

import android.app.Service
import android.content.Intent
import android.os.IBinder
import com.example.thin_client.data.model.User
import com.github.nkzawa.socketio.client.IO
import com.github.nkzawa.socketio.client.Socket
import com.google.gson.Gson
import com.squareup.otto.Subscribe

class ServerService(ipAddress: String, port: String) : Service() {

    var mSocket: Socket? = null

    init {
        mSocket = IO.socket("http://" + ipAddress + ":" + port)
        GlobalBus.bus?.register(this)
        connect()
    }

    fun connect() : Socket? {
        mSocket?.connect()
        return mSocket
    }

    fun disconnect() {
        mSocket?.disconnect()
    }

    fun login(user: User) {
        val gson = Gson()
        val jsonUser = gson.toJson(user)
        mSocket?.emit("sign_in", jsonUser)
    }

    override fun onBind(intent: Intent): IBinder {
        TODO("Return the communication channel to the service.")
    }

    @Subscribe
    fun answerLogout(event: LogoutEvent) {
        mSocket?.emit("sign_out")
        disconnect()
    }


    override fun onDestroy() {
        disconnect()
        GlobalBus.bus?.unregister(this)
        super.onDestroy()
    }
}
