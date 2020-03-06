using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Caliburn.Micro;
using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;
using Socket = Quobject.SocketIoClientDotNet.Client.Socket;
using WPFUI.EventModels;

namespace WPFUI.Models
{
    public partial class SocketHandler : ISocketHandler1
    {
        public IUserData _userdata;
        public IEventAggregator _events;
        public User _user;
        public string _userJSON;
        Socket _socket;
        public bool _canConnect;

        public bool canConnect
        {
            get { return _canConnect; }
            set
            {
                _canConnect = value;
                if (_canConnect)
                {
                    _socket.Emit("join_chat_room");
                    _events.PublishOnUIThread(new LogInEvent());
                }
                else { _events.PublishOnUIThread(new userNameTakenEvent()); };
                ;
            }
        }
        public User user
        {
            get { return _user; }
            set { _user = value; }
        }

        public Socket socket { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SocketHandler(IUserData userdata, IEventAggregator events)
        {
            _userdata = userdata;
            _events = events;
            // TestPOSTWebRequest(user);
            // TestGETWebRequest("Testing get...");
        }

        public void connectionAttempt()
        {
            var options = new IO.Options();
            options.Reconnection = false;
            this._socket = IO.Socket("http://10.200.9.99:5000", options);
            _user = new User(_userdata.userName, _userdata.password);
            Console.WriteLine(_user);
            Console.WriteLine("hello");
            this._userJSON = JsonConvert.SerializeObject(_user);
            Console.WriteLine(this._userJSON);
            Console.WriteLine("hello2");
            this._socket.On(Socket.EVENT_CONNECT, () =>
            {
                this._socket.Emit("sign_in", this._userJSON);

            });

            _socket.On("user_signed_in", (connected) =>
            {
                canConnect = JsonConvert.DeserializeObject<bool>(connected.ToString());
            });

            _socket.On("new_message", (message) =>
            {
                Message newMessage = JsonConvert.DeserializeObject<Message>(message.ToString());
                Console.WriteLine(newMessage.date);
                MessageModel newMessageModel = new MessageModel(newMessage.content, newMessage.author.username,
                                                                newMessage.date);
                _userdata.messages.Add(newMessageModel);
            });

            _socket.On("new_client", (socketId) =>
            {
                MessageModel newMessageModel = new MessageModel("Nouvelle connection de: " + socketId.ToString(), "Server");
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
            Message message = new Message(_user, _userdata.currentMessage, 0);
            if (message.content.Trim() != "")
            {
                _socket.Emit("send_message", JsonConvert.SerializeObject(message));
            }
        }

        public void createUser(PrivateProfile privateProfile)
        {
            Console.WriteLine(privateProfile.firstname);
            try
            {
                TestPOSTWebRequest(privateProfile, "/profile/create/");

            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);

            }

        }
        public static void TestPOSTWebRequest(Object obj, string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://" + "10.200.23.33" + ":5000" + url);
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


