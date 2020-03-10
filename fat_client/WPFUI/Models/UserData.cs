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
        private BindableCollection<Message> _messages;
        private BindableCollection<Room> _publicRooms;
        private BindableCollection<Room> _joinedRooms;
        private IEventAggregator _events;

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
            _currentRoomId = "null";
        }

        public void changeChannel(string roomID)
        {
            _currentRoomId = roomID;
            messages = new BindableCollection<Message>(_joinedRooms.Single(i => i.roomName == roomID).messages);
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
    }
}
