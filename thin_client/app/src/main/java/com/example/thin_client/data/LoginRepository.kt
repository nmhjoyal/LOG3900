package com.example.thin_client.data

import com.example.thin_client.data.model.LoggedInUser
import com.example.thin_client.server.connect.ServerConnect
import com.github.nkzawa.socketio.client.Socket


/**
 * Class that requests authentication and user information from the remote data source and
 * maintains an in-memory cache of login status and user credentials information.
 */

class LoginRepository(val dataSource: LoginDataSource) {

    // in-memory cache of the loggedInUser object
    var user: LoggedInUser? = null
        private set

    val isLoggedIn: Boolean
        get() = user != null

    init {
        // If user credentials will be cached in local storage, it is recommended it be encrypted
        // @see https://developer.android.com/training/articles/keystore
        user = null
    }

    fun logout() {
        user = null
        dataSource.logout()
    }

    fun login(ipAddress: String, port: String, username: String): Socket {
        // handle login
        val result: Socket = ServerConnect().connect(ipAddress, port)

        result.on(Socket.EVENT_CONNECT, ({
            setLoggedInUser(LoggedInUser(username, username))
        }))
        return result
    }

    private fun setLoggedInUser(loggedInUser: LoggedInUser) {
        this.user = loggedInUser
        // If user credentials will be cached in local storage, it is recommended it be encrypted
        // @see https://developer.android.com/training/articles/keystore
    }
}
