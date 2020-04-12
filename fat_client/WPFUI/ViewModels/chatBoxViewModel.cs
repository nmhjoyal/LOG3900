using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFUI.Commands;
using WPFUI.EventModels;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    public class chatBoxViewModel: Screen, IHandle<refreshMessagesEvent>, IHandle<addMessageEvent>,
                                   IHandle<refreshRoomsEvent>, IHandle<resetToGeneralEvent>
    {
        private IEventAggregator _events;
        private IUserData _userData;
        private BindableCollection<Models.Message> _messages;
        private string _currentMessage;
        private ISocketHandler _socketHandler;
        private BindableCollection<SelectableRoom> _availableRooms;
        private BindableCollection<Invitation> _invites;
        private BindableCollection<SelectableRoom> _joinedRooms;
        private string _searchBarText;
        public IchangeChannelCommand _changeChannelCommand { get; set; }
        public IselectAvailableRoomCommand _selectAvailableRoomCommand { get; set; }
        public IselectJoinedRoomCommand _selectJoinedRoomCommand { get; set; }
        private string _createdRoomName;
        private string _currentRoomId;
        private string _selectedAvailableRoom;
        private string _selectedJoinedRoom;

        public IEventAggregator events
        {
            get { return _events; }
        }

        public IUserData userdata
        {
            get { return _userData; }
        }

        public string currentMessage
        {
            get { return _currentMessage; }
            set { _currentMessage = value;
                NotifyOfPropertyChange(() => currentMessage);
                _userData.currentMessage = value;
            }
        }

        public BindableCollection<Models.Message> messages
        {
            get { return _messages; }
            set { _messages = value;
                  NotifyOfPropertyChange(() => messages); }
        }

        public string currentRoomId
        {
            get { return _currentRoomId; }
            set { _currentRoomId = value;
                  NotifyOfPropertyChange(() => currentRoomId);
            }
        }


        public string createdRoomName
        {
            get { return _createdRoomName; }
            set { _createdRoomName = value; }
        }

        public string currentRoomMessage
        {
            get { return "Current room is " + currentRoomId; }
        }

        public BindableCollection<SelectableRoom> availableRooms
        {
            get { return filterAvailableRooms(); }
            set
            {
                _availableRooms = value;
                NotifyOfPropertyChange(() => availableRooms);
            }
        }

        public BindableCollection<SelectableRoom> filterAvailableRooms()
        {
            if (searchBarText != "" & searchBarText != null)
            {
                return new BindableCollection<SelectableRoom>(_availableRooms.
                    Where(room => room.id.ToLower().StartsWith(searchBarText.ToLower())));
            } else
            {
                return _availableRooms;
            }
            

        }

        

        public BindableCollection<SelectableRoom> joinedRooms
        {
            get { return _joinedRooms; }
            set
            {
                _joinedRooms = value;
                NotifyOfPropertyChange(() => joinedRooms);
            }
        }

        public chatBoxViewModel(IUserData userdata, IEventAggregator events, ISocketHandler socketHandler)
        {
            _events = events;
            _events.Subscribe(this);
            _socketHandler = socketHandler;
            getPublicChannels();
            _userData = userdata;
            messages = userdata.messages;
            _availableRooms = userdata.selectablePublicRooms;
            _joinedRooms = userdata.selectableJoinedRooms;
            _invites = userdata.invites;
            currentRoomId = userdata.currentRoomId;
            _changeChannelCommand = new changeChannelCommand(userdata);
            _selectAvailableRoomCommand = new selectAvailableRoomCommand(events);
            _selectJoinedRoomCommand = new selectJoinedRoomCommand(events);
            _selectedJoinedRoom = null;
            _selectedAvailableRoom = null;
            _searchBarText = null;
        }

        public BindableCollection<Invitation> invites
        {
            get { return _invites; }
            set { _invites = value;
                  NotifyOfPropertyChange(() => invites);
                  NotifyOfPropertyChange(() => nbInvites);
                  NotifyOfPropertyChange(() => nbInvitesVisibility);
            }
        }

        public int nbInvites
        {
            get { return _invites.Count(); }
        }

        public string nbInvitesVisibility
        {
            get {
                if (_invites.Count() > 0)
                {
                    return "Visible";
                } else
                {
                    return "Hidden";
                }
                 }
        }

        public string searchBarText
        {
            get { return _searchBarText; }
            set { _searchBarText = value;
                  NotifyOfPropertyChange(() => searchBarText);
                  NotifyOfPropertyChange(() => availableRooms);
            }
        }

        public string welcomeMessage
        {
            get
            {
                return $"Welcome to the chatroom {_userData.userName} !";
            }
        }
        /* Methods ---------------------------------------------------------------------------------------------------*/
        public void sendMessage(string content = null)
        {
            if (content != null)
            {
                _userData.currentMessage = content;
                _socketHandler.sendMessage();
                currentMessage = "";
                _userData.currentMessage = "";
            }
            else if (currentMessage != null & currentMessage != "")
            {
                //messages.Add(new MessageModel(currentMessage, _userData.userName, DateTime.Now));
                _socketHandler.sendMessage();
                currentMessage = "";
                _userData.currentMessage = "";
            }

        }
        public void disconnect()
        {
            _socketHandler.disconnect();
            _events.PublishOnUIThread(new DisconnectEvent());
        }

        public void getPublicChannels()
        {
            _socketHandler.getPublicChannels();
        }

        public void createRoom(string roomID, Boolean isPrivate)
        {
            _socketHandler.createRoom(roomID, isPrivate);
        }

        public void goBack()
        {
            _events.PublishOnUIThread(new goBackMainEvent());
        }

        public void joinRoom()
        {
            _socketHandler.joinRoom(_selectedAvailableRoom);
        }

        public void deleteRoom()
        {
            _socketHandler.deleteRoom(_selectedAvailableRoom);
        }

        public void leaveRoom(string roomID)
        {
            _socketHandler.leaveRoom(roomID);
        }

        public void joinInvitedRoom( string invitedRoomId)
        {
            _socketHandler.joinRoom(invitedRoomId);
        }

        /* Handlers -----------------------------------------------------------------------------------------------*/

        public void Handle(refreshRoomsEvent message)
        {
            if (!message.joined)
            {
                foreach (SelectableRoom sR in _availableRooms)
                {
                    sR.resetColor();
                    sR.menuVisibility = "Collapsed";
                }

                int selectedRoomIndex = _availableRooms.IndexOf(_availableRooms.Single(i => i.id == message.selectedRoomId));
                _availableRooms[selectedRoomIndex].changeColor("Black");
                _availableRooms[selectedRoomIndex].menuVisibility = "Visible";
                _availableRooms.Refresh();
                _selectedAvailableRoom = message.selectedRoomId;
                NotifyOfPropertyChange(null);
            } else
            {
                foreach (SelectableRoom sR in _joinedRooms)
                {
                    sR.resetColor();
                    sR.menuVisibility = "Collapsed";
                }

                int selectedRoomIndex = 0;
                try
                {
                    selectedRoomIndex = _joinedRooms.IndexOf(_joinedRooms.Single(i => i.id == message.selectedRoomId));
                } catch {
                    selectedRoomIndex = _joinedRooms.IndexOf(_joinedRooms.Where(x => x.id == message.selectedRoomId).ToList()[0]);
                  }

                _joinedRooms[selectedRoomIndex].changeColor("Black");
                _joinedRooms[selectedRoomIndex].menuVisibility = "Visible";
                _selectedJoinedRoom = message.selectedRoomId;
                _joinedRooms.Refresh();
                NotifyOfPropertyChange(null);
            }
        }

        public void Handle(refreshMessagesEvent message)
        {
            this.messages = message._messages;
            this.currentRoomId = message._currentRoomId;
        }

        public void Handle(addMessageEvent message)
        {
            this._messages.Add(message.message);
            NotifyOfPropertyChange(() => messages);
        }

        public void Handle(resetToGeneralEvent message)
        {
            _userData.matchId = null;
            _userData.currentGameRoom = null;
            Room general = _userData.selectableJoinedRooms[0].room;
            _userData.messages = new BindableCollection<Models.Message>(general.messages);
            _userData.currentRoomId = general.roomName;

            this.messages = _userData.messages;
            this.currentRoomId = _userData.currentRoomId;
            _events.PublishOnUIThread(new changeChatOptionsEvent(true));
        }

        public void sendInvite(string roomID, string player)
        {
            dynamic invitation = new System.Dynamic.ExpandoObject();
            invitation.id = roomID;
            invitation.username = player;
            Console.WriteLine(JsonConvert.SerializeObject(invitation));
            _socketHandler.socket.Emit("send_invite", JsonConvert.SerializeObject(invitation));

        }

    }

}
