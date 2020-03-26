﻿using Caliburn.Micro;
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
    public class chatBoxViewModel: Screen, IHandle<refreshMessagesEvent>, IHandle<addMessageEvent>, IHandle<createTheRoomEvent>
    {
        private IEventAggregator _events;
        private IUserData _userData;
        private BindableCollection<Models.Message> _messages;
        private string _currentMessage;
        private ISocketHandler _socketHandler;
        private BindableCollection<Room> _availableRooms;
        private BindableCollection<Room> _joinedRooms;
        public IchangeChannelCommand _changeChannelCommand { get; set; }
        private string _selectedChannelId;
        private string _createdRoomName;
        private string _currentRoomId;

        public string currentMessage
        {
            get { return _currentMessage; }
            set { _currentMessage = value;
                NotifyOfPropertyChange(() => currentMessage);
                _userData.currentMessage = value;
            }
        }

        public void keyDown(ActionExecutionContext context)
        {
            var keyArgs = context.EventArgs as KeyEventArgs;

            if (keyArgs != null && keyArgs.Key == Key.Enter)
            {
                //sendMessage();
            }
        }
        public BindableCollection<Models.Message> messages
        {
            get { return _messages; }
            set { _messages = value;
                  NotifyOfPropertyChange(() => messages); }
        }

        public void sendMessage( string content = null)
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

        public chatBoxViewModel(IUserData userdata, IEventAggregator events, ISocketHandler socketHandler)
        {
            _events = events;
            _events.Subscribe(this);
            _socketHandler = socketHandler;
            _userData = userdata;
            messages = userdata.messages;
            DisplayName = "chatBox";

            getPublicChannels();

            availableRooms = userdata.publicRooms;
            joinedRooms = userdata.joinedRooms;
            currentRoomId = userdata.currentRoomId;
            _changeChannelCommand = new changeChannelCommand(userdata);
        }

        public string welcomeMessage
        {
            get
            {
                return $"Welcome to the chatroom {_userData.userName} !";
            }
        }
        public void disconnect()
        {
            _socketHandler.disconnect();
            _events.PublishOnUIThread(new DisconnectEvent());
        }

        public void Handle(refreshMessagesEvent message)
        {
            this._messages = message._messages;
            this._currentRoomId = message._currentRoomId;
            NotifyOfPropertyChange(() => currentRoomId);
            NotifyOfPropertyChange(() => messages);
        }

        public void Handle(addMessageEvent message)
        {
            Console.WriteLine("hello");
            this._messages.Add(message.message);
            NotifyOfPropertyChange(() => messages);
        }

        public void windowMode()
        {
            _events.PublishOnUIThread(new windowChatEvent());
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

        public void Handle(createTheRoomEvent message)
        {
            Room newRoom = new Room(createdRoomName, new Models.Message[0], new Dictionary<string, string>());
            _userData.addRoom(newRoom);
            createdRoomName = "";
        }
    }

}
