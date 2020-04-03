package com.example.thin_client.ui.helpers

import android.os.SystemClock
import android.view.View
import java.util.*


abstract class DebounceClickListener(val minimumIntervalMillis: Long): View.OnClickListener {

    private var lastClickMap: MutableMap<View, Long>? = null

    init {
        lastClickMap = WeakHashMap()
    }

    abstract fun onDebouncedClick(v: View?)

    override fun onClick(clickedView: View) {
        val previousClickTimestamp: Long? = lastClickMap?.get(clickedView)
        val currentTimestamp = SystemClock.uptimeMillis()
        lastClickMap!!.put(clickedView, currentTimestamp)
        if (previousClickTimestamp == null || Math.abs(currentTimestamp - previousClickTimestamp) > minimumIntervalMillis) {
            onDebouncedClick(clickedView)
        }
    }
}