using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;
using Newtonsoft.Json;

public class Client
{
    public static void Main(String[] args)
    {
        /*
        Console.WriteLine("Enter your username");
        string username = Console.ReadLine();
        Console.WriteLine("Enter your password");
        string password = Console.ReadLine();
        User user = new User {
            username = username,
            password = password
        };
        bool disconnected = false;
        while(!disconnected) {
            Console.WriteLine("Enter a command : ");
            string command = Console.ReadLine();
            switch(command) {
                case "join" : join(); // post
                case "leave" : leave(); // delete
                case "get users" : getUsers(); // get
                case "exit" : disconnected = true;
            }
        }
        */
        User user = new User
        {
            username = "zakari",
            password = "banane"
        };
        TestPOSTWebRequest(user);
        TestGETWebRequest("Testing get...");
    }

    public static void sendSocket()
    {
        Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 5000));
        socket.Listen(0);
        Console.WriteLine("1");
        var client = socket.Accept();
        Console.WriteLine("2");
        var buffer = Encoding.UTF8.GetBytes("Hello server");
        client.Send(buffer, 0, buffer.Length, 0);
        buffer = new byte[255];
        int rec = client.Receive(buffer, 0, buffer.Length, 0);
        Array.Resize(ref buffer, rec);
        Console.WriteLine("Received : " + Encoding.UTF8.GetString(buffer));
        client.Close();
        socket.Close();
    }
    /*
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
    */
    public static void TestPOSTWebRequest(Object obj)
    {
        var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:5000/user/");
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "POST";

        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        {
            streamWriter.Write(JsonConvert.SerializeObject(obj));
            streamWriter.Flush();
            streamWriter.Close();
        }

        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            var result = streamReader.ReadToEnd();
            Console.WriteLine(result);
        }
    }

    public static void TestGETWebRequest(string request)
    {
        var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:5000/user/" + request);
        httpWebRequest.ContentType = "application/json";
        httpWebRequest.Method = "GET";

        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            var result = streamReader.ReadToEnd();
            Console.WriteLine(result);
        }
    }
}

public class User
{
    public string username;
    public string password;
}

// C    :\Windows\Microsoft.NET\Framework64\v4.0.30319\
