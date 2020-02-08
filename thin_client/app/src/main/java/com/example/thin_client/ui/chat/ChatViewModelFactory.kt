package com.example.thin_client.ui.chat

import androidx.lifecycle.ViewModel
import androidx.lifecycle.ViewModelProvider
import com.example.thin_client.data.Message


/**
 * ViewModel provider factory to instantiate ChatViewModel.
 * Required given ChatViewModel has a non-empty constructor
 */
class ChatViewModelFactory(private val message:Message) : ViewModelProvider.NewInstanceFactory() {

    @Suppress("UNCHECKED_CAST")
    override fun <T : ViewModel?> create(modelClass: Class<T>): T {
       return ChatViewModel(message) as T

    }

}
