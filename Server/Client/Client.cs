using System;
using System.Net;  
using System.Net.Sockets;  
using System.Text; 
public class Client {  
    public static void Main(String[] args) {  
        byte[] bytes = new byte[1024];  
        Socket sender = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        System.Net.IPEndPoint remoteEP = new IPEndPoint(System.Net.IPAddress.Parse("127.0.0.1"), 5000);
        sender.Connect(remoteEP);  
        Console.WriteLine("Socket connected to {0}", sender.RemoteEndPoint.ToString());  
        // Encode the data string into a byte array.    
        // byte[] msg = Encoding.ASCII.GetBytes("abcdef");
        string msg = "abcdef";
        // Send the data through the socket.    
        int bytesSent = sender.Send(msg);
        // Receive the response from the remote device.
        int bytesRec = sender.Receive(bytes);   
        // Release the socket.    
        sender.Shutdown(SocketShutdown.Both);  
        sender.Close();  
    }
}

// C:\Windows\Microsoft.NET\Framework64\v4.0.30319\