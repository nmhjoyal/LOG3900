import okhttp3.OkHttpClient
import okhttp3.Callback
import okhttp3.Call
import okhttp3.Request
import okhttp3.RequestBody.Companion.toRequestBody
import okhttp3.MediaType.Companion.toMediaType

class OkHttpRequest( var client: OkHttpClient) {

    fun GET(url: String, callback: Callback): Call {
        val request = Request.Builder()
            .url(url)
            .build()

        val call = client.newCall(request)
        call.enqueue(callback)
        return call
    }


}
// Ref : https://medium.com/@rohan.s.jahagirdar/android-http-requests-in-kotlin-with-okhttp-5525f879b9e5