package com.example.thin_client.ui.game_mode.free_draw

import android.content.Context
import android.content.pm.PackageManager
import android.graphics.Bitmap
import android.graphics.Paint
import android.os.Bundle
import android.provider.MediaStore
import android.view.MenuItem
import android.view.WindowManager
import android.widget.EditText
import android.widget.SeekBar
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import androidx.core.content.ContextCompat
import com.example.thin_client.R
import com.example.thin_client.data.PermissionHandler
import com.example.thin_client.data.RequestCodes
import com.example.thin_client.data.app_preferences.PreferenceHandler
import com.example.thin_client.data.app_preferences.Preferences
import com.example.thin_client.data.lifecycle.LoginState
import com.example.thin_client.data.model.User
import com.example.thin_client.server.SocketHandler
import com.example.thin_client.ui.helpers.DEFAULT_INTERVAL
import com.example.thin_client.ui.helpers.setOnClickListener
import kotlinx.android.synthetic.main.free_draw_fragment.*
import java.util.*

private const val PERCENT = 100f
private const val SCALE_FACTOR = 8f
private const val SIZING_FACTOR = 80f

class FreeDrawActivity : AppCompatActivity() {

    var lastColour = R.color.color_black

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.free_draw_fragment)

        crayon.setOnClickListener(({
            draw_view.toggleEraser(false)
            resetDrawingOptions()
            crayon.setBackgroundResource(R.drawable.circle_primary_dark)
            draw_view.setColor(ContextCompat.getColor(this, lastColour))
        }))

        circle_cap.setOnClickListener(({
            draw_view.toggleEraser(false)
            circle_cap.setBackgroundResource(R.drawable.circle_primary_dark)
            draw_view.setStrokeCap(Paint.Cap.ROUND)
        }))

        square_cap.setOnClickListener(({
            draw_view.toggleEraser(false)
            square_cap.setBackgroundResource(R.drawable.circle_primary_dark)
            draw_view.setStrokeCap(Paint.Cap.SQUARE)
        }))

        vertical_cap.setOnClickListener(({
            draw_view.toggleEraser(false)
            vertical_cap.setBackgroundResource(R.drawable.circle_primary_dark)
            draw_view.setStrokeCap(Paint.Cap.BUTT)
        }))

        eraser.setOnClickListener(({
            draw_view.toggleEraser(true)
            resetDrawingOptions()
            eraser.setBackgroundResource(R.drawable.circle_primary_dark)
        }))

        pencil_eraser.setOnClickListener(({
            draw_view.toggleEraser(false)
            resetDrawingOptions()
            pencil_eraser.setBackgroundResource(R.drawable.circle_primary_dark)
            draw_view.setColor(ContextCompat.getColor(this, R.color.default_background))
        }))

        trash.setOnClickListener(({
            showClearDialog()
        }))

        save_button.setOnClickListener(DEFAULT_INTERVAL) {
            if (!PermissionHandler.hasStoragePermission(applicationContext)) {
                PermissionHandler.requestStoragePermission(this)
            } else {
                showSaveDialog(draw_view.getBitmap())
            }
        }

        red.setOnClickListener(({
            setColour(R.drawable.circle_red, R.color.color_red)
            red.setBackgroundResource(R.color.colorPrimaryDark)
        }))

        orange.setOnClickListener(({
            setColour(R.drawable.circle_orange, R.color.color_orange)
            orange.setBackgroundResource(R.color.colorPrimaryDark)
        }))

        yellow.setOnClickListener(({
            setColour(R.drawable.circle_yellow, R.color.color_yellow)
            yellow.setBackgroundResource(R.color.colorPrimaryDark)
        }))

        green.setOnClickListener(({
            setColour(R.drawable.circle_green, R.color.color_green)
            green.setBackgroundResource(R.color.colorPrimaryDark)
        }))

        blue.setOnClickListener(({
            setColour(R.drawable.circle_blue, R.color.color_blue)
            blue.setBackgroundResource(R.color.colorPrimaryDark)
        }))

        purple.setOnClickListener(({
            setColour(R.drawable.circle_purple, R.color.color_purple)
            purple.setBackgroundResource(R.color.colorPrimaryDark)
        }))

        pink.setOnClickListener(({
            setColour(R.drawable.circle_pink, R.color.color_pink)
            pink.setBackgroundResource(R.color.colorPrimaryDark)
        }))

        white.setOnClickListener(({
            setColour(R.drawable.circle_white, R.color.color_white)
            white.setBackgroundResource(R.color.colorPrimaryDark)
        }))

        grey.setOnClickListener(({
            setColour(R.drawable.circle_grey, R.color.color_grey)
            grey.setBackgroundResource(R.color.colorPrimaryDark)
        }))

        brown.setOnClickListener(({
            setColour(R.drawable.circle_brown, R.color.color_brown)
            brown.setBackgroundResource(R.color.colorPrimaryDark)
        }))

        black.setOnClickListener(({
            setColour(R.drawable.circle_black, R.color.color_black)
            black.setBackgroundResource(R.color.colorPrimaryDark)
        }))

        seekBar.setOnSeekBarChangeListener(object: SeekBar.OnSeekBarChangeListener {
            override fun onProgressChanged(seekBar: SeekBar, progress: Int,
                                           fromUser: Boolean) {
                if (progress == 0) {
                    sizing.scaleX = (1.div(PERCENT)) * SCALE_FACTOR
                    sizing.scaleY = (1.div(PERCENT)) * SCALE_FACTOR
                }
                sizing.scaleX = (progress.div(PERCENT)) * SCALE_FACTOR
                sizing.scaleY = (progress.div(PERCENT)) * SCALE_FACTOR
            }

            override fun onStartTrackingTouch(seekBar: SeekBar) {
            }

            override fun onStopTrackingTouch(seekBar: SeekBar) {
                draw_view.setStrokeWidth(seekBar.progress.div(PERCENT) * SIZING_FACTOR)
            }
        })
    }

    override fun onStart() {
        super.onStart()
        setupSocket()
    }

    private fun setupSocket() {
        if (!SocketHandler.isConnected()) {
            SocketHandler.connect()
        }

        val prefs = this.getSharedPreferences(Preferences.USER_PREFS, Context.MODE_PRIVATE)
        when (SocketHandler.getLoginState(prefs)) {
            LoginState.FIRST_LOGIN -> {}
            LoginState.LOGIN_WITH_EXISTING -> {
                val user = PreferenceHandler(applicationContext).getUser()
                SocketHandler.login(User(user.username, user.password))
                SocketHandler.isLoggedIn = true
            }
            LoginState.LOGGED_IN -> {
            }

        }
    }

    private fun setColour(sizingDrawable: Int, colourRes: Int) {
        draw_view.toggleEraser(false)
        resetColourOptions()
        resetDrawingOptions()
        crayon.setBackgroundResource(R.drawable.circle_primary_dark)
        sizing.setBackgroundResource(sizingDrawable)
        draw_view.setColor(ContextCompat.getColor(this, colourRes))
        lastColour = colourRes
    }

    private fun resetDrawingOptions() {
        crayon.setBackgroundResource(R.color.colorPrimary)
        eraser.setBackgroundResource(R.color.colorPrimary)
        pencil_eraser.setBackgroundResource(R.color.colorPrimary)
    }

    private fun resetColourOptions() {
        red.setBackgroundResource(R.color.colorPrimary)
        orange.setBackgroundResource(R.color.colorPrimary)
        yellow.setBackgroundResource(R.color.colorPrimary)
        green.setBackgroundResource(R.color.colorPrimary)
        blue.setBackgroundResource(R.color.colorPrimary)
        purple.setBackgroundResource(R.color.colorPrimary)
        pink.setBackgroundResource(R.color.colorPrimary)
        white.setBackgroundResource(R.color.colorPrimary)
        grey.setBackgroundResource(R.color.colorPrimary)
        brown.setBackgroundResource(R.color.colorPrimary)
        black.setBackgroundResource(R.color.colorPrimary)
    }

    override fun onBackPressed() {
        // Disable native back
    }

    override fun onRequestPermissionsResult(requestCode: Int, permissions: Array<out String>, grantResults: IntArray) {
        when(requestCode){
            RequestCodes.WRITE_EXTERNAL_STORAGE.ordinal -> {
                if ((grantResults.isNotEmpty() && grantResults[0] == PackageManager.PERMISSION_GRANTED)){
                    showSaveDialog(draw_view.getBitmap())
                } else {
                    Toast.makeText(applicationContext, R.string.error_image_saved, Toast.LENGTH_SHORT).show()
                }
                return
            }
            else -> {}
        }
    }

    private fun showSaveDialog(bitmap: Bitmap) {
        val alertDialog = AlertDialog.Builder(this)
        val dialogView = layoutInflater.inflate(R.layout.dialog_save, null)
        alertDialog.setView(dialogView)
        val fileNameEditText: EditText = dialogView.findViewById(R.id.editText_file_name)
        val imgDescription: EditText = dialogView.findViewById(R.id.editText_img_description)
        val filename = UUID.randomUUID().toString()
        fileNameEditText.setSelectAllOnFocus(true)
        fileNameEditText.setText(filename)
        alertDialog.setTitle(R.string.save_drawing)
            .setPositiveButton(R.string.save) { _, _ -> saveImage(bitmap,
                fileNameEditText.text.toString(), imgDescription.text.toString()) }
            .setNegativeButton(R.string.cancel) { _, _ -> }

        val dialog = alertDialog.create()
        dialog.window!!.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE)
        dialog.show()
    }

    private fun showClearDialog() {
        val alertDialog = AlertDialog.Builder(this)
        alertDialog.setTitle(R.string.trash_cd)
            .setMessage(R.string.trash_dialog)
            .setPositiveButton(R.string.yes) { _, _ -> draw_view.clearCanvas() }
            .setCancelable(true)
            .setNegativeButton(R.string.cancel) { _, _ -> }

        val dialog = alertDialog.create()
        dialog.window!!.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE)
        dialog.show()
    }

    private fun saveImage(bitmap: Bitmap, fileName: String, imgDescription: String) {
        val result = MediaStore.Images.Media.insertImage(contentResolver, bitmap, fileName, imgDescription)
        if (result != null) {
            Toast.makeText(applicationContext, R.string.image_saved, Toast.LENGTH_SHORT).show()
        } else {
            Toast.makeText(applicationContext, R.string.error_image_saved, Toast.LENGTH_SHORT).show()
        }
    }
}
