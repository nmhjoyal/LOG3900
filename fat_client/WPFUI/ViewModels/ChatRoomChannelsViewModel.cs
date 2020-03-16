using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public ChatRoomChannelsViewModel(IEventAggregator events, ISocketHandler socketHandler)
        {
            _events = events;
            _socketHandler = socketHandler;
            _availableRooms = new BindableCollection<Room>();
            _joinedRooms = new BindableCollection<Room>();
            addFakeRooms();
        }

        public void addFakeRooms()
        {
            availableRooms.Add(new Room("room1", null, null));
            availableRooms.Add(new Room("room2", null, null));
            availableRooms.Add(new Room("room5", null, null));
            availableRooms.Add(new Room("room6", null, null));

            joinedRooms.Add(new Room("room3", null, null));
            joinedRooms.Add(new Room("room4", null, null));
        }
    }
}
