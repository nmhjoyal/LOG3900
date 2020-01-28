using System;
using System.Net;  
using System.Net.Sockets;  
using System.Text;
using System.IO;
using System.Web.Script.Serialization;

public class Client {  
    public static void Main(String[] args) {
        User user = new User {
            username = "zakari",
            password = "banane123"
        };
        TestPOSTWebRequest(new JavaScriptSerializer().Serialize(user));
        TestGETWebRequest("Testing get...");
    }

    public static void sendSocket() {
        byte[] bytes = new byte[1024];  
        Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        System.Net.IPEndPoint remoteEP = new IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 5000);
        sender.Connect(remoteEP);  
        Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());  
        // Encode the data string into a byte array.    
        byte[] msg = Encoding.ASCII.GetBytes("abcdefg");
        // string msg = "abcdef";
        // Send the data through the socket.    
        int bytesSent = sender.Send(msg);
        // Receive the response from the remote device.
        int bytesRec = sender.Receive(bytes);
        string res = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
        // Release the socket.    
        sender.Shutdown(SocketShutdown.Both);  
        sender.Close();  
        Console.WriteLine("\nbytes received : " + res);
    }
    public static void TestPOSTWebRequest(string json){
        var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:5000/user/");
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "POST";

        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream())){
           streamWriter.Write(json);
           streamWriter.Flush();
           streamWriter.Close();
        }

        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream())){
            var result = streamReader.ReadToEnd();
            Console.WriteLine (result);
        }
    }

    public static void TestGETWebRequest(string request){
        var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:5000/user/" + request);
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "GET";

        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream())){
            var result = streamReader.ReadToEnd();
            Console.WriteLine (result);
        }
    }
}

public class User {
    public string username;
    public string password;
}

// C    :\Windows\Microsoft.NET\Framework64\v4.0.30319\