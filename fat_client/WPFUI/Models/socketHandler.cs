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
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;

namespace WPFUI.Models
{
    public partial class SocketHandler : Window, ISocketHandler
    {
        public IUserData _userdata;
        public IEventAggregator _events;
        public User _user;
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
                Console.WriteLine("3");
                dynamic magic = JsonConvert.DeserializeObject(feedback.ToString());
                if (magic.room_joined != null & magic.isPrivate != null)
                {
                    JoinRoomFeedBack fb = JsonConvert.DeserializeObject<JoinRoomFeedBack>(feedback.ToString());
                    if (fb.feedback.status & fb.joinedRoom != null)
                    {
                        Console.WriteLine("user_joined_room :");
                        Console.WriteLine(fb.joinedRoom.id);

                        if (fb.isPrivate)
                        {
                            _userdata.addJoinedRoom(fb.joinedRoom);
                        }
                        else
                        {
                            _userdata.addJoinedRoom(fb.joinedRoom);
                        }
                    }
                    else if (!fb.feedback.status)
                    {
                        Console.WriteLine(fb.feedback.log_message);
                    }
                } else
                {
                    Console.WriteLine(magic.feedback.log_message);
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
                Console.WriteLine("Room created !!!");
                if (json.status)
                {
                    Console.WriteLine(json.log_message);
                    getPublicChannels();
                }
            });
        }

        public void createRoom(string roomID)
        {
            CreateRoom cR = new CreateRoom(roomID, false);
            _socket.Emit("create_chat_room", JsonConvert.SerializeObject(cR));
        }

        public void joinRoom(string roomID)
        {
            Console.WriteLine("tentive de join de : " + roomID);
            _socket.Emit("join_chat_room", JsonConvert.SerializeObject(roomID));
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
        public void TestPOSTWebRequest(Object obj, string url)
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

        public void TestGETWebRequest(string request)
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
        public void onDrawing(StrokeCollection Traits, Dictionary<Stroke, int> strokes)
        {
            string drawersTool = "";
            int currentStrokeIndex = -1;

            this.socket.On("new_stroke", (new_stroke) =>
            {
                Console.WriteLine("new stroke");
                dynamic json = JsonConvert.DeserializeObject(new_stroke.ToString());
                drawersTool = "crayon";
                StylusPoint stylusPoint = new StylusPoint((int)json.StylusPoints[0].X, (int)json.StylusPoints[0].Y);
                StylusPointCollection stylusPointCollection = new StylusPointCollection();
                stylusPointCollection.Add(stylusPoint);
                Stroke stroke = new Stroke(stylusPointCollection);
                stroke.DrawingAttributes.Width = json.DrawingAttributes.Width;
                stroke.DrawingAttributes.Height = json.DrawingAttributes.Width;
                stroke.DrawingAttributes.StylusTip = (StylusTip)json.DrawingAttributes.StylusTip;
                string color = json.DrawingAttributes.Color;
                stroke.DrawingAttributes.Color = (System.Windows.Media.Color)ColorConverter.ConvertFromString(color.Remove(1, 2));
                int top = json.DrawingAttributes.Top;
                if (Traits.Count == 0)
                {
                    this.Dispatcher.Invoke(() =>
                            Traits.Add(stroke)
                        );
                    currentStrokeIndex = 0;
                }
                else
                {
                    for (int i = Traits.Count - 1; i >= 0; i--)
                    {
                        if (strokes[Traits[i]] <= top)
                        {
                            this.Dispatcher.Invoke(() =>
                                Traits.Insert(i + 1, stroke)
                            );
                            currentStrokeIndex = i + 1;
                            break;
                        }
                        else if (i == 0)
                        {
                            this.Dispatcher.Invoke(() =>
                                Traits.Insert(i, stroke)
                            );
                            currentStrokeIndex = i;
                        }
                    }
                }
                strokes.Add(stroke, top);
            });

            this._socket.On("new_erase_stroke", () =>
            {
                drawersTool = "efface_trait";
            });

            this._socket.On("new_erase_point", () =>
            {
                drawersTool = "efface_segment";
            });

            this.socket.On("new_point", (new_point) =>
            {
                dynamic json = JsonConvert.DeserializeObject(new_point.ToString());
                if (drawersTool == "crayon")
                {
                    StylusPoint stylusPoint = new StylusPoint((int)json.X, (int)json.Y);
                    this.Dispatcher.Invoke(() =>
                        Traits[currentStrokeIndex].StylusPoints.Add(stylusPoint)
                    );
                }
                else if (drawersTool == "efface_trait" || drawersTool == "efface_segment")
                {
                    StrokeCollection erasedStrokes = new StrokeCollection();
                    System.Windows.Point point = new System.Windows.Point((double)json.X, (double)json.Y);
                    this.Dispatcher.Invoke(() =>
                        erasedStrokes = Traits.HitTest(point, 8)
                    );

                    if (drawersTool == "efface_segment")
                    {
                        for (int i = 0; i < erasedStrokes.Count; i++)
                        {
                            StrokeCollection segments = new StrokeCollection(erasedStrokes[i].GetEraseResult(new List<System.Windows.Point>() { point }, new RectangleStylusShape(8, 8)));
                            int index = Traits.IndexOf(erasedStrokes[i]);
                            for (int j = 0; j < segments.Count; j++)
                            {
                                this.Dispatcher.Invoke(() =>
                                    Traits.Insert(index, segments[j])
                                );
                                strokes.Add(segments[j], strokes[erasedStrokes[i]]);
                            }
                        }

                    }

                    for (int i = 0; i < erasedStrokes.Count; i++)
                    {
                        this.Dispatcher.Invoke(() =>
                            Traits.Remove(erasedStrokes[i])
                        );
                        strokes.Remove(erasedStrokes[i]);
                    }
                }
            });

            this._socket.On(("new_clear"), () =>
            {
                this.Dispatcher.Invoke(() =>
                {
                    Traits.Clear();
                });
                strokes.Clear();
            });
        }

        public void offDrawing()
        {
            this._socket.Off("new_stroke");
            this._socket.Off("new_erase_point");
            this._socket.Off("new_erase_stroke");
            this._socket.Off("new_point");
            this._socket.Off("clear");
        }

        public void offPreviewing()
        {
            this._socket.Off("preview_done");
        }
    }

}


