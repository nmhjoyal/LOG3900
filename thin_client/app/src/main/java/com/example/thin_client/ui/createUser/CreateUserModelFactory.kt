package com.example.thin_client.ui.createUser

import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider

class CreateUserModelFactory : ViewModelProvider.Factory {

    @Suppress("UNCHECKED_CAST")
    override fun <T : ViewModel> create(modelClass: Class<T>): T {
        if (modelClass.isAssignableFrom(CreateUserModel::class.java)) {
            return CreateUserModel() as T
        }
        throw IllegalArgumentException("Unknown ViewModel class")
    }
}