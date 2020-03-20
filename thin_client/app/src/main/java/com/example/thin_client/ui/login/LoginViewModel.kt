package com.example.thin_client.ui.login

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel

import com.example.thin_client.R
import java.lang.Integer.parseInt
import java.lang.NumberFormatException

class LoginViewModel : ViewModel() {

    private val _loginForm = MutableLiveData<LoginFormState>()
    val loginFormState: LiveData<LoginFormState> = _loginForm
    private val _loginResult = MutableLiveData<LoginResult>()
    val loginResult: LiveData<LoginResult> = _loginResult



    fun loginDataChanged(ipAddress: String,username: String) {
        if (!isIPAddressValid(ipAddress)) {
            _loginForm.value = LoginFormState(ipAddressError = R.string.invalid_ip)
        } else if (!isUserNameValid(username)) {
            _loginForm.value = LoginFormState(usernameError = R.string.invalid_username)
        } else {
            _loginForm.value = LoginFormState(isDataValid = true)
        }
    }

    private fun isUserNameValid(username: String): Boolean {
        return (username.isNotBlank() && username.length < 21)
    }

    private fun isIPAddressValid(ipAddress: String): Boolean {
        val splitIP = ipAddress.split(".")
        if (splitIP.size == 4) {
            for (strByte in splitIP) {
                try {
                    val byte = parseInt(strByte)
                    if (byte < 0) { return false }
                } catch (e: NumberFormatException) {
                    return false
                }
            }
        } else {
            return false
        }
        return true
    }
}