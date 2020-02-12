package com.example.thin_client.ui.createUser

import android.content.Intent
import android.os.Bundle
import android.view.MenuItem
import android.widget.Button
import android.widget.EditText
import androidx.appcompat.app.AppCompatActivity
import androidx.lifecycle.Observer
import androidx.lifecycle.ViewModelProviders
import com.example.thin_client.R
import com.example.thin_client.ui.login.afterTextChanged


class CreateUserActivity : AppCompatActivity() {

    private lateinit var createUserModel: CreateUserModel

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_createuser)

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
            createUserModel.userDataChanged(username.text.toString(), password.text.toString(),
                confirmPassword.text.toString())
        }

        password.afterTextChanged {
            createUserModel.userDataChanged(username.text.toString(), password.text.toString(),
                confirmPassword.text.toString())
        }

        confirmPassword.afterTextChanged {
            createUserModel.userDataChanged(username.text.toString(), password.text.toString(),
                confirmPassword.text.toString())
        }
        avatar.setOnClickListener {
            val intent = Intent(Intent.ACTION_PICK)
            intent.type="image/*"
            startActivityForResult(intent,0)
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
}