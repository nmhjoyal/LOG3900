package com.example.thin_client.ui.chat

import android.content.Context
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.*
import android.view.inputmethod.InputMethodManager
import androidx.fragment.app.Fragment
import com.example.thin_client.R
import com.example.thin_client.data.Feedback
import com.example.thin_client.data.Message
import com.example.thin_client.data.rooms.RoomArgs
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.chatrooms.ChatRoomsFragment
import com.example.thin_client.ui.login.afterTextChanged
import com.google.gson.Gson
import com.xwray.groupie.GroupAdapter
import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.activity_chat.*
import net.yslibrary.android.keyboardvisibilityevent.util.UIUtil

class ChatFragment : Fragment() {

    private val adapter = GroupAdapter<GroupieViewHolder>()
    private var roomID : String ?= ""

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        recyclerview_chat.adapter = adapter


        roomID = arguments!!.getString(RoomArgs.ROOM_ID)
        room_id.text = roomID

        SocketHandler.socket?.on(SocketEvent.USER_JOINED_ROOM, ({ data ->
            val jsonData = Gson().fromJson(data.first().toString(), Feedback::class.java)
            showUserJoined(jsonData.log_message, true)
        }))
            ?.on(SocketEvent.USER_LEFT_ROOM, ({ data ->
                val jsonData = Gson().fromJson(data.first().toString(), Feedback::class.java)
                showUserJoined(jsonData.log_message, false)
            }))
            ?.on(SocketEvent.NEW_MESSAGE, ({ data ->
                val jsonData = Gson().fromJson(data.first().toString(), Message::class.java)
                val username = jsonData.author.username
                val timestamp = jsonData.date
                Handler(Looper.getMainLooper()).post(Runnable {
                    if (username == SocketHandler.user!!.username) {
                        showToMessage(jsonData.content, timestamp)
                    } else {
                        showFromMessage(jsonData.content, username, timestamp)
                    }
                })
            }))

        back_button.setOnClickListener(({
            val transaction = fragmentManager!!.beginTransaction()
            val chatRoomsFragment = ChatRoomsFragment()
            transaction.replace(R.id.chatrooms_container, chatRoomsFragment)
            transaction.addToBackStack(null)
            transaction.commit()
        }))

        send_button_chat.setOnClickListener {
            if (editText_chat.text.isNotBlank()) {
                send_button_chat.isEnabled = true
                SocketHandler.sendMessage(editText_chat.text.toString(), roomID!!)
                editText_chat.setText("")
            }
        }

        editText_chat.setOnKeyListener(View.OnKeyListener { v, keyCode, event ->
            if(editText_chat.text.isNotBlank()) {
                send_button_chat.isEnabled = true
                if (keyCode == KeyEvent.KEYCODE_ENTER && event.action == KeyEvent.ACTION_UP) {
                    SocketHandler.sendMessage(editText_chat.text.replace("\\n".toRegex(), ""), roomID!!)
                    editText_chat.setText("")
                    UIUtil.hideKeyboard(activity!!)
                    return@OnKeyListener true
                }
            }
            false
        })

        editText_chat.afterTextChanged {
            send_button_chat.isEnabled = editText_chat.text.isNotBlank()
        }
    }

    private fun showToMessage(text: String, date: Long){
        adapter.add(ChatToItem(text.replace("\\n".toRegex(), ""), date))
        if (recyclerview_chat != null){
            recyclerview_chat.scrollToPosition(adapter.itemCount - 1)
        }
    }

    private fun showFromMessage(text: String, author:String, date: Long) {
        adapter.add(ChatFromItem(text, author, date))
        if (recyclerview_chat != null){
            recyclerview_chat.scrollToPosition(adapter.itemCount - 1)
        }
    }

    private fun showUserJoined(author:String, hasJoined: Boolean) {
        adapter.add(ChatUserJoined(author, hasJoined))
        if (recyclerview_chat != null){
            recyclerview_chat.scrollToPosition(adapter.itemCount - 1)
        }
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {

        return inflater.inflate(R.layout.activity_chat, container, false)
    }
}