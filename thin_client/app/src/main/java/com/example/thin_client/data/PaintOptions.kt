package com.example.thin_client.data

import android.graphics.Color
import android.graphics.Paint

data class PaintOptions(var color: Int = Color.BLACK,
                        var strokeWidth: Float = 8f,
                        var alpha: Int = 255,
                        var cap: Paint.Cap = Paint.Cap.ROUND)
