package com.example.thin_client.ui.chat

import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.*
import androidx.fragment.app.Fragment
import com.example.thin_client.R
import com.example.thin_client.data.Message
import com.example.thin_client.data.rooms.RoomArgs
import com.example.thin_client.data.rooms.RoomManager
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
    private val admin : String ="Admin"

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        recyclerview_chat.adapter = adapter


        roomID = arguments!!.getString(RoomArgs.ROOM_ID)
        room_id.text = roomID

        val roomsJoined = RoomManager.roomsJoined
        val messages = roomsJoined[roomID]
        for(i in 0 until messages!!.size){
            when (messages[i].username) {
                admin -> showAdminMessage(messages[i].content)
                SocketHandler.user!!.username -> showToMessage(messages[i].content, messages[i].date)
                else -> showFromMessage(messages[i].content, messages[i].username, messages[i].date)
            }
        }


        SocketHandler.socket?.on(SocketEvent.NEW_MESSAGE, ({ data ->
                val jsonData = Gson().fromJson(data.first().toString(), Message::class.java)
                val username = jsonData.username
                val timestamp = jsonData.date
                Handler(Looper.getMainLooper()).post(Runnable {
                    when (username) {
                        admin -> showAdminMessage(jsonData.content)
                        SocketHandler.user!!.username -> showToMessage(jsonData.content, timestamp)
                        else -> showFromMessage(jsonData.content, username, timestamp)
                    }
                    if (RoomManager.roomsJoined.containsKey(roomID)) {
                        if (!RoomManager.roomsJoined.get(roomID)!!.contains(jsonData)) {
                            RoomManager.roomsJoined.get(roomID)!!.add(jsonData)
                        }
                    }
                })
            }))

        back_button.setOnClickListener(({
            goBackToRooms()
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

        leave_button.setOnClickListener(({
            RoomManager.roomToRemove = roomID!!
            SocketHandler.leaveChatRoom(roomID!!)
            goBackToRooms()
        }))

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

    private fun showAdminMessage(text:String){
        adapter.add(ChatUserJoined(text))
        //TODO
    }

    private fun goBackToRooms() {
        val transaction = fragmentManager!!.beginTransaction()
        val chatRoomsFragment = ChatRoomsFragment()
        transaction.replace(R.id.chatrooms_container, chatRoomsFragment)
        transaction.addToBackStack(null)
        transaction.commit()
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        return inflater.inflate(R.layout.activity_chat, container, false)
    }
}