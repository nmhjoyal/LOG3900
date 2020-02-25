package com.example.thin_client.ui.createUser

data class CreateUserForm (val firstNameError: Int? = null,
                           val lastNameError: Int? = null,
                           val usernameError: Int? = null,
                           val passwordError: Int? = null,
                           val passwordConfirmError: Int? = null,
                           val isDataValid: Boolean = false)