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
import android.util.Log
import android.view.*
import android.widget.EditText
import android.widget.Toast
import androidx.core.content.ContextCompat
import androidx.core.view.isNotEmpty
import androidx.fragment.app.Fragment
import androidx.recyclerview.widget.DividerItemDecoration
import androidx.recyclerview.widget.ItemTouchHelper
import androidx.recyclerview.widget.RecyclerView
import com.example.thin_client.R
import com.example.thin_client.data.Feedback
import com.example.thin_client.data.model.Room
import com.example.thin_client.data.rooms.RoomArgs
import com.example.thin_client.data.rooms.RoomManager
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.Lobby
import com.google.gson.Gson
import com.xwray.groupie.GroupAdapter

import com.xwray.groupie.GroupieViewHolder
import kotlinx.android.synthetic.main.activity_chat.*
import kotlinx.android.synthetic.main.chatrooms_fragment.*
import kotlinx.android.synthetic.main.chatrooms_row.*
import kotlinx.android.synthetic.main.room_row.*
import okhttp3.internal.notify
import java.net.Socket


class ChatRoomsFragment : Fragment() {
    val adapter = GroupAdapter<GroupieViewHolder>()
    private var selectedRoom : String = ""
    private var newRoomName : String = ""
    private var swipeBackground: ColorDrawable = ColorDrawable(Color.parseColor("#FF0000"))
    private lateinit var deleteIcon: Drawable

    private val itemTouchHelperCallBack = object : ItemTouchHelper.SimpleCallback(0, ItemTouchHelper.LEFT or ItemTouchHelper.RIGHT){
        override fun onSwiped(viewHolder: RecyclerView.ViewHolder, position: Int) {

            SocketHandler.leaveChatRoom(selectedRoom)
        }

        override fun onChildDraw(
            c: Canvas,
            recyclerView: RecyclerView,
            viewHolder: RecyclerView.ViewHolder,
            dX: Float,
            dY: Float,
            actionState: Int,
            isCurrentlyActive: Boolean
        ) {
            val itemView = viewHolder.itemView
            val iconMargin = (itemView.height - deleteIcon.intrinsicHeight)/2

            if(dX > 0) {
                swipeBackground.setBounds(itemView.left, itemView.top, dX.toInt(), itemView.bottom)
                deleteIcon.setBounds(itemView.left + iconMargin, itemView.top + iconMargin,
                    itemView.left + iconMargin+ deleteIcon.intrinsicWidth, itemView.bottom - iconMargin )
            } else  {
                swipeBackground.setBounds(itemView.right + dX.toInt(), itemView.top,itemView.right, itemView.bottom)
                deleteIcon.setBounds(itemView.right - iconMargin - deleteIcon.intrinsicWidth, itemView.top + iconMargin,
                    itemView.right - iconMargin, itemView.bottom - iconMargin )
            }
            swipeBackground.draw(c)
            c.save()
            if(dX > 0)
                c.clipRect(itemView.left, itemView.top, dX.toInt(), itemView.bottom)
            else
                c.clipRect(itemView.right + dX.toInt(), itemView.top,itemView.right, itemView.bottom)

            deleteIcon.draw(c)
            c.restore()

            super.onChildDraw(
                c,
                recyclerView,
                viewHolder,
                dX,
                dY,
                actionState,
                isCurrentlyActive
            )
        }
        override fun onMove(
            recyclerView: RecyclerView,
            viewHolder: RecyclerView.ViewHolder,
            target: RecyclerView.ViewHolder
        ): Boolean {
            return false
        }
    }

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)
        val itemTouchHelper = ItemTouchHelper(itemTouchHelperCallBack)
        itemTouchHelper.attachToRecyclerView(recyclerview_chatrooms)
        deleteIcon  = ContextCompat.getDrawable(recyclerview_chatrooms.context, R.drawable.ic_delete_24px)!!

        adapter.setOnItemClickListener{ item, v ->
            selectedRoom = (item as ChatRoomItem).roomname
            //SocketHandler.joinChatRoom(selectedRoom)
        }


        SocketHandler.socket!!.on(SocketEvent.USER_JOINED_ROOM, ({
            val bundle = Bundle()
            bundle.putString(RoomArgs.ROOM_ID, selectedRoom)
            val transaction = fragmentManager!!.beginTransaction()
            val chatFragment = ChatFragment()
            chatFragment.arguments = bundle
            transaction.replace(R.id.chatrooms_container, chatFragment)
            transaction.addToBackStack(null)
            transaction.commit()
        }))
            .on(SocketEvent.ROOM_CREATED, ({ data ->
                val roomCreateFeedback = Gson().fromJson(data.first().toString(), Feedback::class.java)
                if (roomCreateFeedback.status) {
                    activity!!.runOnUiThread(({
                        adapter.add(ChatRoomItem(newRoomName))
                        RoomManager.addRoom((newRoomName))
                    }))
                } else {
                    Handler(Looper.getMainLooper()).post(Runnable {
                        Toast.makeText(context, roomCreateFeedback.log_message, Toast.LENGTH_SHORT).show()
                    })
                }
            }))
            .on(SocketEvent.USER_LEFT_ROOM, ({ data ->
                val leaveRoomFeedback = Gson().fromJson(data.first().toString(), Feedback::class.java)
                if (leaveRoomFeedback.status){
                    activity!!.runOnUiThread(({
                        adapter.removeGroupAtAdapterPosition(getRoomPosition())
                        adapter.notifyItemRemoved(getRoomPosition())
                    }))
                }else {
                    Handler(Looper.getMainLooper()).post(Runnable {
                        Toast.makeText(context, leaveRoomFeedback.log_message, Toast.LENGTH_SHORT)
                            .show()
                    })
                }

            }))

        add_room.setOnClickListener(({
            val alertBuilder = AlertDialog.Builder(context)
            alertBuilder.setTitle(R.string.create_room)
            val roomname = EditText(context)
            roomname.setHint(R.string.room_name)
            alertBuilder.setView(roomname)
            alertBuilder
                .setPositiveButton(R.string.ok) { _, _ ->
                    newRoomName = roomname.text.toString()
                    if (newRoomName.isNotBlank()) {
                        SocketHandler.createChatRoom(newRoomName)
                    } else {
                        roomname.error = Resources.getSystem().getString(R.string.error_roomname)
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

    private fun fetchRooms() {
        for (room in RoomManager.roomsJoined.keys) {
            adapter.add(ChatRoomItem(room))
        }
        recyclerview_chatrooms.adapter = adapter
    }

    private fun getRoomPosition(): Int {
        val roomKeys = RoomManager.roomsJoined.keys
        return roomKeys.indexOf(selectedRoom)
    }

}
