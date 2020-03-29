package com.example.thin_client.ui.chat

import android.app.AlertDialog
import android.content.Context
import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.*
import android.widget.*
import androidx.fragment.app.Fragment
import androidx.recyclerview.widget.RecyclerView
import com.example.thin_client.R
import com.example.thin_client.data.AvatarID
import com.example.thin_client.data.Feedback
import com.example.thin_client.data.Message
import com.example.thin_client.data.app_preferences.PreferenceHandler
import com.example.thin_client.data.game.GameArgs
import com.example.thin_client.data.getAvatar
import com.example.thin_client.data.rooms.Invitation
import com.example.thin_client.data.rooms.RoomArgs
import com.example.thin_client.data.rooms.RoomManager
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.chatrooms.ChatRoomsFragment
import com.example.thin_client.ui.chatrooms.InviteUserRow
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
    private lateinit var userAvatarID: AvatarID


    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        recyclerview_chat.adapter = adapter

        userAvatarID = AvatarID.valueOf(PreferenceHandler(context!!).getPublicProfile().avatar)

        roomID = arguments!!.getString(RoomArgs.ROOM_ID)
        room_id.text = roomID

        if (arguments!!.getBoolean(GameArgs.IS_GAME_CHAT)) {
            back_button.visibility = View.GONE
            leave_button.visibility = View.GONE
            invite_user_button.visibility = View.GONE
        } else {
            send_guess.visibility = View.GONE
        }

        retreiveExistingMessages()
        setupSocketEvents()

        back_button.setOnClickListener(({
            goBackToRooms()
        }))

        send_button_chat.setOnClickListener {
            if (editText_chat.text.isNotBlank()) {
                send_button_chat.isEnabled = true
                send_guess.isEnabled = true
                SocketHandler.sendMessage(editText_chat.text.toString(), roomID!!)
                editText_chat.setText("")
            }
        }

        send_guess.setOnClickListener {
            if (editText_chat.text.isNotBlank()) {
                send_button_chat.isEnabled = true
                send_guess.isEnabled = true
                SocketHandler.sendGuess(editText_chat.text.toString())
                editText_chat.setText("")
            }
        }

        editText_chat.setOnKeyListener(View.OnKeyListener { v, keyCode, event ->
            if(editText_chat.text.isNotBlank()) {
                send_button_chat.isEnabled = true
                send_guess.isEnabled = true
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
            send_guess.isEnabled = editText_chat.text.isNotBlank()
        }

        invite_user_button.setOnClickListener(({
            showInviteDialog()
        }))
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        return inflater.inflate(R.layout.activity_chat, container, false)
    }

    private fun retreiveExistingMessages() {
        val roomsJoined = RoomManager.roomsJoined
        val messages = roomsJoined[roomID]
        if (messages !== null) {
            for(i in 0 until messages.size){
                when (messages[i].username) {
                    admin -> showAdminMessage(messages[i].content)
                    SocketHandler.user!!.username -> showToMessage(messages[i].content, messages[i].date)
                    else -> {
                        var userAvatar: AvatarID = AvatarID.AVOCADO
                        if (RoomManager.roomAvatars[roomID] !== null) {
                            userAvatar = getAvatar(RoomManager.roomAvatars[roomID]!![messages[i].username])
                        }
                        showFromMessage(
                            messages[i].content,
                            userAvatar,
                            messages[i].username,
                            messages[i].date
                        )
                    }
                }
            }
        }
    }

    private fun setupSocketEvents() {
        SocketHandler.socket
            ?.on(SocketEvent.NEW_MESSAGE, ({ data ->
                val jsonData = Gson().fromJson(data.first().toString(), Message::class.java)
                val username = jsonData.username
                val timestamp = jsonData.date
                Handler(Looper.getMainLooper()).post(Runnable {
                    when (username) {
                        admin -> showAdminMessage(jsonData.content)
                        SocketHandler.user!!.username -> showToMessage(jsonData.content, timestamp)
                        else -> {
                            var userAvatar: AvatarID = AvatarID.AVOCADO
                            if (RoomManager.roomAvatars[roomID] !== null) {
                                userAvatar = getAvatar(RoomManager.roomAvatars[roomID]!![username])
                            }
                            showFromMessage(
                                jsonData.content,
                                userAvatar,
                                username,
                                timestamp
                            )
                        }
                    }
                    if (RoomManager.roomsJoined.containsKey(roomID)) {
                        if (!RoomManager.roomsJoined.get(roomID)!!.contains(jsonData)) {
                            RoomManager.roomsJoined.get(roomID)!!.add(jsonData)
                        }
                    }
                })
            }))
            ?.on(SocketEvent.GUESS_RESULT, ({ data ->
                val response = Gson().fromJson(data.first().toString(), Feedback::class.java)
                Handler(Looper.getMainLooper()).post(Runnable {
                    if (response.status) {
                        send_guess.isEnabled = false
                    } else {
                        showAdminMessage(response.log_message)
                    }
                })

            }))
    }

    private fun showToMessage(text: String, date: Long){
        val trimmedText = text.replace("\\n".toRegex(), "")
        adapter.add(ChatToItem(trimmedText, userAvatarID, date))
        if (recyclerview_chat != null){
            recyclerview_chat.scrollToPosition(adapter.itemCount - 1)
        }
    }

    private fun showFromMessage(text: String, avatarID: AvatarID, author:String, date: Long) {
        adapter.add(ChatFromItem(text, avatarID, author, date))
        if (recyclerview_chat != null){
            recyclerview_chat.scrollToPosition(adapter.itemCount - 1)
        }
    }

    private fun showAdminMessage(text:String){
        adapter.add(ChatUserJoined(text))
    }

    private fun goBackToRooms() {
        val transaction = fragmentManager!!.beginTransaction()
        val chatRoomsFragment = ChatRoomsFragment()
        transaction.replace(R.id.chatrooms_container, chatRoomsFragment)
        transaction.addToBackStack(null)
        transaction.commit()
    }

    private fun showInviteDialog() {
        val alertBuilder = AlertDialog.Builder(context)
        alertBuilder.setTitle(R.string.invite_user)
        val dialogView = layoutInflater.inflate(R.layout.dialog_invite_users, null)
        alertBuilder.setView(dialogView)
        val inviteRecyclerView = dialogView.findViewById<RecyclerView>(R.id.invite_list)
        val addButton = dialogView.findViewById<ImageButton>(R.id.add_user_button)
        val addUsername = dialogView.findViewById<EditText>(R.id.add_user_username)
        val inviteListAdapter = GroupAdapter<GroupieViewHolder>()
        var inviteList = arrayListOf<String>()
        inviteListAdapter.setOnItemClickListener(({ item, view ->
            inviteListAdapter.remove(item)
            inviteListAdapter.notifyDataSetChanged()
            inviteList.remove((item as InviteUserRow).user)
        }))
        addButton.setOnClickListener(({
            if (addUsername.text.isNotBlank()) {
                inviteListAdapter.add(InviteUserRow(addUsername.text.toString()))
                inviteListAdapter.notifyDataSetChanged()
                inviteRecyclerView.scrollToPosition(inviteListAdapter.itemCount - 1)
                if (!inviteList.contains(addUsername.text.toString())) {
                    inviteList.add(addUsername.text.toString())
                }
                addUsername.text.clear()
            }
        }))
        inviteRecyclerView.adapter = inviteListAdapter
        alertBuilder
            .setPositiveButton(R.string.ok) { _, _ ->
                for (invitee in inviteList) {
                    SocketHandler.sendInvite(Invitation(roomID!!, invitee))
                }
                inviteList = arrayListOf()
            }
            .setNegativeButton(R.string.cancel) { _, _ -> }
            .setCancelable(false)
        val dialog = alertBuilder.create()
        dialog.window!!.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE)
        dialog.show()
    }
}