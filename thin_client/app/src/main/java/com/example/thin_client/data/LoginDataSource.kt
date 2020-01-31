package com.example.thin_client.data

import com.example.thin_client.server.connect.ServerConnect
import com.github.nkzawa.socketio.client.Socket

/**
 * Class that handles authentication w/ login credentials and retrieves user information.
 */
class LoginDataSource {

    private var connection: ServerConnect = ServerConnect()

    fun login(ipAddress: String, port: String): Socket {
        return connection.connect(ipAddress, port)
    }

    fun logout() {
        connection.disconnect()
    }
}

