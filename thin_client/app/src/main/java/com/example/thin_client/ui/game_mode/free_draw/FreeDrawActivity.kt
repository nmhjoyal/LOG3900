package com.example.thin_client.ui.game_mode.free_draw

import android.content.pm.PackageManager
import android.graphics.Bitmap
import android.graphics.Paint
import android.os.Bundle
import android.provider.MediaStore
import android.view.MenuInflater
import android.view.WindowManager
import android.widget.EditText
import android.widget.SeekBar
import android.widget.Toast
import androidx.appcompat.app.AlertDialog
import androidx.appcompat.app.AppCompatActivity
import androidx.appcompat.widget.PopupMenu
import androidx.core.app.ActivityCompat
import androidx.core.content.ContextCompat
import com.example.thin_client.R
import kotlinx.android.synthetic.main.activity_free_draw.*
import java.util.*


class FreeDrawActivity : AppCompatActivity() {

    override fun onCreate(savedInstanceState: Bundle?) {
        super.onCreate(savedInstanceState)
        setContentView(R.layout.activity_free_draw)

        tip_button.setOnClickListener(({v ->
            draw_view.toggleEraser(false)
            val popup = PopupMenu(this, v)
            val inflater: MenuInflater = popup.menuInflater
            inflater.inflate(R.menu.draw_tip_menu, popup.menu)
            popup.setOnMenuItemClickListener (({
                if (it.itemId == R.id.round) {
                    draw_view.setStrokeCap(Paint.Cap.ROUND)
                } else {
                    draw_view.setStrokeCap(Paint.Cap.SQUARE)
                }
            }))
            popup.show()
        }))

        save_button.setOnClickListener(({
            if (!hasStoragePermission()) {
                requestStoragePermission()
            } else {
                showSaveDialog(draw_view.getBitmap())
            }
        }))

        red.setOnClickListener(({
            draw_view.toggleEraser(false)
            draw_view.setColor(ContextCompat.getColor(this, R.color.color_red))
        }))

        orange.setOnClickListener(({
            draw_view.toggleEraser(false)
            draw_view.setColor(ContextCompat.getColor(this, R.color.color_orange))
        }))

        yellow.setOnClickListener(({
            draw_view.toggleEraser(false)
            draw_view.setColor(ContextCompat.getColor(this, R.color.color_yellow))
        }))

        green.setOnClickListener(({
            draw_view.toggleEraser(false)
            draw_view.setColor(ContextCompat.getColor(this, R.color.color_green))
        }))

        blue.setOnClickListener(({
            draw_view.toggleEraser(false)
            draw_view.setColor(ContextCompat.getColor(this, R.color.color_blue))
        }))

        purple.setOnClickListener(({
            draw_view.toggleEraser(false)
            draw_view.setColor(ContextCompat.getColor(this, R.color.color_purple))
        }))

        pink.setOnClickListener(({
            draw_view.toggleEraser(false)
            draw_view.setColor(ContextCompat.getColor(this, R.color.color_pink))
        }))

        white.setOnClickListener(({
            draw_view.toggleEraser(false)
            draw_view.setColor(ContextCompat.getColor(this, R.color.color_white))
        }))

        grey.setOnClickListener(({
            draw_view.toggleEraser(false)
            draw_view.setColor(ContextCompat.getColor(this, R.color.color_grey))
        }))

        brown.setOnClickListener(({
            draw_view.toggleEraser(false)
            draw_view.setColor(ContextCompat.getColor(this, R.color.color_brown))
        }))

        black.setOnClickListener(({
            draw_view.toggleEraser(false)
            draw_view.setColor(ContextCompat.getColor(this, R.color.color_black))
        }))

        eraser.setOnClickListener(({v ->
            val popup = PopupMenu(this, v)
            val inflater: MenuInflater = popup.menuInflater
            inflater.inflate(R.menu.draw_erase_menu, popup.menu)
            popup.setOnMenuItemClickListener (({
                if (it.itemId == R.id.draw_erase) {
                    draw_view.toggleEraser(false)
                    draw_view.setColor(ContextCompat.getColor(this, R.color.default_background))
                } else {
                    draw_view.toggleEraser(true)
                }
            }))
            popup.show()
        }))

        seekBar.setOnSeekBarChangeListener(object: SeekBar.OnSeekBarChangeListener {
            override fun onProgressChanged(seekBar: SeekBar, progress: Int,
                                           fromUser: Boolean) {
                draw_view.toggleEraser(false)
                sizing.scaleX = (progress.div(100f)) * 10
                sizing.scaleY = (progress.div(100f)) * 10
            }

            override fun onStartTrackingTouch(seekBar: SeekBar) {
            }

            override fun onStopTrackingTouch(seekBar: SeekBar) {
                draw_view.setStrokeWidth(seekBar.progress.div(100f) * 70f)
            }
        })
    }

    private fun hasStoragePermission(): Boolean {
        return ContextCompat.checkSelfPermission(applicationContext, android.Manifest.permission.WRITE_EXTERNAL_STORAGE) ==
                PackageManager.PERMISSION_GRANTED
    }

    private fun requestStoragePermission() {
        ActivityCompat.requestPermissions(this,
            arrayOf(android.Manifest.permission.WRITE_EXTERNAL_STORAGE),
            2)
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
        alertDialog.setTitle("Save Drawing")
            .setPositiveButton("Save") { _, _ -> saveImage(bitmap,
                fileNameEditText.text.toString(), imgDescription.text.toString()) }
            .setNegativeButton("Cancel") { _, _ -> }

        val dialog = alertDialog.create()
        dialog.window!!.setSoftInputMode(WindowManager.LayoutParams.SOFT_INPUT_STATE_VISIBLE)
        dialog.show()
    }

    override fun onRequestPermissionsResult(requestCode: Int, permissions: Array<out String>, grantResults: IntArray) {
        when(requestCode){
            2 -> {
                if ((grantResults.isNotEmpty() && grantResults[0] == PackageManager.PERMISSION_GRANTED)){
                    showSaveDialog(draw_view.getBitmap())
                }
                return
            }
            else -> {}
        }
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
