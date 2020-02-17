package com.example.thin_client.ui.createUser

import android.app.Activity
import android.content.Intent

import android.net.Uri
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.provider.MediaStore
import android.view.MenuItem
import android.widget.Button
import android.widget.EditText

import android.widget.Toast
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProviders
import com.example.thin_client.R
import com.example.thin_client.data.model.User
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.MainActivity
import com.example.thin_client.ui.chatrooms.ChatRoomsFragment
import com.example.thin_client.ui.login.afterTextChanged
import com.github.nkzawa.socketio.client.Socket
import de.hdodenhof.circleimageview.CircleImageView



class CreateUserActivity : AppCompatActivity() {

    private lateinit var createUserModel: CreateUserModel

    var avatarUri: Uri ?=null


    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_createuser)

        val username = findViewById<EditText>(R.id.username)
        val password = findViewById<EditText>(R.id.password)
        var confirmPassword = findViewById<EditText>(R.id.confirmPass)
        val create = findViewById<Button>(R.id.create)
        val avatar = findViewById<Button>(R.id.button_upload_avatar)
        val ipAddress = "10.226.230.137"

        supportActionBar?.setDisplayHomeAsUpEnabled(true)

        createUserModel = ViewModelProviders.of(this, CreateUserModelFactory())
            .get(CreateUserModel::class.java)

        createUserModel.createUserForm.observe(this@CreateUserActivity, Observer {
            val createUserState = it ?: return@Observer

            // disable login button unless both username / password is valid
            create.isEnabled = createUserState.isDataValid

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

        username.afterTextChanged {
            createUserModel.userDataChanged(
                username.text.toString(), password.text.toString(),
                confirmPassword.text.toString()
            )
        }

        password.afterTextChanged {
            createUserModel.userDataChanged(
                username.text.toString(), password.text.toString(),
                confirmPassword.text.toString()
            )
        }

        confirmPassword.afterTextChanged {
            createUserModel.userDataChanged(
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
            val socket = SocketHandler.connect(ipAddress)
            socket.on(Socket.EVENT_CONNECT, ({
                SocketHandler.login(User(username.text.toString(), password.text.toString()))
            }))
                .on(Socket.EVENT_CONNECT_ERROR, ({
                    Handler(Looper.getMainLooper()).post(Runnable {
                        Toast.makeText(applicationContext, "Unable to connect", Toast.LENGTH_SHORT).show()
                    })
                    SocketHandler.disconnect()
                }))
                .on("user_signed_in", ({ data ->
                    if (data.last().toString().toBoolean()) {
                        val intent = Intent(applicationContext, MainActivity::class.java)
                        startActivity(intent)
                        finish()
                    } else {
                        Handler(Looper.getMainLooper()).post(Runnable {
                            Toast.makeText(applicationContext, "Username already taken", Toast.LENGTH_SHORT).show()
                        })
                        SocketHandler.disconnect()
                    }
                }))
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