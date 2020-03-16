package com.example.thin_client.data
import android.widget.ImageView
import com.example.thin_client.R
import java.lang.IllegalArgumentException

enum class AvatarID {
    AVOCADO,
    BANANA,
    STRAWBERRY,
    WATERMELON,
    GRAPE,
    KIWI,
    ORANGE,
    PINEAPPLE,
    APPLE,
    LEMON,
    CHERRY,
    PEAR
}

fun setAvatar(imgResource: ImageView, avatarID: AvatarID) {
    when (avatarID) {
        AvatarID.PEAR -> imgResource.setImageResource(R.drawable.ic_pear)
        AvatarID.CHERRY -> imgResource.setImageResource(R.drawable.ic_cherry)
        AvatarID.LEMON -> imgResource.setImageResource(R.drawable.ic_lemon)
        AvatarID.APPLE -> imgResource.setImageResource(R.drawable.ic_apple)
        AvatarID.PINEAPPLE -> imgResource.setImageResource(R.drawable.ic_pineapple)
        AvatarID.ORANGE -> imgResource.setImageResource(R.drawable.ic_orange)
        AvatarID.KIWI -> imgResource.setImageResource(R.drawable.ic_kiwi)
        AvatarID.GRAPE -> imgResource.setImageResource(R.drawable.ic_grape)
        AvatarID.WATERMELON -> imgResource.setImageResource(R.drawable.ic_watermelon)
        AvatarID.STRAWBERRY -> imgResource.setImageResource(R.drawable.ic_strawberry)
        AvatarID.BANANA -> imgResource.setImageResource(R.drawable.ic_banana)
        AvatarID.AVOCADO -> imgResource.setImageResource(R.drawable.ic_avocado)
    }
}

fun getAvatar(avatar: String?): AvatarID {
    if (avatar !== null) {
        try {
            return AvatarID.valueOf(avatar)
        } catch (e: IllegalArgumentException) {
            return AvatarID.AVOCADO
        }
    }
    return AvatarID.AVOCADO
}