using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Caliburn.Micro;
using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;
using Socket = Quobject.SocketIoClientDotNet.Client.Socket;
using WPFUI.EventModels;
using System.Windows.Media;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;

namespace WPFUI.Models
{
    public partial class SocketHandler : ISocketHandler
    {
        public IUserData _userdata;
        public IEventAggregator _events;
        public User _user;
        public Trait _trait;
        public string _userJSON;
        Socket _socket;
        public bool _canConnect;
        private string _traitJSON;


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


        public string traitJSon
        {
            get { return _traitJSON; }
            set { _traitJSON = value; }
        }

        public Socket socket { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SocketHandler(IUserData userdata, IEventAggregator events)
        {
            _userdata = userdata;
            _events = events;
            // TestPOSTWebRequest(user);
            // TestGETWebRequest("Testing get...");
            this._socket = IO.Socket("http://localhost:5000");
            _socket.On("user_signed_in", (signInFeedback) =>
            {
                SignInFeedback feedback = JsonConvert.DeserializeObject<SignInFeedback>(signInFeedback.ToString());
            if (feedback.feedback.status)
            {
                _events.PublishOnUIThread(new joinedRoomReceived(feedback.rooms_joined));
                _userdata.avatarName = feedback.rooms_joined.Single(i => i.roomName == "General").avatars[_userdata.userName];
                Console.WriteLine("fruit:");
                Console.WriteLine(_userdata.avatarName);
                _events.PublishOnUIThread(new LogInEvent());

                }
                //voir doc
            });

            _socket.On("new_message", (message) =>
             {
                 Message newMessage = JsonConvert.DeserializeObject<Message>(message.ToString());
                 Console.WriteLine("message received");
                 _userdata.messages.Add(newMessage);
             });

            /*_socket.On("new_client", (socketId) =>
            {
                MessageModel newMessageModel = new MessageModel("Nouvelle connection de: " + socketId.ToString(), "Server");
                _userdata.messages.Add(newMessageModel);
                ///Console.WriteLine(socketId + " is connected");
            });*/

            _socket.On("user_signed_out", (feedback) =>
            {
                dynamic json = JsonConvert.DeserializeObject(feedback.ToString());
                if ((Boolean)json.status)
                {
                    _events.PublishOnUIThread(new logOutEvent());
                }
                //voir doc
            });

            _socket.On("user_joined_room", (feedback) =>
            {
                JoinRoomFeedBack fb = JsonConvert.DeserializeObject<JoinRoomFeedBack>(feedback.ToString());
                if (fb.feedback.status & fb.joinedRoom != null)
                {
                    _userdata.addRoom(fb.joinedRoom);
                }
                //voir doc
            });

            _socket.On("rooms_retrieved", (feedback) =>
            {
                Console.WriteLine("reception des public rooms");
                dynamic json = JsonConvert.DeserializeObject(feedback.ToString());
                string[] publicRooms = json.ToObject<string[]>();
                Console.WriteLine("nb de room publiques:");
                Console.WriteLine(publicRooms.Length);
                _events.PublishOnUIThread(new roomsRetrievedEvent(publicRooms));
            });

            _socket.On("room_created", (feedback) =>
            {
                Feedback json = JsonConvert.DeserializeObject<Feedback>(feedback.ToString());
                if (json.status)
                {
                    getPublicChannels();
                    _events.PublishOnUIThread(new createTheRoomEvent());
                }
            });
        }

        public void createRoom(string roomID)
        {
            CreateRoom cR = new CreateRoom(roomID, false);
            _socket.Emit("create_chat_room", JsonConvert.SerializeObject(cR));
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
        public void sendMessage()
        {
            ClientMessage message = new ClientMessage(_userdata.currentMessage, _userdata.currentRoomId);

            if (message.content.Trim() != "")
            {
                _socket.Emit("send_message", JsonConvert.SerializeObject(message));
            }
        }

        public void createUser(PrivateProfile privateProfile)
        {
            TestPOSTWebRequest(privateProfile, "/profile/create/");
        }

        public void getPublicChannels()
        {
            _socket.Emit("get_rooms");
            Console.WriteLine("demande des public rooms");
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

        public void sendStroke(string path, string couleur, string width, bool stylusTip)
        {
            _trait = new Trait(path, couleur, width, stylusTip);
            Console.WriteLine(_trait.ToString());
            this._traitJSON = JsonConvert.SerializeObject(_trait);
            Console.WriteLine(_traitJSON.ToString());
            this._socket.Emit("sent_path", this._traitJSON);
        }
        public void getStrokes(InkCanvas Canvas)
        {
            _socket.On("receive_path", (response) =>
            {
                Trait json = JsonConvert.DeserializeObject<Trait>(response.ToString());
                System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
                path.Data = Geometry.Parse(json.path);
                path.StrokeThickness = 1;
                path.Stroke = System.Windows.Media.Brushes.Blue;
                path.StrokeEndLineCap = PenLineCap.Round;
                path.StrokeStartLineCap = PenLineCap.Round;
                path.StrokeLineJoin = PenLineJoin.Round;
                Canvas.Children.Add(path);
            });
        }

    }

}


