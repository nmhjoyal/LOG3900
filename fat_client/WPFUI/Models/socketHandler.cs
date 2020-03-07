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
    public partial class SocketHandler : ISocketHandler
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
            this._socket = IO.Socket("http://192.168.0.152:5000");
            _socket.On("user_signed_in", (signInFeedback) =>
            {
                Console.WriteLine("hello");
                dynamic json = JsonConvert.DeserializeObject(signInFeedback.ToString());
                if ((Boolean)json.feedback.status)
                {
                    Console.WriteLine("connect");
                    _events.PublishOnUIThread(new LogInEvent());
                }
                else
                {
                    Console.WriteLine("cant connect");
                }
                //voir doc
            });

            /* _socket.On("new_message", (message) =>
             {
                 Message newMessage = JsonConvert.DeserializeObject<Message>(message.ToString());
                 Console.WriteLine(newMessage.date);
                 MessageModel newMessageModel = new MessageModel(newMessage.content, newMessage.author.username,
                                                                 newMessage.date);
                 _userdata.messages.Add(newMessageModel);
             });*/

            _socket.On("new_client", (socketId) =>
            {
                MessageModel newMessageModel = new MessageModel("Nouvelle connection de: " + socketId.ToString(), "Server");
                _userdata.messages.Add(newMessageModel);
                ///Console.WriteLine(socketId + " is connected");
            });

            _socket.On("user_signed_out", (feedback) =>
            {
                dynamic json = JsonConvert.DeserializeObject(feedback.ToString());
                if ((Boolean)json.status)
                {
                    _events.PublishOnUIThread(new logOutEvent());
                }
                //voir doc
            });

        }

        public void connectionAttempt()
        {
            

            _user = new User(_userdata.userName, _userdata.password);

            this._userJSON = JsonConvert.SerializeObject(_user);

            Console.WriteLine(this._userJSON);

            this._socket.Emit("sign_in", this._userJSON);

           
        }
        public void SignOut()
        {
            this._socket.Emit("sign_out");  
        }
        public void disconnect()
        {
            _socket.Disconnect();
        }
       /* public void sendMessage()
        {
            Message message = new Message(_user, _userdata.currentMessage, 0);
            if(message.content.Trim() != "")
            {
                _socket.Emit("send_message", JsonConvert.SerializeObject(message));
            }
        }*/

        public void createUser(PrivateProfile privateProfile)
        { 
                TestPOSTWebRequest(privateProfile, "/profile/create/");

        }
        public static void TestPOSTWebRequest(Object obj, string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://192.168.0.152:5000" + url);
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
            var httpWebRequest = (HttpWebRequest)WebRequest.Create("http://192.168.0.152:5000/user/" + request);
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


