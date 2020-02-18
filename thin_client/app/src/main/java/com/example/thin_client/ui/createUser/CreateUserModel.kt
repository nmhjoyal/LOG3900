package com.example.thin_client.ui.createUser

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import com.example.thin_client.R

class CreateUserModel : ViewModel() {
    private val _createUserForm = MutableLiveData<CreateUserForm>()
    val createUserForm: LiveData<CreateUserForm> = _createUserForm

    fun userDataChanged(username: String, password: String, confirmPassword: String) {
        if (!isUsernameValid(username)) {
            _createUserForm.value = CreateUserForm(usernameError = R.string.invalid_username)
        } else if (!isPasswordValid(password)) {
            _createUserForm.value = CreateUserForm(passwordError = R.string.invalid_password)
        } else if (!isConfirmPasswordValid(password, confirmPassword)) {
            _createUserForm.value = CreateUserForm(passwordConfirmError = R.string.invalid_password_confirm)
        } else {
            _createUserForm.value = CreateUserForm(isDataValid = true)
        }
    }

    private fun isUsernameValid(username: String): Boolean {
        if (username.length < 4) {
            _createUserForm.value = CreateUserForm(usernameError = R.string.invalid_username)
            return false
        }
        return true
    }

    private fun isPasswordValid(password: String): Boolean {
        if (password.length < 5) {
            _createUserForm.value = CreateUserForm(passwordError = R.string.invalid_password)
            return false
        }
        return true
    }

    private fun isConfirmPasswordValid(password: String, confirmPassword: String): Boolean {
        return password == confirmPassword
    }

}