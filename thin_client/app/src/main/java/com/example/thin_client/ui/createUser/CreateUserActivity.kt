package com.example.thin_client.ui.createUser

import OkHttpRequest
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.MenuItem
import android.view.View
import android.widget.Button
import android.widget.EditText
import android.widget.ProgressBar
import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProviders
import com.example.thin_client.R
import com.example.thin_client.data.AvatarID
import com.example.thin_client.data.Feedback
import com.example.thin_client.data.app_preferences.PreferenceHandler
import com.example.thin_client.data.model.PrivateProfile
import com.example.thin_client.data.server.HTTPRequest
import com.example.thin_client.ui.login.afterTextChanged
import com.google.gson.Gson
import kotlinx.android.synthetic.main.activity_createuser.*
import okhttp3.Call
import java.io.IOException


class CreateUserActivity : AppCompatActivity() {

    private lateinit var createUserModel: CreateUserModel

    var selectedAvatar: AvatarID = AvatarID.AVOCADO


    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_createuser)
        loading.visibility = View.INVISIBLE

        val firstName = findViewById<EditText>(R.id.firstName)
        val lastName = findViewById<EditText>(R.id.lastName)
        val username = findViewById<EditText>(R.id.username)
        val password = findViewById<EditText>(R.id.password)
        val confirmPassword = findViewById<EditText>(R.id.confirmPass)
        val create = findViewById<Button>(R.id.create)

        setupAvatarButtons()

        createUserModel = ViewModelProviders.of(this, CreateUserModelFactory())
            .get(CreateUserModel::class.java)

        createUserModel.createUserForm.observe(this@CreateUserActivity, Observer {
            val createUserState = it ?: return@Observer

            // disable login button unless all fields are correctly filled
            create.isEnabled = createUserState.isDataValid

            if (createUserState.firstNameError != null) {
                firstName.error = getString(createUserState.firstNameError)
            }
            if (createUserState.lastNameError != null) {
                lastName.error = getString(createUserState.lastNameError)
            }
            if (createUserState.usernameError != null) {
                username.error = getString(createUserState.usernameError)
            }
            if (createUserState.passwordError != null) {
                password.error = getString(createUserState.passwordError)
            }
            if (createUserState.passwordConfirmError != null) {
                confirmPassword.error = getString(createUserState.passwordConfirmError)
            }
        })

        firstName.afterTextChanged {
            createUserModel.userDataChanged(
                firstName.text.toString(), lastName.text.toString(),
                username.text.toString(), password.text.toString(),
                confirmPassword.text.toString()
            )
        }

        lastName.afterTextChanged {
            createUserModel.userDataChanged(
                firstName.text.toString(), lastName.text.toString(),
                username.text.toString(), password.text.toString(),
                confirmPassword.text.toString()
            )
        }

        username.afterTextChanged {
            createUserModel.userDataChanged(
                firstName.text.toString(), lastName.text.toString(),
                username.text.toString(), password.text.toString(),
                confirmPassword.text.toString()
            )
        }

        password.afterTextChanged {
            createUserModel.userDataChanged(
                firstName.text.toString(), lastName.text.toString(),
                username.text.toString(), password.text.toString(),
                confirmPassword.text.toString()
            )
        }

        confirmPassword.afterTextChanged {
            createUserModel.userDataChanged(
                firstName.text.toString(), lastName.text.toString(),
                username.text.toString(), password.text.toString(),
                confirmPassword.text.toString()
            )
        }

        create.setOnClickListener {
            loading.visibility = ProgressBar.VISIBLE
            create.isEnabled = false
            val httpClient = OkHttpRequest(okhttp3.OkHttpClient())
            val privateProfile = PrivateProfile(username.text.toString(), firstName.text.toString(),
                lastName.text.toString(), password.text.toString(), selectedAvatar.name, arrayListOf())
            val gson = Gson()
            val gsonProfile = gson.toJson(privateProfile)
            httpClient.POST(HTTPRequest.BASE_URL + HTTPRequest.URL_CREATE, gsonProfile.toString(),
                object: okhttp3.Callback {
                    //N'entre pas dans le on failure
                    override fun onFailure(call: Call, e: IOException) {
                        runOnUiThread(({
                            loading.visibility = ProgressBar.GONE
                            create.isEnabled = true
                        }))
                    }

                    override fun onResponse(call: Call, response: okhttp3.Response) {
                        val responseData = response.body?.charStream()
                        val feedback = gson.fromJson(responseData, Feedback::class.java)
                        runOnUiThread(({
                            loading.visibility = ProgressBar.GONE
                            create.isEnabled = true
                            if(feedback.status){
                                PreferenceHandler(applicationContext).setUser(privateProfile)
                                finish()
                            } else{
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
                }
            )
        }
    }

    /* Icons source https://www.flaticon.com/authors/freepik author https://www.flaticon.com/ */
    private fun setupAvatarButtons() {
        avocado_avatar.setOnClickListener(({
            resetAvatarSelection()
            avocado_avatar.setBackgroundResource(R.drawable.circle_primary_dark)
            selectedAvatar = AvatarID.AVOCADO
        }))
        banana_avatar.setOnClickListener(({
            resetAvatarSelection()
            banana_avatar.setBackgroundResource(R.drawable.circle_primary_dark)
            selectedAvatar = AvatarID.BANANA
        }))
        strawberry_avatar.setOnClickListener(({
            resetAvatarSelection()
            strawberry_avatar.setBackgroundResource(R.drawable.circle_primary_dark)
            selectedAvatar = AvatarID.STRAWBERRY
        }))
        watermelon_avatar.setOnClickListener(({
            resetAvatarSelection()
            watermelon_avatar.setBackgroundResource(R.drawable.circle_primary_dark)
            selectedAvatar = AvatarID.WATERMELON
        }))
        grape_avatar.setOnClickListener(({
            resetAvatarSelection()
            grape_avatar.setBackgroundResource(R.drawable.circle_primary_dark)
            selectedAvatar = AvatarID.GRAPE
        }))
        kiwi_avatar.setOnClickListener(({
            resetAvatarSelection()
            kiwi_avatar.setBackgroundResource(R.drawable.circle_primary_dark)
            selectedAvatar = AvatarID.KIWI
        }))
        orange_avatar.setOnClickListener(({
            resetAvatarSelection()
            orange_avatar.setBackgroundResource(R.drawable.circle_primary_dark)
            selectedAvatar = AvatarID.ORANGE
        }))
        pineapple_avatar.setOnClickListener(({
            resetAvatarSelection()
            pineapple_avatar.setBackgroundResource(R.drawable.circle_primary_dark)
            selectedAvatar = AvatarID.PINEAPPLE
        }))
        apple_avatar.setOnClickListener(({
            resetAvatarSelection()
            apple_avatar.setBackgroundResource(R.drawable.circle_primary_dark)
            selectedAvatar = AvatarID.APPLE
        }))
        lemon_avatar.setOnClickListener(({
            resetAvatarSelection()
            lemon_avatar.setBackgroundResource(R.drawable.circle_primary_dark)
            selectedAvatar = AvatarID.LEMON
        }))
        cherry_avatar.setOnClickListener(({
            resetAvatarSelection()
            cherry_avatar.setBackgroundResource(R.drawable.circle_primary_dark)
            selectedAvatar = AvatarID.CHERRY
        }))
        pear_avatar.setOnClickListener(({
            resetAvatarSelection()
            pear_avatar.setBackgroundResource(R.drawable.circle_primary_dark)
            selectedAvatar = AvatarID.PEAR
        }))
    }

    private fun resetAvatarSelection() {
        avocado_avatar.setBackgroundResource(R.drawable.avatar_background)
        banana_avatar.setBackgroundResource(R.drawable.avatar_background)
        strawberry_avatar.setBackgroundResource(R.drawable.avatar_background)
        watermelon_avatar.setBackgroundResource(R.drawable.avatar_background)
        grape_avatar.setBackgroundResource(R.drawable.avatar_background)
        kiwi_avatar.setBackgroundResource(R.drawable.avatar_background)
        orange_avatar.setBackgroundResource(R.drawable.avatar_background)
        pineapple_avatar.setBackgroundResource(R.drawable.avatar_background)
        apple_avatar.setBackgroundResource(R.drawable.avatar_background)
        lemon_avatar.setBackgroundResource(R.drawable.avatar_background)
        cherry_avatar.setBackgroundResource(R.drawable.avatar_background)
        pear_avatar.setBackgroundResource(R.drawable.avatar_background)
    }

    override fun onBackPressed() {
        // Disable native back
    }

}