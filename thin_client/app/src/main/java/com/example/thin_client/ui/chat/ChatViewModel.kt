package com.example.thin_client.ui.chat

import androidx.lifecycle.ViewModel
import com.example.thin_client.data.Message

class ChatViewModel(message: Message) : ViewModel() {


    fun messageDataChanges(text: String): Boolean {
        return text.isNotBlank()
    }

}
