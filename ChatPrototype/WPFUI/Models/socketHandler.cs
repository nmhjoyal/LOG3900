using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;
using Caliburn.Micro;
using WPFUI.EventModels;
using WPFUI.Models;

namespace WPFUI.Models
{
    public class SocketHandler : ISocketHandler
    {
        private IUserData _userdata;
        private User _user;
        private Socket _socket;

        public User user
        {
            get { return _user; }
            set { _user = value; }
        }

        public Socket socket
        {
            get { return _socket; }
            set { _socket = value; }
        }


        public SocketHandler(IUserData userdata)
        {
            _userdata = userdata;
        }

        public void connectionAttempt()
        {
            _user = new User(_userdata.userName, "xxxxx");

            _socket = IO.Socket("http://localhost:5000");

            // Socket on connect send user + socket id to map on the server

            _socket.On("new_message", (message) =>
            {
                Message newMessage = JsonConvert.DeserializeObject<Message>(message.ToString());
                MessageModel newMessageModel = new MessageModel(newMessage.content, newMessage.author.username, DateTime.Now);
                _userdata.messages.Add(newMessageModel);
                //Console.WriteLine(newMessage.author.username + " : " + newMessage.content);
            });

            _socket.On("new_client", (socketId) =>
            {
                MessageModel newMessageModel = new MessageModel("Nouvelle connection de: " + socketId.ToString(), "Server", DateTime.Now);
                _userdata.messages.Add(newMessageModel);
                ///Console.WriteLine(socketId + " is connected");
            });
            // TestPOSTWebRequest(user);
            // TestGETWebRequest("Testing get...");
        }

        public void disconnect()
        {
            _socket.Disconnect();
        }
        public void sendMessage()
        {
            Message message = new Message(_userdata.currentMessage, _user);
            _socket.Emit("send_message", JsonConvert.SerializeObject(message));

        }

        public void createUser(User user)
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

}


