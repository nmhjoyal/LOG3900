package com.example.thin_client.ui.game_mode.free_draw

import com.divyanshu.draw.widget.MyPath

import android.content.Context
import android.graphics.*
import android.util.AttributeSet
import android.view.MotionEvent
import android.view.View
import androidx.annotation.ColorInt
import androidx.core.graphics.ColorUtils
import com.example.thin_client.data.PaintOptions
import java.util.LinkedHashMap

/* https://github.com/divyanshub024/AndroidDraw */

class DrawView(context: Context, attrs: AttributeSet) : View(context, attrs) {
    var mPaths = LinkedHashMap<MyPath, PaintOptions>()

    private var mLastPaths = LinkedHashMap<MyPath, PaintOptions>()

    private var mPaint = Paint()
    private var mPath = MyPath()
    private var mPaintOptions = PaintOptions()
    private var mIsErasing = false

    private var mCurX = 0f
    private var mCurY = 0f
    private var mStartX = 0f
    private var mStartY = 0f
    private var mIsSaving = false
    private var mIsStrokeWidthBarEnabled = false

    init {
        mPaint.apply {
            color = mPaintOptions.color
            style = Paint.Style.STROKE
            strokeJoin = Paint.Join.ROUND
            strokeCap = Paint.Cap.ROUND
            strokeWidth = mPaintOptions.strokeWidth
            isAntiAlias = true
        }
    }

    fun setColor(newColor: Int): Boolean {
        @ColorInt
        val alphaColor = ColorUtils.setAlphaComponent(newColor, mPaintOptions.alpha)
        mPaintOptions.color = alphaColor
        if (mIsStrokeWidthBarEnabled) {
            invalidate()
        }
        return true
    }

    fun setStrokeWidth(newStrokeWidth: Float) {
        mPaintOptions.strokeWidth = newStrokeWidth
        if (mIsStrokeWidthBarEnabled) {
            invalidate()
        }
    }

    fun setStrokeCap(cap: Paint.Cap): Boolean {
        mPaintOptions.cap = cap
        if (mIsStrokeWidthBarEnabled) {
            invalidate()
        }
        return true
    }

    fun toggleEraser(isToggled: Boolean): Boolean {
        mIsErasing = isToggled
        return true
    }

    fun getBitmap(): Bitmap {
        val bitmap = Bitmap.createBitmap(width, height, Bitmap.Config.ARGB_8888)
        val canvas = Canvas(bitmap)
        canvas.drawColor(Color.WHITE)
        mIsSaving = true
        draw(canvas)
        mIsSaving = false
        return bitmap
    }

    fun addPath(path: MyPath, options: PaintOptions) {
        mPaths[path] = options
    }

    override fun onDraw(canvas: Canvas) {
        super.onDraw(canvas)

        for ((key, value) in mPaths) {
            changePaint(value)
            canvas.drawPath(key, mPaint)
        }

        changePaint(mPaintOptions)
        canvas.drawPath(mPath, mPaint)
    }

    private fun changePaint(paintOptions: PaintOptions) {
        mPaint.color = paintOptions.color
        mPaint.strokeWidth = paintOptions.strokeWidth
        mPaint.strokeCap = paintOptions.cap
    }

    fun clearCanvas() {
        mLastPaths = mPaths.clone() as LinkedHashMap<MyPath, PaintOptions>
        mPath.reset()
        mPaths.clear()
        invalidate()
    }

    private fun actionDown(x: Float, y: Float) {
        if (!mIsErasing) {
            mPath.reset()
            mPath.moveTo(x, y)
            mCurX = x
            mCurY = y
        }
    }

    private fun actionMove(x: Float, y: Float) {
        if (mIsErasing) {
            val region = Region()
            val toDelete: ArrayList<MyPath> = ArrayList()
            for (path in mPaths.keys) {
                region.setPath(path, Region(0, 0, this.right, this.bottom))
                if (region.contains(x.toInt() ,y.toInt())) {
                    toDelete.add(path)
                }
            }
            for (path in toDelete.iterator()) {
                mPaths.remove(path)
            }
        } else {
            mPath.quadTo(mCurX, mCurY, (x + mCurX) / 2, (y + mCurY) / 2)
            mCurX = x
            mCurY = y
        }
    }

    private fun actionUp() {
        if (!mIsErasing) {
            mPath.lineTo(mCurX, mCurY)

            // draw a dot on click
            if (mStartX == mCurX && mStartY == mCurY) {
                mPath.lineTo(mCurX, mCurY + 2)
                mPath.lineTo(mCurX + 1, mCurY + 2)
                mPath.lineTo(mCurX + 1, mCurY)
            }

            mPaths.put(mPath, mPaintOptions)
            mPath = MyPath()
            mPaintOptions = PaintOptions(
                mPaintOptions.color,
                mPaintOptions.strokeWidth,
                mPaintOptions.alpha,
                mPaintOptions.cap
            )
        }
    }

    override fun onTouchEvent(event: MotionEvent): Boolean {
        val x = event.x
        val y = event.y

        when (event.action) {
            MotionEvent.ACTION_DOWN -> {
                mStartX = x
                mStartY = y
                actionDown(x, y)
            }
            MotionEvent.ACTION_MOVE -> actionMove(x, y)
            MotionEvent.ACTION_UP -> actionUp()
        }

        invalidate()
        return true
    }
}