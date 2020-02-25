package com.example.thin_client.ui.createUser

import OkHttpRequest
import android.app.Activity
import android.content.Intent

import android.net.Uri
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.provider.MediaStore
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
import com.example.thin_client.data.model.PrivateProfile
import com.example.thin_client.data.model.User
import com.example.thin_client.data.server.HTTPRequest
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.Lobby
import com.example.thin_client.ui.login.afterTextChanged
import com.github.nkzawa.socketio.client.Socket
import com.google.gson.Gson
import com.squareup.okhttp.Callback
import com.squareup.okhttp.OkHttpClient
import com.squareup.okhttp.Request
import com.squareup.okhttp.Response
import de.hdodenhof.circleimageview.CircleImageView
import kotlinx.android.synthetic.main.activity_createuser.*
import okhttp3.Call
import java.io.IOException


class CreateUserActivity : AppCompatActivity() {

    private lateinit var createUserModel: CreateUserModel

    var avatarUri: Uri ?=null


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
        val avatar = findViewById<Button>(R.id.button_upload_avatar)

        supportActionBar?.setDisplayHomeAsUpEnabled(true)

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

        avatar.setOnClickListener {
            val intent = Intent(Intent.ACTION_PICK)
            intent.type = "image/*"
            startActivityForResult(intent, 0)
        }

        create.setOnClickListener {
            loading.visibility = ProgressBar.VISIBLE
            create.isEnabled = false
            val httpClient = OkHttpRequest(okhttp3.OkHttpClient())
            val gson = Gson()
            val profile = gson.toJson(PrivateProfile(username.text.toString(), firstName.text.toString(),
                lastName.text.toString(), password.text.toString(), "banane"))
            httpClient.POST(HTTPRequest.BASE_URL + HTTPRequest.URL_CREATE, profile.toString(),
                object: okhttp3.Callback {
                    override fun onFailure(call: Call, e: IOException) {
                        runOnUiThread(({
                            loading.visibility = ProgressBar.GONE
                            create.isEnabled = true
                        }))
                    }

                    override fun onResponse(call: Call, response: okhttp3.Response) {
                        runOnUiThread(({
                            loading.visibility = ProgressBar.GONE
                            create.isEnabled = true
                            finish()
                        }))
                    }
                }
            )
        }
    }

    override fun onActivityResult(requestCode: Int, resultCode: Int, data: Intent?) {
        super.onActivityResult(requestCode, resultCode, data)
        if(requestCode == 0 && resultCode == Activity.RESULT_OK && data!=null){
            //proceed and check what the selected image was
            avatarUri = data.data //uri represent where the image is located on the device
            val bitmap = MediaStore.Images.Media.getBitmap(contentResolver,avatarUri)
            val avatarCircleImageView = findViewById<CircleImageView>(R.id.avatar_circleImageView)
            val uploadAvatar = findViewById<Button>(R.id.button_upload_avatar)
            avatarCircleImageView.setImageBitmap(bitmap)
            uploadAvatar.alpha = 0f

        }

    }

    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        when (item.itemId) {
            android.R.id.home -> {
                finish()
                return true
            }
        }
        return super.onOptionsItemSelected(item)
    }

    override fun onBackPressed() {
        // Disable native back
    }

  /*  private fun signup(){
        val uFirstname = firstName.text.toString()
        val uLastname = lastName.text.toString()
        val uUsername = username.text.toString()

        if(uFirstname.isEmpty()|| uLastname.isEmpty() || uUsername.isEmpty()) {
            Toast.makeText(this, "Please fill up all the form fields", Toast.LENGTH_SHORT).show()
            return
        }
        // add here call to add user to the data base
    }*/



    //private fun uploadImageToDB(){}
    //private fun saveUserToDB(){}

}