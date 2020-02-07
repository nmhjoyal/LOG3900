package com.example.thin_client.ui.login

/**
 * Data validation state of the login form.
 */
data class LoginFormState (val usernameError: Int? = null,
                           val ipAddressError: Int? = null,
                           val isDataValid: Boolean = false)
