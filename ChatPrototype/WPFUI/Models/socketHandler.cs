using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;
using Socket = Quobject.SocketIoClientDotNet.Client.Socket;

namespace WPFUI.Models
{
    public partial class SocketHandler : ISocketHandler
    {
        public IUserData _userdata;
        public User _user;
        public string _userJSON;
        Socket _socket;

        public User user
        {
            get { return _user; }
            set { _user = value; }
        }

        public Socket socket { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SocketHandler(IUserData userdata)
        {
            _userdata = userdata;
            // TestPOSTWebRequest(user);
            // TestGETWebRequest("Testing get...");
        }

        public void connectionAttempt()
        {

            _user = new User(_userdata.userName, "hubert");

            this._userJSON = JsonConvert.SerializeObject(_user);

            this._socket = IO.Socket("http://10.200.26.21:5000");

            this._socket.On(Socket.EVENT_CONNECT, () =>
            {
                this._socket.Emit("sign_in", this._userJSON);
            });

            _socket.On("user_signed_in", (connected) =>
            {
                // bool canConnect = JsonConvert.DeserializeObject<bool>(connected.ToString());
                _socket.Emit("join_chat_room");
            });

            _socket.On("new_message", (message) =>
            {
                Message newMessage = JsonConvert.DeserializeObject<Message>(message.ToString());
                MessageModel newMessageModel = new MessageModel(newMessage.content, newMessage.author.username, DateTime.Now);
                _userdata.messages.Add(newMessageModel);
            });

            _socket.On("new_client", (socketId) =>
            {
                MessageModel newMessageModel = new MessageModel("Nouvelle connection de: " + socketId.ToString(), "Server", DateTime.Now);
                _userdata.messages.Add(newMessageModel);
                ///Console.WriteLine(socketId + " is connected");
            });
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


