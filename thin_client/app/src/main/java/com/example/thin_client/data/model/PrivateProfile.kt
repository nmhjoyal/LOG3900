package com.example.thin_client.data.model

data class PrivateProfile (
    val username: String,
    val firstname : String,
    val lastname : String,
    val password : String,
    val avatar : String /* String for the moment eventually needs to be image */
)