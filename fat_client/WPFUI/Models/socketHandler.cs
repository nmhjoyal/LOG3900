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
        private string roomToBeDeleted;
        private Boolean _isRoomToBeCreatedPrivate;
        private string baseURL;
        private string roomToBeLeft;
        private string _avatarChangePending;

        public string avatarChangePending
        {
            get { return _avatarChangePending; }
            set { _avatarChangePending = value; }
        }


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
                    _events.PublishOnUIThread(new joinedRoomReceived(feedback.rooms_joined));
                    _userdata.avatarName = feedback.rooms_joined.Single(i => i.roomName == "General").avatars[_userdata.userName];
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
                 _userdata.addMessage(newMessage);
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

            _socket.On("user_joined_room", (feedback) =>
            {
                Console.WriteLine("qqn a join une room");
                dynamic magic = JsonConvert.DeserializeObject(feedback.ToString());
                if (magic.room_joined != null & magic.isPrivate != null)
                {
                    JoinRoomFeedBack fb = JsonConvert.DeserializeObject<JoinRoomFeedBack>(feedback.ToString());
                    if (fb.feedback.status & fb.room_joined != null)
                    {
                        if (fb.isPrivate)
                        {
                            _userdata.addJoinedRoom(fb.room_joined, true);
                        }
                        else
                        {
                            _userdata.addJoinedRoom(fb.room_joined, false);
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
                dynamic json = JsonConvert.DeserializeObject(feedback.ToString());
                string[] publicRooms = json.ToObject<string[]>();
                _events.PublishOnUIThread(new roomsRetrievedEvent(publicRooms));
            });

            _socket.On("room_created", (feedback) =>
            {
                Console.WriteLine("qqn a create une room");
                Console.WriteLine(feedback);
                Feedback json = JsonConvert.DeserializeObject<Feedback>(feedback.ToString());
                if (json.status & _roomToBeCreated != null)
                {
                    if (_isRoomToBeCreatedPrivate)
                    {
                        getPublicChannels();
                        Message[] messages = new Message[1];
                        // TODO: Mettre le bon timestamp
                        messages[0] = new Message("Admin", _userdata.userName + " joigned the room.", 0, _roomToBeCreated);
                        /* TODO: Ajouter l'avatar du user dans le dictionnaire */
                        _userdata.addJoinedRoom(new Room(_roomToBeCreated, messages, new Dictionary<string, string>()), true);
                    }
                    else
                    {
                        getPublicChannels();
                        Message[] messages = new Message[1];
                        // TODO: Mettre le bon timestamp
                        messages[0] = new Message("Admin", _userdata.userName + " joigned the room.", 0, _roomToBeCreated);
                        /* TODO: Ajouter l'avatar du user dans le dictionnaire */
                        _userdata.addJoinedRoom(new Room(_roomToBeCreated, messages, new Dictionary<string, string>()), false);
                    }
                    _roomToBeCreated = null;
                    _isRoomToBeCreatedPrivate = false;
                }
                else
                {
                    _events.PublishOnUIThread(new appWarningEvent(json.log_message));
                }
            });

            _socket.On("user_sent_invite", (feedback) =>
            {
                dynamic json = JsonConvert.DeserializeObject(feedback.ToString());
                if (!(Boolean)json.status)
                {
                    _events.PublishOnUIThread(new appWarningEvent((string)json.log_message));
                }

            });

            _socket.On("receive_invite", (feedback) =>
            {
                dynamic json = JsonConvert.DeserializeObject(feedback.ToString());
                Invitation invite = new Invitation((string)json.id, (string)json.username);
                if (_userdata.invites.Where(x => x.id == invite.id).Count() == 0)
                {
                    _userdata.invites.Add(invite);
                    _events.PublishOnUIThread(new refreshInvitesEvent());
                }

            });

            _socket.On("avatar_updated", (feedback) =>
            {
                Console.WriteLine(feedback);
                dynamic json = JsonConvert.DeserializeObject(feedback.ToString());
                Console.WriteLine("avatar update dans la room :" + (string)json.roomId);
                PublicProfile pp = new PublicProfile((string)json.updatedProfile.username, (string)json.updatedProfile.avatar);
                _userdata.addModifiedProfile(pp);

            });

            _socket.On("user_left_room", (feedback) =>
            {
                dynamic json = JsonConvert.DeserializeObject(feedback.ToString());
                if ((Boolean)json.status)
                {
                    IEnumerable<SelectableRoom> enumRoom = _userdata.selectableJoinedRooms.Where(x => x.id == roomToBeLeft);
                    BindableCollection<SelectableRoom> roomsTobeDeleted = new BindableCollection<SelectableRoom>(enumRoom);

                    foreach (SelectableRoom s in roomsTobeDeleted)
                    {
                        _userdata.selectableJoinedRooms.Remove(s);
                    }
                    _userdata.selectableJoinedRooms.Refresh();

                    if (roomToBeLeft == userdata.currentRoomId)
                    {
                        _userdata.changeChannel("General");
                    }
                }
                else
                {
                    roomToBeLeft = null;
                    _events.PublishOnUIThread(new appWarningEvent((string)json.log_message));
                }


            });

            _socket.On("room_deleted", (feedback) =>
            {
                dynamic json = JsonConvert.DeserializeObject(feedback.ToString());
                if ((Boolean)json.status)
                {
                    IEnumerable<SelectableRoom> enumRoomJoigned = _userdata.selectableJoinedRooms.Where(x => x.id == roomToBeDeleted);
                    BindableCollection<SelectableRoom> joignedRoomsTobeDeleted = new BindableCollection<SelectableRoom>(enumRoomJoigned);

                    IEnumerable<SelectableRoom> enumRoomPublic = _userdata.selectablePublicRooms.Where(x => x.id == roomToBeDeleted);
                    BindableCollection<SelectableRoom> publicRoomsTobeDeleted = new BindableCollection<SelectableRoom>(enumRoomPublic);

                    foreach (SelectableRoom s in joignedRoomsTobeDeleted)
                    {
                        _userdata.selectableJoinedRooms.Remove(s);
                    }


                    foreach (SelectableRoom s in publicRoomsTobeDeleted)
                    {
                        _userdata.selectablePublicRooms.Remove(s);
                    }
                    _userdata.selectablePublicRooms.Refresh();
                    _userdata.selectableJoinedRooms.Refresh();

                    if (roomToBeDeleted == userdata.currentRoomId)
                    {
                        _userdata.changeChannel("General");
                    }
                }
                else
                {
                    roomToBeDeleted = null;
                    _events.PublishOnUIThread(new appWarningEvent((string)json.log_message));
                }


            });

            _socket.On("profile_updated", (feedback) =>
            {
                Console.WriteLine(feedback);
                dynamic json = JsonConvert.DeserializeObject(feedback.ToString());
                if ((Boolean)json.status)
                {
                    _userdata.avatarName = avatarChangePending;
                    _events.PublishOnUIThread(new avatarUpdated());
                } else
                {
                    _events.PublishOnUIThread(new appWarningEvent((string)json.log_message));
                }
            });
        }

        public void createRoom(string roomID, Boolean isPrivate)
        {
            _roomToBeCreated = roomID;
            _isRoomToBeCreatedPrivate = isPrivate;
            CreateRoom cR = new CreateRoom(roomID, isPrivate);
            _socket.Emit("create_chat_room", JsonConvert.SerializeObject(cR));
        }

        public void leaveRoom(string roomID)
        {
            roomToBeLeft = roomID;
            _socket.Emit("leave_chat_room", roomID);
        }

        public void deleteRoom(string roomID)
        {
            roomToBeDeleted = roomID;
            _socket.Emit("delete_chat_room", roomID);
        }

        public void joinRoom(string roomID)
        {
            _socket.Emit("join_chat_room", roomID);
        }

        public void connectionAttempt()
        {
            _user = new User(_userdata.userName, _userdata.password);
            this._userJSON = JsonConvert.SerializeObject(_user);
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
                    // Console.WriteLine(stylusPoint.X + " & " + stylusPoint.Y);
                    try
                    {
                        this.Dispatcher.Invoke(() =>
                        Traits[currentStrokeIndex].StylusPoints.Add(stylusPoint)
                    );
                    }
                    catch (Exception e) { _events.PublishOnUIThread(new appWarningEvent("New_point error")); }
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

            this._socket.On("new_clear", () =>
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

        public void onPreview()
        {
            this._socket.On("preview_done", () =>
            {
                Console.WriteLine("preview_done");
                this._events.PublishOnUIThread(new previewDoneEvent());
            });
        }

        public void offPreview()
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
                    _events.PublishOnUIThread(new appWarningEvent((string)json.feedback.log_message));
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
                players.Clear();
                players.AddRange(JsonConvert.DeserializeObject<BindableCollection<Player>>(new_players.ToString()));
            });

            this.socket.On("match_left", (feedback) =>
            {
                dynamic json = JsonConvert.DeserializeObject(feedback.ToString());
                if ((Boolean)json.status)
                {
                    this._events.PublishOnUIThread(new goBackMainEvent());
                    this.offWaitingRoom();
                }
            });

            this.socket.On("vp_added", (feedback) =>
            {
                //Console.WriteLine(JsonConvert.DeserializeObject(feedback.ToString()));
            });

            this.socket.On("vp_removed", (feedback) =>
            {
                //Console.WriteLine(JsonConvert.DeserializeObject(feedback.ToString()));
            });

            this.socket.On("match_started", (startMatchFeedback) =>
            {
                dynamic json = JsonConvert.DeserializeObject(startMatchFeedback.ToString());
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

        public void onMatch(StartTurn startTurn, EndTurn endTurn, GuessesLeft guessesLeft)
        {
            this.socket.On("turn_ended", (new_endTurn) =>
            {
                EndTurn json = JsonConvert.DeserializeObject<EndTurn>(new_endTurn.ToString());
                endTurn.set(json);
                _events.PublishOnUIThread(new endTurnRoutineVMEvent());
            });

            this.socket.On("turn_started", (new_startTurn) =>
            {
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
                guessesLeft.guess = json.guess;
                _events.PublishOnUIThread(new startTurnRoutineEvent(startTurn.timeLimit));
            });

            this.socket.On("guess_res", (Feedback) =>
            {
                dynamic json = JsonConvert.DeserializeObject(Feedback.ToString());
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
                socket.Emit("leave_chat_room", _userdata.matchId);
                socket.Emit("leave_match");
                this._events.PublishOnUIThread(new joinGameEvent());
                _events.PublishOnUIThread(new appWarningEvent("Unexpected match leave"));
            });

            this.socket.On("update_players", (new_players) =>
            {
                List<Player> players = JsonConvert.DeserializeObject<List<Player>>(new_players.ToString());
                endTurn.players.Clear();
                endTurn.players.AddRange(players.OrderByDescending(i => i.ScoreTotal));
            });

            this.socket.On("hint_enable", () =>
            {
                this._events.PublishOnUIThread(new hintEvent(true));
            });

            this.socket.On("hint_disable", () =>
            {
                this._events.PublishOnUIThread(new hintEvent(false));
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
            this.socket.Off("update_players");
            this.socket.Off("hint_enable");
            this.socket.Off("hint_disable");
        }

    }

}


