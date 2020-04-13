package com.example.thin_client.ui.game_mode.free_draw

import android.content.Context
import android.graphics.*
import android.util.AttributeSet
import android.view.MotionEvent
import android.view.View
import androidx.annotation.ColorInt
import androidx.core.content.ContextCompat
import androidx.core.graphics.ColorUtils
import com.divyanshu.draw.widget.MyPath
import com.example.thin_client.R
import com.example.thin_client.data.PaintOptions
import com.example.thin_client.data.drawing.*
import com.example.thin_client.server.SocketHandler
import java.util.*
import kotlin.collections.ArrayList
import kotlin.collections.component1
import kotlin.collections.component2
import kotlin.collections.iterator


/* https://github.com/divyanshub024/AndroidDraw */

class DrawView(context: Context, attrs: AttributeSet) : View(context, attrs) {
    var mPaths = LinkedHashMap<MyPath, PaintOptions>()
    var isDrawer: Boolean = true
    private var mIsFirstMove = false

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

    private fun getStrokeCapShape(): Shape {
        return if (mPaintOptions.cap == Paint.Cap.ROUND) Shape.ELLIPSE else Shape.RECTANGLE
    }

    private fun getStrokeCap(shape: Shape): Paint.Cap {
        return if (shape == Shape.ELLIPSE) Paint.Cap.ROUND else Paint.Cap.SQUARE
    }

    fun toggleEraser(isToggled: Boolean) {
        mIsErasing = isToggled
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

    override fun onDraw(canvas: Canvas) {
        super.onDraw(canvas)

        for ((key, value) in mPaths) {
            changePaint(value)
            canvas.drawPath(key, mPaint)
        }

        changePaint(mPaintOptions)
        canvas.drawPath(mPath, mPaint)
    }

    fun startTrace(drawPoint: Stroke) {
        val scaledPoint = getScaledPoint(drawPoint.StylusPoints[0])
        mStartX = scaledPoint.X.toFloat()
        mStartY = scaledPoint.Y.toFloat()
        mCurX = mStartX
        mCurY = mStartY
        setColor(Color.parseColor(drawPoint.DrawingAttributes.Color))
        setStrokeWidth(drawPoint.DrawingAttributes.Width.toFloat())
        setStrokeCap(getStrokeCap(drawPoint.DrawingAttributes.StylusTip))
        mPath.moveTo(mStartX, mStartY)
        mIsFirstMove = false
    }

    fun stopTrace() {
        val scaledPoint = getScaledPoint(StylusPoint(mCurX, mCurY))
        if (mStartX == scaledPoint.X && mStartY == scaledPoint.Y && scaledPoint.X != 0f && scaledPoint.Y != 0f) {
            mPath.lineTo(scaledPoint.X, scaledPoint.Y + 2)
            mPath.lineTo(scaledPoint.X + 1, scaledPoint.Y + 2)
            mPath.lineTo(scaledPoint.X + 1, scaledPoint.Y)
        }
        mPaths.put(mPath, mPaintOptions)
        mPath = MyPath()
        mPaintOptions = PaintOptions(
            mPaintOptions.color,
            mPaintOptions.strokeWidth,
            mPaintOptions.alpha,
            mPaintOptions.cap
        )
        mIsFirstMove = true
        postInvalidate()
    }

    fun addPath(drawPoint: StylusPoint) {
        val scaledPoint = getScaledPoint(drawPoint)
        if (!mIsErasing) {
            val x = scaledPoint.X.toFloat()
            val y = scaledPoint.Y.toFloat()
            if (mIsFirstMove) {
                startTrace(
                    Stroke(
                    DrawingAttributes(
                        String.format("#%8X", (0xFFFFFFFF and mPaintOptions.color.toLong())),
                        mPaintOptions.strokeWidth,
                        getStrokeCapShape(),
                        0
                    ),
                        arrayOf(drawPoint)
                    )
                )
            } else {
                mPath.quadTo(mCurX, mCurY, (x + mCurX) / 2, (y + mCurY) / 2)
                mCurX = x
                mCurY = y
            }
        } else {
            eraseStrokes(scaledPoint.X.toFloat(), scaledPoint.Y.toFloat())
        }
        postInvalidate()
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
        }
        mCurX = x
        mCurY = y

        if (!mIsErasing) {
            if (mPaintOptions.color.equals(ContextCompat.getColor(context!!, R.color.default_background))) {
                SocketHandler.startErasePoint()
            } else {
                val scaledPoint = returnScaledPoint(StylusPoint(mCurX.toInt(), mCurY.toInt()))
                SocketHandler.startStroke(
                    Stroke(
                        DrawingAttributes(
                            String.format("#%8X", (0xFFFFFFFF and mPaintOptions.color.toLong())),
                            mPaintOptions.strokeWidth,
                            getStrokeCapShape(),
                            0
                        ),
                        arrayOf(scaledPoint)
                    )
                )
            }
        } else {
            SocketHandler.startEraseStroke()
        }
    }

    private fun actionMove(x: Float, y: Float) {
        if (mIsErasing) {
            eraseStrokes(x, y)
        } else {
            mPath.quadTo(mCurX, mCurY, (x + mCurX) / 2, (y + mCurY) / 2)
            mCurX = x
            mCurY = y
        }
        val scaledPoint = returnScaledPoint(StylusPoint(x, y))
        SocketHandler.point(scaledPoint)
    }

    private fun eraseStrokes(x: Float, y: Float) {
        val rect = RectF(x - 10, y - 10, x + 10, y + 10)
        val toDelete: ArrayList<MyPath> = ArrayList()
        for (path in mPaths.entries) {
            val tempPath = Path()
            tempPath.moveTo(x, y)
            tempPath.addRect(rect, Path.Direction.CW)
            tempPath.op(path.key, Path.Op.INTERSECT)
            if (!tempPath.isEmpty
                && path.value.color != ContextCompat.getColor(context, R.color.default_background)) {
                toDelete.add(path.key)
            }
        }
        for (path in toDelete.iterator()) {
            mPaths.remove(path)
        }
    }

    private fun getScaledPoint(point: StylusPoint): StylusPoint {
        return StylusPoint((point.X.toFloat().div(600)) * this.width, (point.Y.toFloat().div(500)) * this.height)
    }

    private fun returnScaledPoint(point: StylusPoint): StylusPoint {
        return StylusPoint((point.X.toFloat().div(this.width)) * 600, (point.Y.toFloat().div(this.height)) * 500)
    }

    private fun actionUp() {
        mPath.lineTo(mCurX, mCurY)

        // draw a dot on click
        if (mStartX == mCurX && mStartY == mCurY) {
            mPath.lineTo(mCurX, mCurY + 2)
            mPath.lineTo(mCurX + 1, mCurY + 2)
            mPath.lineTo(mCurX + 1, mCurY)
        }

        if (!mIsErasing) {
            mPaths.put(mPath, mPaintOptions)
        }

        mPath = MyPath()
        mPaintOptions = PaintOptions(
            mPaintOptions.color,
            mPaintOptions.strokeWidth,
            mPaintOptions.alpha,
            mPaintOptions.cap
        )
    }



    override fun onTouchEvent(event: MotionEvent): Boolean {
        if (!isDrawer) {
            return false
        }
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