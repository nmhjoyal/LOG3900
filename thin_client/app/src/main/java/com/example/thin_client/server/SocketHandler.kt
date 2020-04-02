package com.example.thin_client.server

import android.content.SharedPreferences
import com.example.thin_client.data.ClientMessage
import com.example.thin_client.data.app_preferences.Preferences
import com.example.thin_client.data.drawing.Stroke
import com.example.thin_client.data.drawing.StylusPoint
import com.example.thin_client.data.game.CreateMatch
import com.example.thin_client.data.lifecycle.LoginState
import com.example.thin_client.data.model.PrivateProfile
import com.example.thin_client.data.model.User
import com.example.thin_client.data.rooms.CreateRoom
import com.example.thin_client.data.rooms.Invitation
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
        val opts = IO.Options()
        opts.reconnection = false
        socket = IO.socket(HTTPRequest.BASE_URL)
        return socket!!.connect()
    }

    fun isConnected(): Boolean {
        return socket !== null
    }

    fun getLoginState(prefs: SharedPreferences): LoginState {
        if (!prefs.getBoolean(Preferences.LOGGED_IN_KEY, false)) {
            return LoginState.FIRST_LOGIN
        } else if (!isLoggedIn){
            return LoginState.LOGIN_WITH_EXISTING
        } else {
            return LoginState.LOGGED_IN
        }
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
        if(socket == null) {
            connect()
        }
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

    fun searchRooms() {
        socket!!.emit(SocketEvent.GET_ROOMS)
    }

    fun updateProfile(privateProfile: PrivateProfile) {
        val gson = Gson()
        val args = gson.toJson(privateProfile)
        socket!!.emit(SocketEvent.UPDATE_PROFILE, args)
    }

    fun startStroke(drawPoint: Stroke) {
        val args = Gson().toJson(drawPoint)
        socket!!.emit(SocketEvent.STROKE, args)
    }

    fun startEraseStroke() {
        socket!!.emit(SocketEvent.ERASE_STROKE)
    }

    fun startErasePoint() {
        socket!!.emit(SocketEvent.ERASE_POINT)
    }

    fun point(drawPoint: StylusPoint) {
        val args = Gson().toJson(drawPoint)
        socket!!.emit(SocketEvent.POINT, args)
    }

//    fun sendScreenResolution(screen: ScreenResolution) {
//        val args = Gson().toJson(screen)
//        socket!!.emit(SocketEvent.SEND_SCREEN_RESOLUTION, args)
//    }

    fun sendInvite(invite: Invitation) {
        val args = Gson().toJson(invite)
        socket!!.emit(SocketEvent.SEND_INVITE, args)
    }

    fun createMatch(match: CreateMatch) {
        val args = Gson().toJson(match)
        socket!!.emit(SocketEvent.CREATE_MATCH, args)
    }

    fun startMatch() {
        socket!!.emit(SocketEvent.START_MATCH)
    }

    fun clearDrawing() {
        socket!!.emit(SocketEvent.CLEAR)
    }

    /*fun updateMatches(matchInfos:ArrayList<MatchInfos>) {
        val gson = Gson()
        val args = gson.toJson(matchInfos)
        socket!!.emit(SocketEvent.UPDATE_MATCHES, args)
    }*/

    fun searchMatches() {
        socket!!.emit(SocketEvent.GET_MATCHES)
    }

    fun searchPlayers() {
        socket!!.emit(SocketEvent.GET_PLAYERS)
    }

    fun joinMatch(matchId:String){
        val args = Gson().toJson(matchId)
        socket!!.emit(SocketEvent.JOIN_MATCH,args)
    }

    fun leaveMatch() {
        socket!!.emit(SocketEvent.LEAVE_MATCH)
    }

    fun startTurn(word: String) {
        val args = Gson().toJson(word)
        socket!!.emit(SocketEvent.START_TURN, args)
    }

    fun sendGuess(guess: String) {
        val args = Gson().toJson(guess)
        socket!!.emit(SocketEvent.GUESS, args)
    }

    fun addVirtualPlayer() {
        socket!!.emit(SocketEvent.ADD_VP)
    }

    fun removeVirtualPlayer() {
        socket!!.emit(SocketEvent.REMOVE_VP)
    }


}