using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.EventModels;

namespace WPFUI.Models
{
    public class UserData : IHandle<roomsRetrievedEvent>, IHandle<joinedRoomReceived>, IUserData
    {
        private string _userName;
        private string _password;
        private string _ipAdress;
        private string _currentMessage;
        private string _currentRoomId;
        private string _avatarName;
        private BindableCollection<Message> _messages;
        private BindableCollection<Room> _publicRooms;
        private BindableCollection<Room> _joinedRooms;
        private IEventAggregator _events;

        public string avatarName
        {
            get { return _avatarName; }
            set { _avatarName = value; }
        }
        public string currentRoomId
        {
            get { return _currentRoomId; }
            set { _currentRoomId = value; }
        }
        public BindableCollection<Room> publicRooms
        {
            get { return _publicRooms; }
            set { _publicRooms = value; }
        }
        public BindableCollection<Room> joinedRooms
        {
            get { return _joinedRooms; }
            set { _joinedRooms = value; }
        }

        public BindableCollection<Message> messages
        {

            get { return _messages; }
            set { _messages = value; }
        }

        public string currentMessage
        {
            get { return _currentMessage; }
            set { _currentMessage = value; }
        }

        public string userName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string ipAdress
        {
            get { return _ipAdress; }
            set { _ipAdress = value; }
        }

        public string password
        {
            get { return _password; }
            set { _password = value; }
        }

        public UserData(IEventAggregator events)
        {
            _events = events;
            _events.Subscribe(this);
            _messages = new BindableCollection<Message>();
            _joinedRooms = new BindableCollection<Room>();
            _publicRooms = new BindableCollection<Room>();
            _currentRoomId = null;
            _avatarName = null;
        }

        public void changeChannel(string roomID)
        {
            Console.WriteLine("changing channel in userdata: " + roomID);
            this.currentRoomId = roomID;
            this.messages = new BindableCollection<Message>(this.joinedRooms.Single(i => i.roomName == roomID).messages);
            _events.PublishOnUIThread(new refreshMessagesEvent(this.messages));
        }

        public void Handle(roomsRetrievedEvent message)
        {
            _publicRooms.Clear();
            foreach (string channelID in message._publicRooms)
            {
                publicRooms.Add(new Room(channelID, null, null));
            }
        }

        public void Handle(joinedRoomReceived message)
        {
            BindableCollection<Room> joinedRooms = new BindableCollection<Room>(message._joinedRooms);
            this.joinedRooms = joinedRooms;
            this.currentRoomId = this.joinedRooms[0].id;
            Console.WriteLine("currentRoomId:");
            Console.WriteLine(currentRoomId);
            this.messages = new BindableCollection<Message>(this.joinedRooms[0].messages);
        }

        public void addRoom(Room room)
        {
            joinedRooms.Add(room);
        }

        public void addMessage(Message message)
        {
            Message[] messagesToUpdate = this.joinedRooms.Single(i => i.roomName == message.roomId).messages;

            if (messagesToUpdate != null)
            {
                if (message.roomId == currentRoomId)
                {
                    _events.PublishOnUIThread(new addMessageEvent(message));
                }
                List<Message> list = new List<Message>(messagesToUpdate);
                list.Add(message);
                this.joinedRooms.Single(i => i.roomName == message.roomId).messages = list.ToArray();
            }
        }
    }
}
