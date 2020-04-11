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
        private string _roomToBeCreated;
        private string baseURL;


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
            this.baseURL = "http://localhost:5000";
            _userdata = userdata;
            _events = events;
            _roomToBeCreated = null;
            // TestPOSTWebRequest(user);
            // TestGETWebRequest("Testing get...");
            this._socket = IO.Socket(this.baseURL);
            _socket.On("user_signed_in", (signInFeedback) =>
            {
                SignInFeedback feedback = JsonConvert.DeserializeObject<SignInFeedback>(signInFeedback.ToString());
                if (feedback.feedback.status)
                {
                    Console.WriteLine("wudup");
                    Console.WriteLine(JsonConvert.SerializeObject(feedback.rooms_joined[0].avatars).ToString());
                    _events.PublishOnUIThread(new joinedRoomReceived(feedback.rooms_joined));
                    _userdata.avatarName = feedback.rooms_joined.Single(i => i.roomName == "General").avatars[_userdata.userName];
                    Console.WriteLine("fruit:");
                    Console.WriteLine(_userdata.avatarName);
                    _events.PublishOnUIThread(new LogInEvent());

                }
                else
                {
                    _events.PublishOnUIThread(new appWarningEvent(feedback.feedback.log_message));
                }
                //voir doc
            });

            _socket.On("new_message", (message) =>
             {
                 Message newMessage = JsonConvert.DeserializeObject<Message>(message.ToString());
                 Console.WriteLine("message received");
                 _userdata.addMessage(newMessage);
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
                dynamic magic = JsonConvert.DeserializeObject(feedback.ToString());
                if (magic.room_joined != null & magic.isPrivate != null)
                {
                    JoinRoomFeedBack fb = JsonConvert.DeserializeObject<JoinRoomFeedBack>(feedback.ToString());
                    if (fb.feedback.status & fb.room_joined != null)
                    {
                        Console.WriteLine("user_joined_room :");
                        Console.WriteLine(fb.room_joined.id);

                        if (fb.isPrivate)
                        {
                            _userdata.addJoinedRoom(fb.room_joined);
                        }
                        else
                        {
                            _userdata.addJoinedRoom(fb.room_joined);
                        }
                    }
                    else
                    {
                        Console.WriteLine(fb.feedback.log_message);
                    }
                }
                else
                {
                    Console.WriteLine(magic);
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
                Console.WriteLine("Room created onSocket");
                if (json.status & _roomToBeCreated != null)
                {
                    Console.WriteLine(json.log_message);
                    getPublicChannels();
                    _userdata.addJoinedRoom(new Room(_roomToBeCreated, new Message[0], new Dictionary<string, string>()));
                    _roomToBeCreated = null;
                    /* TODO: Ajouter l'avatar du user dans le dictionnaire */
                }
            });
        }

        public void createRoom(string roomID)
        {
            _roomToBeCreated = roomID;
            CreateRoom cR = new CreateRoom(roomID, false);
            _socket.Emit("create_chat_room", JsonConvert.SerializeObject(cR));
        }

        public void joinRoom(string roomID)
        {
            Console.WriteLine("tentive de join de : " + roomID);
            _socket.Emit("join_chat_room", roomID);
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
            if (_userdata.currentMessage != null)
            {
                ClientMessage message = new ClientMessage(_userdata.currentMessage, _userdata.currentRoomId);

                if (message.content.Trim() != "")
                {
                    _socket.Emit("send_message", JsonConvert.SerializeObject(message));
                }
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
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(this.baseURL + url);
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

        public Object TestGETWebRequest(string url)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(this.baseURL + url);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "GET";

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                // Console.WriteLine(result);
                return result;
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

        public void onLobby(BindableCollection<Match> matches)
        {
            this.socket.On("update_matches", (new_matches) =>
            {
                matches.Clear();
                matches.AddRange(JsonConvert.DeserializeObject<BindableCollection<Match>>(new_matches.ToString()));
            });

            this.socket.On("match_joined", (joinRoomFeedback) =>
            {
                JoinRoomFeedBack jRF = JsonConvert.DeserializeObject<JoinRoomFeedBack>(joinRoomFeedback.ToString());
                if (jRF.feedback.status)
                {
                    if (jRF.isPrivate)
                    {
                        this._userdata.matchId = jRF.room_joined.id;
                        this._userdata.currentRoomId = jRF.room_joined.id;
                        this._userdata.currentGameRoom = jRF.room_joined;
                        this._events.PublishOnUIThread(new waitingRoomEvent());
                    }
                }
                else
                {
                    _events.PublishOnUIThread(new appWarningEvent(jRF.feedback.log_message));
                }
            });
        }

        public void offLobby()
        {
            this.socket.Off("update_matches");
            this.socket.Off("match_joined");
        }

        public void onCreateMatch()
        {
            this.socket.On("match_created", (new_match) =>
            {
                dynamic json = JsonConvert.DeserializeObject(new_match.ToString());
                if ((Boolean)json.feedback.status)
                {
                    this._userdata.matchId = json.matchId;
                    Room room = new Room(this._userdata.matchId, new Message[0], new Dictionary<string, string>());
                    this._userdata.currentGameRoom = room;
                    this._events.PublishOnUIThread(new waitingRoomEvent());
                }
                else
                {
                    _events.PublishOnUIThread(new appWarningEvent(json.feedback.log_message));
                }
            });
        }

        public void offCreateMatch()
        {
            this.socket.Off("match_created");
        }


        public void onWaitingRoom(BindableCollection<Player> players)
        {
            this.socket.On("update_players", (new_players) =>
            {
                Console.WriteLine("update_players " + _userdata.userName);
                players.Clear();
                players.AddRange(JsonConvert.DeserializeObject<BindableCollection<Player>>(new_players.ToString()));
            });

            this.socket.On("match_left", (feedback) =>
            {
                Console.WriteLine("match_left " + _userdata.userName);
                dynamic json = JsonConvert.DeserializeObject(feedback.ToString());
                Console.WriteLine(json);
                if ((Boolean)json.status)
                {
                    this._events.PublishOnUIThread(new goBackMainEvent());
                    this.offWaitingRoom();
                }
            });

            this.socket.On("vp_added", (feedback) =>
            {
                Console.WriteLine(JsonConvert.DeserializeObject(feedback.ToString()));
            });

            this.socket.On("vp_removed", (feedback) =>
            {
                Console.WriteLine(JsonConvert.DeserializeObject(feedback.ToString()));
            });

            this.socket.On("match_started", (startMatchFeedback) =>
            {
                Console.WriteLine("onWaitingRoom match_started");
                dynamic json = JsonConvert.DeserializeObject(startMatchFeedback.ToString());
                Console.WriteLine(startMatchFeedback);
                if ((Boolean)json.feedback.status)
                {
                    this._userdata.nbRounds = (int)json.nbRounds;
                    _events.PublishOnUIThread(new gameEvent());
                }
                else
                {
                    _events.PublishOnUIThread(new appWarningEvent((string)json.feedback.log_message));
                }
            });

            this.socket.On("unexpected_leave", () =>
            {
                this.offWaitingRoom();
                this._events.PublishOnUIThread(new joinGameEvent());
                _events.PublishOnUIThread(new appWarningEvent("Unexpected match leave"));
            });
        }

        public void offWaitingRoom()
        {
            this.socket.Off("update_players");
            this.socket.Off("match_left");
            this.socket.Off("vp_added");
            this.socket.Off("vp_removed");
            this.socket.Off("match_started");
            this.socket.Off("unexpected_leave");
        }

        public void onMatch(StartTurn startTurn, EndTurn endTurn)
        {
            this.socket.On("turn_ended", (new_endTurn) =>
            {
                Console.WriteLine("onMatch turn_ended");
                EndTurn json = JsonConvert.DeserializeObject<EndTurn>(new_endTurn.ToString());
                endTurn.set(json);
                _events.PublishOnUIThread(new endTurnRoutineVMEvent());
            });

            this.socket.On("turn_started", (new_startTurn) =>
            {
                Console.WriteLine("onMatch turn_started");
                StartTurn json = JsonConvert.DeserializeObject<StartTurn>(new_startTurn.ToString());
                startTurn.set(json, endTurn.drawer == this._userdata.userName);
                _events.PublishOnUIThread(new startTurnRoutineEvent(startTurn.timeLimit));
            });

            this.socket.On("update_sprint", (new_update_sprint) =>
            {
                UpdateSprint json = JsonConvert.DeserializeObject<UpdateSprint>(new_update_sprint.ToString());
                startTurn.word = string.Concat(json.word.Select(letter => letter + " "));
                startTurn.timeLimit = json.time;
                endTurn.players = new BindableCollection<Player>(json.players.OrderByDescending(i => i.ScoreTotal));
                // json.guess TODO
                _events.PublishOnUIThread(new startTurnRoutineEvent(startTurn.timeLimit));
            });

            this.socket.On("guess_res", (Feedback) =>
            {
                dynamic json = JsonConvert.DeserializeObject(Feedback.ToString());
                Console.WriteLine("onMatch guess_res " + (Boolean)json.status);
                _events.PublishOnUIThread(new guessResponseEvent((Boolean)json.status));

            });

            this.socket.On("match_ended", (Feedback) =>
            {
                List<Player> players = JsonConvert.DeserializeObject<List<Player>>(Feedback.ToString());
                _events.PublishOnUIThread(new endMatchEvent(new List<Player>(players.Where(player => !player.isVirtual))));
            });

            this.socket.On("unexpected_leave", () =>
            {
                this.offMatch();
                this._events.PublishOnUIThread(new joinGameEvent());
                _events.PublishOnUIThread(new appWarningEvent("Unexpected match leave"));
            });
        }

        public void offMatch()
        {
            this.socket.Off("turn_ended");
            this.socket.Off("turn_started");
            this.socket.Off("update_sprint");
            this.socket.Off("guess_res");
            this.socket.Off("match_ended");
            this.socket.Off("unexpected_leave");
        }

    }

}


