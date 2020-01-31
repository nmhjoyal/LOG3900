using System;
using System.Windows.Forms;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;

namespace SocketIoClient
{
    public delegate void UpdateTextBoxMethod(string text);
    public partial class Form1 : Form
    {
        public Socket socket;
        public User user;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            user = new User
            {
                username = textBox1.Text,
                password = "xxxxx"
            };

            createUser(user);

            socket = IO.Socket("http://localhost:5000");

            // Socket on connect send user + socket id to map on the server

            socket.On("new_message", (message) =>
            {
                Message newMessage = JsonConvert.DeserializeObject<Message>(message.ToString());
                Console.WriteLine(newMessage.author.username + " : " + newMessage.content);
            });

            socket.On("new_client", (socketId) =>
            {
                Console.WriteLine(socketId + " is connected");
            });
            // TestPOSTWebRequest(user);
            // TestGETWebRequest("Testing get...");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Message message = new Message
            {
                author = user,
                content = textBox2.Text
            };
            socket.Emit("send_message", JsonConvert.SerializeObject(message));
        }

            public static void createUser(User user)
        {
            TestPOSTWebRequest(user, "/user/add");
        }
        public static void TestPOSTWebRequest(Object obj, string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://localhost:5000" + url);
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

    public class Message
    {
        public User author;
        public string content;
    }
}
