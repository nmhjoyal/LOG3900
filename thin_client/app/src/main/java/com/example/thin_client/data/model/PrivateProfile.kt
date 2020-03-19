package com.example.thin_client.data.model

import com.example.thin_client.data.Message

data class PrivateProfile (
    val username: String,
    val firstname : String,
    val lastname : String,
    val password : String,
    val avatar : String /* String for the moment eventually needs to be image */,
    val rooms_joined: ArrayList<String>
)