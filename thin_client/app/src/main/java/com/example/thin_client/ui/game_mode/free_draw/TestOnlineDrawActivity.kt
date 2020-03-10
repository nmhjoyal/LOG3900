package com.example.thin_client.ui.game_mode.free_draw

import android.os.Bundle
import android.view.MenuItem
import androidx.appcompat.app.AppCompatActivity
import com.example.thin_client.R
import com.example.thin_client.data.drawing.DrawPoint
import com.example.thin_client.data.server.SocketEvent
import com.example.thin_client.server.SocketHandler
import com.google.gson.Gson
import kotlinx.android.synthetic.main.activity_observer.*


class TestOnlineDrawActivity : AppCompatActivity() {


    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_observer)
        draw_view.isDrawer = false

        SocketHandler.socket!!.on(SocketEvent.DRAW_POINT, ({ data ->
            val drawPoint = Gson().fromJson(data.first().toString(), DrawPoint::class.java)
            draw_view.addPath(drawPoint)
        }))
    }

    override fun onBackPressed() {
        // Disable native back
    }

    override fun onOptionsItemSelected(item: MenuItem): Boolean {
        when (item.itemId) {
            android.R.id.home -> {
                SocketHandler.disconnectOnlineDraw()
                finish()
                return true
            }
        }
        return super.onOptionsItemSelected(item)
    }
}
