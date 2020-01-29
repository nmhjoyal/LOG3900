package com.example.thin_client.ui.createUser

import org.junit.Test

import org.junit.Assert.*

class CreateUserModelTest {

    @Test
    fun userDataChanged() {
        val createUserModel = CreateUserModel()
        val user = "yolo"
        val pass = "testpass"
        val confirm = "testpass"
        createUserModel.userDataChanged(user, pass, confirm)
        assert(createUserModel.createUserForm.value!!.isDataValid)
    }
}