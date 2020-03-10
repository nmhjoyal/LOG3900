package com.example.thin_client.ui.chatrooms

import android.app.AlertDialog
import android.content.res.Resources
import android.graphics.Canvas
import android.graphics.Color
import android.graphics.drawable.ColorDrawable
import android.graphics.drawable.Drawable
import com.example.thin_client.ui.chat.ChatFragment

import android.os.Bundle
import android.os.Handler
import android.os.Looper
import android.view.*
import android.widget.EditText
import android.widget.RadioGroup
import android.widget.Toast
import androidx.core.content.ContextCompat
import androidx.fragment.app.Fragment
import androidx.recyclerview.widget.DividerItemDecoration
import androidx.recyclerview.widget.ItemTouchHelper
import androidx.recyclerview.widget.RecyclerView
import com.example.thin_client.R
import com.example.thin_client.data.Feedback
import com.example.thin_client.data.rooms.RoomArgs
import com.example.thin_client.data.rooms.RoomManager
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.google.gson.Gson
import com.xwray.groupie.GroupAdapter

import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.chatrooms_fragment.*


class ChatRoomsFragment : Fragment() {
    val adapter = GroupAdapter<GroupieViewHolder>()
    private var selectedRoom : String = ""
    private var newRoomName : String = ""
    private var swipeBackground: ColorDrawable = ColorDrawable(Color.parseColor("#FF0000"))
    private lateinit var deleteIcon: Drawable

//    private val itemTouchHelperCallBack = object : ItemTouchHelper.SimpleCallback(0, ItemTouchHelper.LEFT or ItemTouchHelper.RIGHT){
//        override fun onSwiped(viewHolder: RecyclerView.ViewHolder, position: Int) {
//
//            SocketHandler.deleteChatRoom(selectedRoom)
//        }
//
//        override fun onChildDraw(
//            c: Canvas,
//            recyclerView: RecyclerView,
//            viewHolder: RecyclerView.ViewHolder,
//            dX: Float,
//            dY: Float,
//            actionState: Int,
//            isCurrentlyActive: Boolean
//        ) {
//            val itemView = viewHolder.itemView
//            val iconMargin = (itemView.height - deleteIcon.intrinsicHeight)/2
//
//            if(dX > 0) {
//                swipeBackground.setBounds(itemView.left, itemView.top, dX.toInt(), itemView.bottom)
//                deleteIcon.setBounds(itemView.left + iconMargin, itemView.top + iconMargin,
//                    itemView.left + iconMargin+ deleteIcon.intrinsicWidth, itemView.bottom - iconMargin )
//            } else  {
//                swipeBackground.setBounds(itemView.right + dX.toInt(), itemView.top,itemView.right, itemView.bottom)
//                deleteIcon.setBounds(itemView.right - iconMargin - deleteIcon.intrinsicWidth, itemView.top + iconMargin,
//                    itemView.right - iconMargin, itemView.bottom - iconMargin )
//            }
//            swipeBackground.draw(c)
//            c.save()
//            if(dX > 0)
//                c.clipRect(itemView.left, itemView.top, dX.toInt(), itemView.bottom)
//            else
//                c.clipRect(itemView.right + dX.toInt(), itemView.top,itemView.right, itemView.bottom)
//
//            deleteIcon.draw(c)
//            c.restore()
//
//            super.onChildDraw(
//                c,
//                recyclerView,
//                viewHolder,
//                dX,
//                dY,
//                actionState,
//                isCurrentlyActive
//            )
//        }
//        override fun onMove(
//            recyclerView: RecyclerView,
//            viewHolder: RecyclerView.ViewHolder,
//            target: RecyclerView.ViewHolder
//        ): Boolean {
//            return false
//        }
//    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
//        val itemTouchHelper = ItemTouchHelper(itemTouchHelperCallBack)
//        itemTouchHelper.attachToRecyclerView(recyclerview_chatrooms)
//        deleteIcon  = ContextCompat.getDrawable(recyclerview_chatrooms.context, R.drawable.ic_delete_24px)!!

        adapter.setOnItemClickListener{ item, v ->
            selectedRoom = (item as ChatRoomItem).roomname
            RoomManager.currentRoom = selectedRoom
            SocketHandler.joinChatRoom(selectedRoom)
        }

        setupSocketEvents()

        add_room.setOnClickListener(({
            val alertBuilder = AlertDialog.Builder(context)
            alertBuilder.setTitle(R.string.create_room)
            val dialogView = layoutInflater.inflate(R.layout.dialog_create_room, null)
            alertBuilder.setView(dialogView)
            val radioGroup = dialogView.findViewById<RadioGroup>(R.id.room_visibility)
            var isPrivate = false
            radioGroup.check(R.id.is_public_room)
            radioGroup.setOnCheckedChangeListener(({ _, checkedId ->
                isPrivate = checkedId == R.id.is_private_room
            }))

            alertBuilder
                .setPositiveButton(R.string.ok) { _, _ ->
                    newRoomName = dialogView.findViewById<EditText>(R.id.room_name).text.toString()
                    if (newRoomName.isNotBlank()) {
                        SocketHandler.createChatRoom(newRoomName, isPrivate)
                    } else {
                        dialogView.findViewById<EditText>(R.id.room_name).error = Resources.getSystem().getString(R.string.error_roomname)
                    }
                }
                .setNegativeButton(R.string.cancel) { _, _ -> }
            val dialog = alertBuilder.create()
            dialog.window!!.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE)
            dialog.show()
        }))


        fetchRooms()

        recyclerview_chatrooms.addItemDecoration(DividerItemDecoration(recyclerview_chatrooms.context, DividerItemDecoration.VERTICAL))
    }

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        return inflater.inflate(R.layout.chatrooms_fragment, container, false)
    }

    override fun onDestroy() {
        super.onDestroy()
        turnOffSocketEvents()
    }

    private fun fetchRooms() {
        for (room in RoomManager.roomsJoined.keys) {
            adapter.add(ChatRoomItem(room))
        }
        recyclerview_chatrooms.adapter = adapter
    }

    private fun getRoomPosition(): Int {
        val roomKeys = RoomManager.roomsJoined.keys
        return roomKeys.indexOf(RoomManager.roomToDelete)
    }

    private fun setupSocketEvents() {
        SocketHandler.socket!!
            .on(SocketEvent.ROOM_CREATED, ({ data ->
                val roomCreateFeedback = Gson().fromJson(data.first().toString(), Feedback::class.java)
                Handler(Looper.getMainLooper()).post(Runnable {
                    if (roomCreateFeedback.status) {
                        adapter.add(ChatRoomItem(newRoomName))
                        if (!RoomManager.roomsJoined.containsKey(newRoomName)) {
                            RoomManager.addRoom((newRoomName))
                        }
                    } else {
                        Toast.makeText(context, roomCreateFeedback.log_message, Toast.LENGTH_SHORT)
                            .show()
                    }
                })
            }))
            .on(SocketEvent.ROOM_DELETED, ({ data ->
                val leaveRoomFeedback = Gson().fromJson(data.first().toString(), Feedback::class.java)
                Handler(Looper.getMainLooper()).post(Runnable {
                    if (leaveRoomFeedback.status) {
                        adapter.removeGroupAtAdapterPosition(getRoomPosition())
                        adapter.notifyItemRemoved(getRoomPosition())
                        RoomManager.leaveRoom()
                    } else {
                        Toast.makeText(context, leaveRoomFeedback.log_message, Toast.LENGTH_SHORT).show()
                    }
                })
            }))
    }

    private fun turnOffSocketEvents() {
        if (SocketHandler.socket != null) {
            SocketHandler.socket!!.off(SocketEvent.ROOM_CREATED)
                .off(SocketEvent.ROOM_DELETED)
        }
    }

}
