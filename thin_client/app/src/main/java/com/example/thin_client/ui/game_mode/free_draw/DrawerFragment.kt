package com.example.thin_client.ui.game_mode.free_draw

import android.graphics.Paint
import android.os.Bundle
import android.view.*
import android.widget.SeekBar
import androidx.core.content.ContextCompat
import androidx.fragment.app.Fragment
import com.example.thin_client.R
import com.example.thin_client.data.game.GameArgs
import kotlinx.android.synthetic.main.free_draw_fragment.*

private const val PERCENT = 100f
private const val SCALE_FACTOR = 8f
private const val SIZING_FACTOR = 80f

class DrawerFragment : Fragment() {

    var lastColour = R.color.color_black

    override fun onViewCreated(view: View, savedInstanceState: Bundle?) {
        super.onViewCreated(view, savedInstanceState)

        trash.setOnClickListener(({
            draw_view.clearCanvas()
        }))
        save_button.visibility = View.GONE

        crayon.setOnClickListener(({
            draw_view.toggleEraser(false)
            resetDrawingOptions()
            crayon.setBackgroundResource(R.drawable.circle_primary_dark)
            draw_view.setColor(ContextCompat.getColor(context!!, lastColour))
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
            draw_view.setColor(ContextCompat.getColor(context!!, R.color.default_background))
        }))

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

    override fun onCreateView(
        inflater: LayoutInflater,
        container: ViewGroup?,
        savedInstanceState: Bundle?
    ): View {
        return inflater.inflate(R.layout.free_draw_fragment, container, false)
    }

    private fun setColour(sizingDrawable: Int, colourRes: Int) {
        draw_view.toggleEraser(false)
        resetColourOptions()
        resetDrawingOptions()
        crayon.setBackgroundResource(R.drawable.circle_primary_dark)
        sizing.setBackgroundResource(sizingDrawable)
        draw_view.setColor(ContextCompat.getColor(context!!, colourRes))
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
}
