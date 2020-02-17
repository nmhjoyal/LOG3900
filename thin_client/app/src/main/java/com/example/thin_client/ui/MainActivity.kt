package com.example.thin_client.ui

import android.content.Context
import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import androidx.fragment.app.Fragment
import com.example.thin_client.R
import com.example.thin_client.data.Preferences
import com.example.thin_client.ui.chatrooms.ChatRoomsFragment
import com.example.thin_client.ui.login.LoginActivity

class MainActivity : AppCompatActivity() {

    val manager = supportFragmentManager


    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

       // val prefs = this.getSharedPreferences(Preferences.USER_PREFS, Context.MODE_PRIVATE)
       // val isLoggedIn = prefs.getBoolean(Preferences.LOGGED_IN_KEY, false)

        showChatRoomsFragment()

    }

    fun showChatRoomsFragment(){
        val transaction = manager.beginTransaction()
        val chatroomsFragment = ChatRoomsFragment()
        transaction.replace(R.id.chatrooms_container, chatroomsFragment)
        transaction.addToBackStack(null)
        transaction.commit()
    }

}
