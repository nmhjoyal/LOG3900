package com.example.thin_client.ui.login

import androidx.lifecycle.LiveData
import androidx.lifecycle.MutableLiveData
import androidx.lifecycle.ViewModel
import com.example.thin_client.data.LoginRepository

import com.example.thin_client.R
import com.github.nkzawa.socketio.client.Socket
import java.lang.Integer.parseInt
import java.lang.NumberFormatException

class LoginViewModel(private val loginRepository: LoginRepository) : ViewModel() {

    private val _loginForm = MutableLiveData<LoginFormState>()
    val loginFormState: LiveData<LoginFormState> = _loginForm

    private val _loginResult = MutableLiveData<LoginResult>()
    val loginResult: LiveData<LoginResult> = _loginResult

    fun login(ipAddress: String, port: String, username: String): Socket {
        // can be launched in a separate asynchronous job
        return loginRepository.login(ipAddress, port, username)

    }

    fun loginDataChanged(ipAddress: String, port: String, username: String) {
        if (!isUserNameValid(username)) {
            _loginForm.value = LoginFormState(usernameError = R.string.invalid_username)
        } else if (!isIPAddressValid(ipAddress)) {
            _loginForm.value = LoginFormState(ipAddressError = R.string.invalid_ip)
        } else if (!isPortValid(port)) {
            _loginForm.value = LoginFormState(portError = R.string.invalid_port)
        } else {
            _loginForm.value = LoginFormState(isDataValid = true)
        }
    }

    private fun isUserNameValid(username: String): Boolean {
        return username.isNotBlank()
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

    private fun isPortValid(port: String): Boolean {
        try {
            val portNum = parseInt(port)
            if (portNum < 0) { return false }
        } catch (e: NumberFormatException) {
            return false
        }
        return true
    }
}
