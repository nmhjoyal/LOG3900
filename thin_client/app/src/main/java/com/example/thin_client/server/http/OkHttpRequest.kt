import okhttp3.OkHttpClient
import okhttp3.Callback
import okhttp3.Call
import okhttp3.Request
import okhttp3.RequestBody.Companion.toRequestBody
import okhttp3.MediaType.Companion.toMediaType

class OkHttpRequest(private val client: OkHttpClient) {

    fun GET(url: String, callback: Callback): Call {
        val request = Request.Builder()
            .url(url)
            .build()

        val call = client.newCall(request)
        call.enqueue(callback)
        return call
    }
    
    fun POST(url: String, jsonStr: String, callback: Callback): Call {
        val body = jsonStr.toRequestBody(JSONMediaType)

        val request = Request.Builder()
            .url(url)
            .post(body)
            .build()

        val call = client.newCall(request)
        call.enqueue(callback)
        return call
    }

    fun PUT(url: String, jsonStr: String, callback: Callback): Call {
        val body = jsonStr.toRequestBody(JSONMediaType)

        val request = Request.Builder()
            .url(url)
            .put(body)
            .build()

        val call = client.newCall(request)
        call.enqueue(callback)
        return call
    }

    fun DELETE(url: String, callback: Callback): Call {
        val request = Request.Builder()
            .url(url)
            .delete()
            .build()

        val call = client.newCall(request)
        call.enqueue(callback)
        return call
    }

    companion object {
        val JSONMediaType = "application/json; charset=utf-8".toMediaType()
    }


}
// Ref : https://medium.com/@rohan.s.jahagirdar/android-http-requests-in-kotlin-with-okhttp-5525f879b9e5