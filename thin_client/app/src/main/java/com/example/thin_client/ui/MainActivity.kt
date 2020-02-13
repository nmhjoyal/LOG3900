package com.example.thin_client.ui

import android.content.Context
import android.content.Intent
import androidx.appcompat.app.AppCompatActivity
import android.os.Bundle
import android.view.Menu
import android.view.MenuItem
import com.example.thin_client.R
import com.example.thin_client.data.Preferences
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.login.LoginActivity

class MainActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_main)

        val prefs = this.getSharedPreferences(Preferences.USER_PREFS, Context.MODE_PRIVATE)
        val isLoggedIn = prefs.getBoolean(Preferences.LOGGED_IN_KEY, false)

        

        if (!isLoggedIn) {
            val intent = Intent(applicationContext, LoginActivity::class.java)
            startActivity(intent)
        }
    }

    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        when(item.itemId) {
            R.id.menu_sign_out -> {
                SocketHandler.logout()
                val prefs = this.getSharedPreferences(Preferences.USER_PREFS, Context.MODE_PRIVATE)
                prefs.edit().putBoolean(Preferences.LOGGED_IN_KEY, false).apply()
                val intent = Intent(applicationContext, LoginActivity::class.java)
                startActivity(intent)
            }
        }
        return super.onOptionsItemSelected(item)
    }

    override fun onCreateOptionsMenu(menu: Menu): Boolean{
        menuInflater.inflate(R.menu.nav_menu, menu)
        return super.onCreateOptionsMenu(menu)
    }
}
