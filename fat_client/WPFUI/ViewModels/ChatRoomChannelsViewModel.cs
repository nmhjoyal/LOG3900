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
    class ChatRoomChannelsViewModel: Screen
    {
        private IEventAggregator _events;
        private ISocketHandler _socketHandler;
        private BindableCollection<Room> _availableRooms;
        private BindableCollection<Room> _joinedRooms;
        public IchangeChannelCommand _changeChannelCommand { get; set; }
        private string _selectedChannelId;
        private string _currentRoomId;

        public string currentRoomMessage
        {
            get { return "Current room is " + _currentRoomId; }
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
                _availableRooms = value;
                NotifyOfPropertyChange(() => availableRooms);
            }
        }

        public BindableCollection<Room> joinedRooms
        {
            get { return _joinedRooms; }
            set
            {
                _joinedRooms = value;
                NotifyOfPropertyChange(() => joinedRooms);
            }
        }
        public ChatRoomChannelsViewModel(IEventAggregator events, ISocketHandler socketHandler, IUserData userdata)
        {
            _events = events;
            _socketHandler = socketHandler;
            _availableRooms = new BindableCollection<Room>();
            _joinedRooms = new BindableCollection<Room>();
            _changeChannelCommand = new changeChannelCommand(userdata);
            _currentRoomId = userdata.currentRoomId;
        }

        private void getPublicChannels()
        {
            //_socketHandler.
        }
    }
}
