package com.example.thin_client.data


data class SignedInResponse(
    val signed_in: Boolean,
    val log_message: String
)