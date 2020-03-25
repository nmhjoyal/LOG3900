using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.Commands;
using WPFUI.EventModels;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    class ChatRoomChannelsViewModel: Screen, IHandle<createTheRoomEvent>
    {
        private IUserData _userdata;
        private IEventAggregator _events;
        private ISocketHandler _socketHandler;
        private BindableCollection<Room> _availableRooms;
        private BindableCollection<Room> _joinedRooms;
        public IchangeChannelCommand _changeChannelCommand { get; set; }
        private string _selectedChannelId;
        private string _createdRoomName;
        private string _currentRoomId;

        public string currentRoomId
        {
            get { return _currentRoomId; }
            set { _currentRoomId = value; }
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


        public string selectedChannelId
        {
            get { return _selectedChannelId; }
            set { _selectedChannelId = value; }
        }


        public BindableCollection<Room> availableRooms
        {
            get { return _availableRooms; }
            set
            {
                Console.WriteLine("set available rooms");
                _availableRooms = value;
                NotifyOfPropertyChange(() => availableRooms);
            }
        }

        public BindableCollection<Room> joinedRooms
        {
            get { return _joinedRooms; }
            set
            {
                Console.WriteLine("set available rooms");
                _joinedRooms = value;
                NotifyOfPropertyChange(() => joinedRooms);
            }
        }
        public ChatRoomChannelsViewModel(IEventAggregator events, ISocketHandler socketHandler, IUserData userdata)
        {
            _userdata = userdata;
            _socketHandler = socketHandler;
            getPublicChannels();
            _events = events;
            _events.Subscribe(this);
            availableRooms = userdata.publicRooms;
            joinedRooms = userdata.joinedRooms;
            currentRoomId = userdata.currentRoomId;
            _changeChannelCommand = new changeChannelCommand(userdata);
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

        public void GoBack()
        {
            _events.PublishOnUIThread(new goBackMainEvent());
        }

        public void Handle(createTheRoomEvent message)
        {
            Room newRoom = new Room(createdRoomName, new Models.Message[0], new Dictionary<string, string>());
            _userdata.addRoom(newRoom);
            createdRoomName = "";
        }
    }
}
