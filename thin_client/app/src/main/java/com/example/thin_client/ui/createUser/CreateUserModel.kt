package com.example.thin_client.ui.createUser

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import com.example.thin_client.R

class CreateUserModel : ViewModel() {
    private val _createUserForm = MutableLiveData<CreateUserForm>()
    val createUserForm: LiveData<CreateUserForm> = _createUserForm

    fun userDataChanged(firstname: String, lastname: String, username:String,
                        password: String, confirmPassword: String) {
        if (!isFirstNameValid(firstname)) {
            _createUserForm.value = CreateUserForm(firstNameError = R.string.invalid_name)
        } else if (!isLastNameValid(lastname)) {
            _createUserForm.value = CreateUserForm(lastNameError = R.string.invalid_name)
        } else if (!isUsernameValid(username)) {
            _createUserForm.value = CreateUserForm(usernameError = R.string.invalid_username)
        } else if (!isPasswordValid(password)){
            _createUserForm.value = CreateUserForm(passwordError = R.string.invalid_password)
        } else if (!isConfirmPasswordValid(password, confirmPassword)) {
            _createUserForm.value = CreateUserForm(passwordConfirmError = R.string.invalid_password_confirm)
        } else {
            _createUserForm.value = CreateUserForm(isDataValid = true)
        }
    }

    private fun isFirstNameValid(firstname: String): Boolean {
        return firstname.isNotBlank()
    }

    private fun isLastNameValid(lastname: String): Boolean {
        return lastname.isNotBlank()
    }

    private fun isUsernameValid(username: String): Boolean {
        if (username.length < 4 || username.length > 20) {
            _createUserForm.value = CreateUserForm(usernameError = R.string.invalid_username)
            return false
        }
        return true
    }

    private fun isPasswordValid(password: String): Boolean {
        if (password.length < 8 || password.length > 20) {
            _createUserForm.value = CreateUserForm(passwordError = R.string.invalid_password)
            return false
        }
        return true
    }

    private fun isConfirmPasswordValid(password: String, confirmPassword: String): Boolean {
        return password == confirmPassword
    }

}