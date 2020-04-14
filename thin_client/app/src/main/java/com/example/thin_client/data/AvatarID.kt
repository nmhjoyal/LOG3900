package com.example.thin_client.data
import android.widget.ImageView
import com.example.thin_client.R
import com.example.thin_client.data.rooms.RoomManager
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

const val MR_AVOCADO = "Mr Avocado"
const val LORD_BANANA = "Lord Banana"
const val SGT_STRAWBERRY = "Sgt Strawberry"
const val LADY_WATERMELON = "Lady Watermelon"
const val MASTER_GRAPE = "Master Grape"
const val GENTLEMAN_KIWI = "Gentleman Kiwi"
const val MADAM_ORANGE = "Madam Orange"
const val SIR_PINEAPPLE = "Sir Pineapple"
const val LITTLE_APPLE = "Little Apple"
const val MISS_LEMON = "Miss Lemon"
const val DRE_CHERRY = "Dre Cherry"
const val PIRATE_PEAR = "Pirate Pear"

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

fun getPlayerAvatar(username: String): AvatarID {
    return when(username) {
        MR_AVOCADO -> AvatarID.AVOCADO
        LORD_BANANA -> AvatarID.BANANA
        SGT_STRAWBERRY -> AvatarID.STRAWBERRY
        LADY_WATERMELON -> AvatarID.WATERMELON
        MASTER_GRAPE -> AvatarID.GRAPE
        GENTLEMAN_KIWI -> AvatarID.KIWI
        MADAM_ORANGE -> AvatarID.ORANGE
        SIR_PINEAPPLE -> AvatarID.PINEAPPLE
        LITTLE_APPLE -> AvatarID.APPLE
        MISS_LEMON -> AvatarID.LEMON
        DRE_CHERRY -> AvatarID.CHERRY
        PIRATE_PEAR -> AvatarID.PEAR
        else -> {
            val avatarList = RoomManager.roomAvatars["General"]
            if (avatarList !== null) {
                return getAvatar(avatarList[username])
            }
            return AvatarID.AVOCADO
        }
    }
}