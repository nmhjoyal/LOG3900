package com.example.thin_client.ui.createUser

data class CreateUserForm (val usernameError: Int? = null,
                           val passwordError: Int? = null,
                           val passwordConfirmError: Int? = null,
                           val isDataValid: Boolean = false)