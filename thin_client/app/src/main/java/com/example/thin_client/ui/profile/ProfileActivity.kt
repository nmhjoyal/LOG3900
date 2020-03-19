package com.example.thin_client.ui.profile

import OkHttpRequest
import android.content.Context
import android.content.Intent
import android.content.SharedPreferences
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.View
import android.view.WindowManager
import android.widget.ImageButton
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import androidx.core.view.isVisible
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProviders
import com.example.thin_client.R
import com.example.thin_client.data.AvatarID
import com.example.thin_client.data.Feedback
import com.example.thin_client.data.app_preferences.PreferenceHandler
import com.example.thin_client.data.app_preferences.Preferences
import com.example.thin_client.data.lifecycle.LoginState
import com.example.thin_client.data.model.PrivateProfile
import com.example.thin_client.data.model.User
import com.example.thin_client.data.rooms.RoomManager
import com.example.thin_client.data.server.HTTPRequest
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.data.setAvatar
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.Lobby
import com.example.thin_client.ui.createUser.CreateUserModel
import com.example.thin_client.ui.createUser.CreateUserModelFactory
import com.example.thin_client.ui.login.afterTextChanged
import com.github.nkzawa.socketio.client.Socket
import com.google.gson.Gson
import kotlinx.android.synthetic.main.activity_profile.*
import okhttp3.Call
import java.io.IOException

class ProfileActivity : AppCompatActivity() {

    private lateinit var privateProfile: PrivateProfile
    private lateinit var createUserModel: CreateUserModel
    private lateinit var selectedAvatar: AvatarID
    private lateinit var prefs: SharedPreferences

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_profile)

        privateProfile = PreferenceHandler(this).getPrivateProfile()
        prefs = this.getSharedPreferences(Preferences.USER_PREFS, Context.MODE_PRIVATE)

        username.text = privateProfile.username
        selectedAvatar = AvatarID.valueOf(privateProfile.avatar.capitalize())

        resetProfile()
        loading.visibility = View.GONE

        createUserModel = ViewModelProviders.of(this, CreateUserModelFactory())
            .get(CreateUserModel::class.java)

        createUserModel.createUserForm.observe(this@ProfileActivity, Observer {
            val createUserState = it ?: return@Observer

            // disable login button unless all fields are correctly filled
            save_button.isEnabled = createUserState.isDataValid

            if (createUserState.firstNameError != null) {
                firstName.error = getString(createUserState.firstNameError)
            }
            if (createUserState.lastNameError != null) {
                lastName.error = getString(createUserState.lastNameError)
            }
            if (createUserState.passwordError != null) {
                password.error = getString(createUserState.passwordError)
            }
            if (createUserState.passwordConfirmError != null) {
                confirmPass.error = getString(createUserState.passwordConfirmError)
            }
        })

        firstName.afterTextChanged {
            createUserModel.userDataChanged(
                firstName.text.toString(), lastName.text.toString(),
                username.text.toString(), password.text.toString(),
                confirmPass.text.toString()
            )
        }

        lastName.afterTextChanged {
            createUserModel.userDataChanged(
                firstName.text.toString(), lastName.text.toString(),
                username.text.toString(), password.text.toString(),
                confirmPass.text.toString()
            )
        }

        password.afterTextChanged {
            createUserModel.userDataChanged(
                firstName.text.toString(), lastName.text.toString(),
                username.text.toString(), password.text.toString(),
                confirmPass.text.toString()
            )
        }

        confirmPass.afterTextChanged {
            createUserModel.userDataChanged(
                firstName.text.toString(), lastName.text.toString(),
                username.text.toString(), password.text.toString(),
                confirmPass.text.toString()
            )
        }

        user_stats_button.setOnClickListener(({
            if (user_stats_table.isVisible) {
                user_stats_table.visibility = View.GONE
            } else {
                user_stats_table.visibility = View.VISIBLE
            }
        }))

        game_stats_button.setOnClickListener(({
            if (game_stats_table.isVisible) {
                game_stats_table.visibility = View.GONE
            } else {
                game_stats_table.visibility = View.VISIBLE
            }
        }))

        delete_profile_button.setOnClickListener(({
            val alertDialog = AlertDialog.Builder(this)
            alertDialog.setTitle(R.string.delete_profile)
                .setMessage(R.string.delete_profile_warning)
                .setPositiveButton(R.string.delete_profile) { _, _ -> deleteProfile() }
                .setNegativeButton(R.string.cancel) { _, _ -> }

            val dialog = alertDialog.create()
            dialog.window!!.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE)
            dialog.show()
        }))

        save_button.setOnClickListener(({
            loading.visibility = View.VISIBLE
            val updatedProfile = PrivateProfile(username.text.toString(),
                firstName.text.toString(), lastName.text.toString(),
                password.text.toString(), selectedAvatar.name,
                ArrayList(RoomManager.roomsJoined.keys))
            SocketHandler.updateProfile(updatedProfile)
        }))

        undo_button.setOnClickListener(({
            resetProfile()
        }))

        avocado_avatar.setOnClickListener(({
            val alertDialog = AlertDialog.Builder(this)
            val dialogView = layoutInflater.inflate(R.layout.dialog_change_avatar, null)
            alertDialog.setView(dialogView)
            initView(dialogView)
            alertDialog.setTitle(R.string.change_avatar)
                .setPositiveButton(R.string.ok) { _, _ -> setAvatar(avocado_avatar, selectedAvatar)}
                .setNegativeButton(R.string.cancel) { _, _ -> }

            val dialog = alertDialog.create()
            dialog.window!!.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE)
            dialog.show()
        }))
    }

    override fun onStart() {
        super.onStart()
        setupSocket()
    }

    override fun onStop() {
        super.onStop()
        turnOffSocketEvents()
    }

    private fun initView(view: View) {
        resetAvatarSelection(view)
        when (selectedAvatar) {
            AvatarID.PEAR -> view.findViewById<ImageButton>(R.id.pear_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
            AvatarID.CHERRY -> view.findViewById<ImageButton>(R.id.cherry_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
            AvatarID.LEMON -> view.findViewById<ImageButton>(R.id.lemon_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
            AvatarID.APPLE -> view.findViewById<ImageButton>(R.id.apple_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
            AvatarID.PINEAPPLE -> view.findViewById<ImageButton>(R.id.pineapple_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
            AvatarID.ORANGE -> view.findViewById<ImageButton>(R.id.orange_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
            AvatarID.KIWI -> view.findViewById<ImageButton>(R.id.kiwi_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
            AvatarID.GRAPE -> view.findViewById<ImageButton>(R.id.grape_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
            AvatarID.WATERMELON -> view.findViewById<ImageButton>(R.id.watermelon_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
            AvatarID.STRAWBERRY -> view.findViewById<ImageButton>(R.id.strawberry_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
            AvatarID.BANANA -> view.findViewById<ImageButton>(R.id.banana_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
            AvatarID.AVOCADO -> view.findViewById<ImageButton>(R.id.avocado_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
        }
        setupAvatarButtons(view)
    }

    private fun setupAvatarButtons(view: View) {
        view.findViewById<ImageButton>(R.id.avocado_avatar).setOnClickListener(({
            selectedAvatar = AvatarID.AVOCADO
            resetAvatarSelection(view)
            view.findViewById<ImageButton>(R.id.avocado_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
        }))
        view.findViewById<ImageButton>(R.id.banana_avatar).setOnClickListener(({
            selectedAvatar = AvatarID.BANANA
            resetAvatarSelection(view)
            view.findViewById<ImageButton>(R.id.banana_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
        }))
        view.findViewById<ImageButton>(R.id.strawberry_avatar).setOnClickListener(({
            selectedAvatar = AvatarID.STRAWBERRY
            resetAvatarSelection(view)
            view.findViewById<ImageButton>(R.id.strawberry_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
        }))
        view.findViewById<ImageButton>(R.id.watermelon_avatar).setOnClickListener(({
            selectedAvatar = AvatarID.WATERMELON
            resetAvatarSelection(view)
            view.findViewById<ImageButton>(R.id.watermelon_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
        }))
        view.findViewById<ImageButton>(R.id.grape_avatar).setOnClickListener(({
            selectedAvatar = AvatarID.GRAPE
            resetAvatarSelection(view)
            view.findViewById<ImageButton>(R.id.grape_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
        }))
        view.findViewById<ImageButton>(R.id.kiwi_avatar).setOnClickListener(({
            selectedAvatar = AvatarID.KIWI
            resetAvatarSelection(view)
            view.findViewById<ImageButton>(R.id.kiwi_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
        }))
        view.findViewById<ImageButton>(R.id.orange_avatar).setOnClickListener(({
            selectedAvatar = AvatarID.ORANGE
            resetAvatarSelection(view)
            view.findViewById<ImageButton>(R.id.orange_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
        }))
        view.findViewById<ImageButton>(R.id.pineapple_avatar).setOnClickListener(({
            selectedAvatar = AvatarID.PINEAPPLE
            resetAvatarSelection(view)
            view.findViewById<ImageButton>(R.id.pineapple_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
        }))
        view.findViewById<ImageButton>(R.id.apple_avatar).setOnClickListener(({
            selectedAvatar = AvatarID.APPLE
            resetAvatarSelection(view)
            view.findViewById<ImageButton>(R.id.apple_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
        }))
        view.findViewById<ImageButton>(R.id.lemon_avatar).setOnClickListener(({
            selectedAvatar = AvatarID.LEMON
            resetAvatarSelection(view)
            view.findViewById<ImageButton>(R.id.lemon_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
        }))
        view.findViewById<ImageButton>(R.id.cherry_avatar).setOnClickListener(({
            selectedAvatar = AvatarID.CHERRY
            resetAvatarSelection(view)
            view.findViewById<ImageButton>(R.id.cherry_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
        }))
        view.findViewById<ImageButton>(R.id.pear_avatar).setOnClickListener(({
            selectedAvatar = AvatarID.PEAR
            resetAvatarSelection(view)
            view.findViewById<ImageButton>(R.id.pear_avatar).setBackgroundResource(R.drawable.circle_primary_dark)
        }))
    }

    private fun resetAvatarSelection(view: View) {
        view.findViewById<ImageButton>(R.id.avocado_avatar).setBackgroundResource(R.drawable.avatar_background)
        view.findViewById<ImageButton>(R.id.banana_avatar).setBackgroundResource(R.drawable.avatar_background)
        view.findViewById<ImageButton>(R.id.strawberry_avatar).setBackgroundResource(R.drawable.avatar_background)
        view.findViewById<ImageButton>(R.id.watermelon_avatar).setBackgroundResource(R.drawable.avatar_background)
        view.findViewById<ImageButton>(R.id.grape_avatar).setBackgroundResource(R.drawable.avatar_background)
        view.findViewById<ImageButton>(R.id.kiwi_avatar).setBackgroundResource(R.drawable.avatar_background)
        view.findViewById<ImageButton>(R.id.orange_avatar).setBackgroundResource(R.drawable.avatar_background)
        view.findViewById<ImageButton>(R.id.pineapple_avatar).setBackgroundResource(R.drawable.avatar_background)
        view.findViewById<ImageButton>(R.id.apple_avatar).setBackgroundResource(R.drawable.avatar_background)
        view.findViewById<ImageButton>(R.id.lemon_avatar).setBackgroundResource(R.drawable.avatar_background)
        view.findViewById<ImageButton>(R.id.cherry_avatar).setBackgroundResource(R.drawable.avatar_background)
        view.findViewById<ImageButton>(R.id.pear_avatar).setBackgroundResource(R.drawable.avatar_background)
    }

    override fun onBackPressed() {
        // Disable native back
    }

    private fun deleteProfile() {
        val httpClient = OkHttpRequest(okhttp3.OkHttpClient())
        httpClient.DELETE(HTTPRequest.BASE_URL + HTTPRequest.URL_PROFILE + username.text.toString(), object: okhttp3.Callback {
            //N'entre pas dans le on failure
            override fun onFailure(call: Call, e: IOException) {
            }

            override fun onResponse(call: Call, response: okhttp3.Response) {
                val responseData = response.body?.charStream()
                val feedback = Gson().fromJson(responseData, Feedback::class.java)
                runOnUiThread(({
                    if (feedback.status) {
                        PreferenceHandler(applicationContext).resetUserPrefs()
                        val intent = Intent(applicationContext, Lobby::class.java)
                        startActivity(intent)
                        finish()
                    } else {
                        Handler(Looper.getMainLooper()).post(Runnable {
                            Toast.makeText(
                                applicationContext,
                                feedback.log_message,
                                Toast.LENGTH_SHORT
                            ).show()
                        })
                    }
                }))
            }
        })
    }

    private fun resetProfile() {
        firstName.setText(privateProfile.firstname)
        lastName.setText(privateProfile.lastname)
        password.setText(privateProfile.password)
        confirmPass.setText(privateProfile.password)

        val avatarID = AvatarID.valueOf(privateProfile.avatar)
        setAvatar(avocado_avatar, avatarID)
    }

    private fun setupSocket() {
        if (!SocketHandler.isConnected()) {
            SocketHandler.connect()
        }

        setupSocketEvents()

        when (SocketHandler.getLoginState(prefs)) {
            LoginState.FIRST_LOGIN -> {}
            LoginState.LOGIN_WITH_EXISTING -> {
                val user = PreferenceHandler(applicationContext).getUser()
                SocketHandler.login(User(user.username, user.password))
                SocketHandler.isLoggedIn = true
            }
            LoginState.LOGGED_IN -> {}

        }
    }

    private fun setupSocketEvents() {
        SocketHandler.socket!!.on(SocketEvent.PROFILE_UPDATED, ({ data ->
            val feedback = Gson().fromJson(data.first().toString(),Feedback::class.java)
            Handler(Looper.getMainLooper()).post(Runnable {
                loading.visibility = View.GONE
                if (feedback.status) {
                    privateProfile = PrivateProfile(username.text.toString(),
                        firstName.text.toString(), lastName.text.toString(),
                        password.text.toString(), selectedAvatar.name,
                        ArrayList(RoomManager.roomsJoined.keys))
                    PreferenceHandler(applicationContext).setUser(privateProfile)
                }
                Toast.makeText(
                    applicationContext,
                    feedback.log_message,
                    Toast.LENGTH_SHORT
                ).show()
            })
        }))
    }

    private fun turnOffSocketEvents() {
        SocketHandler.socket!!.off(SocketEvent.PROFILE_UPDATED)
    }
}