package com.example.thin_client.server

import com.squareup.otto.Bus


object GlobalBus {
    private var sBus: Bus? = null
    val bus: Bus?
        get() {
            if (sBus == null) sBus = Bus()
            return sBus
        }
}

class LogoutEvent(message: String) {
    var message: String = ""
}