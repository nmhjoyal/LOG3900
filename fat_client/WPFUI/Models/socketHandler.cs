using System;
using System.IO;
using System.Net;
using Caliburn.Micro;
using Newtonsoft.Json;
using Quobject.SocketIoClientDotNet.Client;
using Socket = Quobject.SocketIoClientDotNet.Client.Socket;
using WPFUI.EventModels;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;

namespace WPFUI.Models
{
    public partial class SocketHandler : Window, ISocketHandler
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

        public Socket socket { get => this._socket; set => throw new NotImplementedException(); }

        public SocketHandler(IUserData userdata, IEventAggregator events)
        {
            _userdata = userdata;
            _events = events;
            // TestPOSTWebRequest(user);
            // TestGETWebRequest("Testing get...");
            this._socket = IO.Socket("http://localhost:5000");
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

            
            _trait = new Trait(path,couleur,width,stylusTip);
            Console.WriteLine(_trait.ToString());
            this._traitJSON = JsonConvert.SerializeObject(_trait);
            Console.WriteLine(_traitJSON.ToString());

            this._socket.Emit("sent_path", this._traitJSON);

        }

        /*public void getStrokes(InkCanvas Canvas)
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
        }*/

        public void freeDraw(StrokeCollection Traits, DrawingAttributes AttributsDessin)
        {
            this.socket.Emit("connect_free_draw");

            this.socket.On("new_trace", (trace) => {
                dynamic json = JsonConvert.DeserializeObject(trace.ToString());
                StylusPoint stylusPoint = new StylusPoint((int)json.point.x, (int)json.point.y);
                StylusPointCollection stylusPointCollection = new StylusPointCollection();
                stylusPointCollection.Add(stylusPoint);
                Stroke stroke = new Stroke(stylusPointCollection);
                stroke.DrawingAttributes.Width = json.width;
                stroke.DrawingAttributes.Height = json.width;
                stroke.DrawingAttributes.StylusTip = StylusTip.Ellipse;
                stroke.DrawingAttributes.Color = (System.Windows.Media.Color)ColorConverter.ConvertFromString(new Color((int)json.color.r, (int)json.color.g, (int)json.color.b).getHex());

                this.Dispatcher.Invoke(() =>
                    Traits.Add(stroke)
                );

                /*
                StylusPoint stylusPoint = new StylusPoint((double)json.x, json.y);

                Stroke stroke = new Stroke()
                Console.WriteLine(point);
                */
            });

            this.socket.On("drawPoint", (point) => {
                dynamic json = JsonConvert.DeserializeObject(point.ToString());
                StylusPoint stylusPoint = new StylusPoint((int)json.x, (int)json.y);

                this.Dispatcher.Invoke(() =>
                    Traits[Traits.Count - 1].StylusPoints.Add(stylusPoint)
                );

                /*
                StylusPoint stylusPoint = new StylusPoint((double)json.x, json.y);

                Stroke stroke = new Stroke()
                Console.WriteLine(point);
                */
            });
        }
    }

}


