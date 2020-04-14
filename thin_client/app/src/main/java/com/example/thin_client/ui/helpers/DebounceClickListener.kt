package com.example.thin_client.ui.helpers

import android.view.View

const val DEFAULT_INTERVAL = 500L

class DebounceClickListener(private val interval: Long = DEFAULT_INTERVAL,
                            private val listenerBlock: (View) -> Unit): View.OnClickListener {

    private var lastClickTime = 0L

    override fun onClick(v: View) {
        val time = System.currentTimeMillis()
        if (time - lastClickTime >= interval) {
            lastClickTime = time
            listenerBlock(v)
        }
    }
}

fun View.setOnClickListener(debounceInterval: Long, listenerBlock: (View) -> Unit) =
    setOnClickListener(DebounceClickListener(debounceInterval, listenerBlock))