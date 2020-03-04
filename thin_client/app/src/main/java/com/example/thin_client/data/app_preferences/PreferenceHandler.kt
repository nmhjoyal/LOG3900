package com.example.thin_client.data.app_preferences

import android.content.Context
import com.example.thin_client.data.model.PrivateProfile
import com.example.thin_client.data.model.PublicProfile
import com.example.thin_client.data.model.User
import com.example.thin_client.data.rooms.RoomManager

class PreferenceHandler(context: Context) {
    private val mUserPrefs = context.getSharedPreferences(Preferences.USER_PREFS, Context.MODE_PRIVATE)

    fun resetUserPrefs() {
        mUserPrefs.edit()
            .putBoolean(Preferences.LOGGED_IN_KEY, false)
            .putString(Preferences.USERNAME, "")
            .putString(Preferences.PASSWORD, "")
            .putString(Preferences.FIRST_NAME, "")
            .putString(Preferences.LAST_NAME, "")
            .putString(Preferences.AVATAR, "")
            .apply()
    }

    fun setUser(privateProfile: PrivateProfile) {
        mUserPrefs.edit()
            .putBoolean(Preferences.LOGGED_IN_KEY, true)
            .putString(Preferences.USERNAME, privateProfile.username)
            .putString(Preferences.PASSWORD, privateProfile.password)
            .putString(Preferences.AVATAR, privateProfile.avatar)
            .putString(Preferences.FIRST_NAME, privateProfile.firstname)
            .putString(Preferences.LAST_NAME, privateProfile.lastname)
            .apply()
    }

    fun getUser(): User {
        return User(mUserPrefs.getString(Preferences.USERNAME, "")!!,
            mUserPrefs.getString(Preferences.PASSWORD, "")!!)
    }

    fun getPublicProfile(): PublicProfile {
        return PublicProfile(mUserPrefs.getString(Preferences.USERNAME, "")!!,
            mUserPrefs.getString(Preferences.AVATAR, "")!!)
    }

    fun getPrivateProfile(): PrivateProfile {
        return PrivateProfile(mUserPrefs.getString(Preferences.USERNAME, "")!!,
            mUserPrefs.getString(Preferences.FIRST_NAME, "")!!,
            mUserPrefs.getString(Preferences.LAST_NAME, "")!!,
            mUserPrefs.getString(Preferences.PASSWORD, "")!!,
            mUserPrefs.getString(Preferences.AVATAR, "BANANA")!!,
            RoomManager.roomsJoined)
    }
}