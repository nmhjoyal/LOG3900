package com.example.thin_client.data

import android.app.Activity
import android.content.Context
import android.content.pm.PackageManager
import androidx.core.app.ActivityCompat
import androidx.core.content.ContextCompat

object PermissionHandler {

    fun hasStoragePermission(context: Context): Boolean {
        return ContextCompat.checkSelfPermission(context, android.Manifest.permission.WRITE_EXTERNAL_STORAGE) ==
                PackageManager.PERMISSION_GRANTED
    }

    fun requestStoragePermission(activity: Activity) {
        ActivityCompat.requestPermissions(activity,
            arrayOf(android.Manifest.permission.WRITE_EXTERNAL_STORAGE),
            RequestCodes.WRITE_EXTERNAL_STORAGE.ordinal)
    }
}