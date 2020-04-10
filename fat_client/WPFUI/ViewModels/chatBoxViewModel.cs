using Caliburn.Micro;
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
    public class chatBoxViewModel: Screen, IHandle<refreshMessagesEvent>, IHandle<addMessageEvent>, IHandle<createTheRoomEvent>,
                                   IHandle<refreshRoomsEvent>, IHandle<resetToGeneralEvent>
    {
        private IEventAggregator _events;
        private IUserData _userData;
        private BindableCollection<Models.Message> _messages;
        private string _currentMessage;
        private ISocketHandler _socketHandler;
        private BindableCollection<SelectableRoom> _availableRooms;
        private BindableCollection<SelectableRoom> _joinedRooms;
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
            get { return _availableRooms; }
            set
            {
                _availableRooms = value;
                NotifyOfPropertyChange(() => availableRooms);
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
            availableRooms = userdata.selectablePublicRooms;
            joinedRooms = userdata.selectableJoinedRooms;
            currentRoomId = userdata.currentRoomId;
            _changeChannelCommand = new changeChannelCommand(userdata);
            _selectAvailableRoomCommand = new selectAvailableRoomCommand(events);
            _selectJoinedRoomCommand = new selectJoinedRoomCommand(events);
            _selectedJoinedRoom = null;
            _selectedAvailableRoom = null;
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
            Console.WriteLine("message sending attempted");
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

        public void createRoom()
        {
            if (createdRoomName != null & createdRoomName != "")
            {
                _socketHandler.createRoom(createdRoomName);
            }
        }

        public void goBack()
        {
            _events.PublishOnUIThread(new goBackMainEvent());
        }

        public void joinRoom()
        {
            Console.WriteLine("1");
            _socketHandler.joinRoom(_selectedAvailableRoom);
        }

        /* Handlers -----------------------------------------------------------------------------------------------*/
        public void Handle(createTheRoomEvent message)
        {
            Room newRoom = new Room(createdRoomName, new Models.Message[0], new Dictionary<string, string>());
            _userData.addJoinedRoom(newRoom);
            createdRoomName = "";
        }

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
            Console.WriteLine("hello");
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

    }

}
