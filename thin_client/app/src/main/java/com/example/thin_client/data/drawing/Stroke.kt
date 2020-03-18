package com.example.thin_client.data.drawing

data class Stroke(
    val DrawingAttributes: DrawingAttributes,
    val StylusPoints: Array<StylusPoint>
)