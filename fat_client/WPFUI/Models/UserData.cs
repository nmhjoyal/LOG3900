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
        private BindableCollection<SelectableRoom> _selectablePublicRooms;
        private BindableCollection<SelectableRoom> _selectableJoinedRooms;
        private string _matchId;
        private int _nbRounds;
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

        public BindableCollection<SelectableRoom> selectablePublicRooms
        {
            get { return _selectablePublicRooms; }
            set { _selectablePublicRooms = value; }
        }

        public BindableCollection<SelectableRoom> selectableJoinedRooms
        {
            get { return _selectableJoinedRooms; }
            set { _selectableJoinedRooms = value; }
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

        public string matchId
        {
            get { return _matchId; }
            set { _matchId = value; }
        }

        public int nbRounds
        {
            get { return _nbRounds; }
            set { _nbRounds = value; }
        }
        public UserData(IEventAggregator events)
        {
            _events = events;
            _events.Subscribe(this);
            _messages = new BindableCollection<Message>();
            _selectableJoinedRooms = new BindableCollection<SelectableRoom>();
            _selectablePublicRooms = new BindableCollection<SelectableRoom>();
            _currentRoomId = null;
            _avatarName = null;
        }

        public void changeChannel(string roomID)
        {
            Console.WriteLine("changing channel in userdata: " + roomID);
            this.currentRoomId = roomID;
            try
            {
                this.messages = new BindableCollection<Message>((this.selectableJoinedRooms.Single(i => i.id == roomID)).room.messages);
            } catch
            {
                this.messages = new BindableCollection<Message>(this.selectableJoinedRooms.Where(x => x.id == roomID).ToList()[0].room.messages);
            }

            _events.PublishOnUIThread(new refreshMessagesEvent(this.messages, roomID));
        }

        public void Handle(roomsRetrievedEvent message)
        {
            Console.WriteLine("roomsRetrievedEvent Handled");
            this.selectablePublicRooms.Clear();
            Console.WriteLine(message._publicRooms.Length);
            foreach (string channelID in message._publicRooms)
            {
                this.selectablePublicRooms.Add(new SelectableRoom(new Room(channelID, null, null)));
            }
        }

        public void Handle(joinedRoomReceived message)
        {
            this.selectableJoinedRooms = new BindableCollection<SelectableRoom>();
            foreach (Room r in message._joinedRooms)
            {
                this.selectableJoinedRooms.Add(new SelectableRoom(r));
            }

            this.currentRoomId = this.selectableJoinedRooms[0].id;
            this.messages = new BindableCollection<Message>(this.selectableJoinedRooms[0].room.messages);
        }

        public void addJoinedRoom(Room room)
        {
            selectableJoinedRooms.Add(new SelectableRoom(room));
        }

        public void addPublicRoom(Room room)
        {
            selectablePublicRooms.Add(new SelectableRoom(room));
        }

        public void addMessage(Message message)
        {
            Message[] messagesToUpdate = this.selectableJoinedRooms.Single(i => i.id == message.roomId).room.messages;

            if (messagesToUpdate != null)
            {
                if (message.roomId == currentRoomId)
                {
                    _events.PublishOnUIThread(new addMessageEvent(message));
                }
                List<Message> list = new List<Message>(messagesToUpdate);
                list.Add(message);
                this.selectableJoinedRooms.Single(i => i.id == message.roomId).room.messages = list.ToArray();
            }
        }
    }
}
