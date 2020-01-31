using System;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;
using Socket = Quobject.SocketIoClientDotNet.Client.Socket;
using System.Configuration;

namespace SocketIoClient
{
    public delegate void UpdateTextBoxMethod(string text);
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
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
            User user = new User {
                username = "zakari",
                password = "banane"
            };
            TestPOSTWebRequest(user);
            */
            TestGETWebRequest("Testing get...");
            socketIoManager();
        }

        private void socketIoManager()
        {
            var socket = IO.Socket("http://localhost:5000");
            socket.On(Socket.EVENT_CONNECT, () =>
            {
                Console.WriteLine("Connected");
            });
            socket.Emit("test");
        }

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
}
