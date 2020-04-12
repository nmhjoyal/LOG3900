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
        private BindableCollection<Invitation> _invites;
        private BindableCollection<Message> _messages;
        private BindableCollection<SelectableRoom> _selectablePublicRooms;
        private BindableCollection<SelectableRoom> _selectableJoinedRooms;
        private Room _currentGameRoom;
        private string _matchId;
        private int _nbRounds;
        private IEventAggregator _events;
        private MatchMode _matchMode;

        public BindableCollection<Invitation> invites
        {
            get { return _invites; }
            set { _invites = value; }
        }
        public Room currentGameRoom
        {
            get { return _currentGameRoom; }
            set { _currentGameRoom = value; }
        }

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

        public MatchMode matchMode
        {
            get { return _matchMode; }
            set { _matchMode = value; }
        }
        public UserData(IEventAggregator events)
        {
            _events = events;
            _events.Subscribe(this);
            _messages = new BindableCollection<Message>();
            _selectableJoinedRooms = new BindableCollection<SelectableRoom>();
            _selectablePublicRooms = new BindableCollection<SelectableRoom>();
            _currentRoomId = null;
            _currentGameRoom = null;
            _avatarName = null;
            _invites = new BindableCollection<Invitation>();
        }

        public void changeChannel(string roomID)
        {
            this.currentRoomId = roomID;
            try
            {
                this.messages = new BindableCollection<Message>((this.selectableJoinedRooms.Single(i => i.id == roomID)).room.messages);
            }
            catch
            {
                this.messages = new BindableCollection<Message>(this.selectableJoinedRooms.Where(x => x.id == roomID).ToList()[0].room.messages);
            }
            _currentRoomId = roomID;
            _events.PublishOnUIThread(new refreshMessagesEvent(this.messages, roomID));
        }

        public void Handle(roomsRetrievedEvent message)
        {
            this.selectablePublicRooms.Clear();
            foreach (string channelID in message._publicRooms)
            {
                if (channelID != null)
                {
                    this.selectablePublicRooms.Add(new SelectableRoom(new Room(channelID, null, null)));
                }
            }
        }

        public void Handle(joinedRoomReceived message)
        {
            this.selectableJoinedRooms = new BindableCollection<SelectableRoom>();
            foreach (Room r in message._joinedRooms)
            {
                if (r.id != null)
                {
                    this.selectableJoinedRooms.Add(new SelectableRoom(r));
                }
            }

            this.currentRoomId = this.selectableJoinedRooms[0].id;
            this.messages = new BindableCollection<Message>(this.selectableJoinedRooms[0].room.messages);
        }

        public void addJoinedRoom(Room room, Boolean isPrivate)
        {
            BindableCollection<SelectableRoom> roomAlreadyExists = new BindableCollection<SelectableRoom>(this.selectableJoinedRooms.Where(x => x.id == room.id));
            if (roomAlreadyExists.Count() == 0)
            {
                SelectableRoom sR = new SelectableRoom(room);
                sR.isPrivate = isPrivate;
                selectableJoinedRooms.Add(sR);
            }
        }

        public void addGameRoom(Room room)
        {
            currentGameRoom = room;
        }

        public void addPublicRoom(Room room)
        {
            BindableCollection<SelectableRoom> roomAlreadyExists = new BindableCollection<SelectableRoom>(this.selectablePublicRooms.Where(x => x.id == room.id));
            if (roomAlreadyExists.Count() == 0)
            {
                selectablePublicRooms.Add(new SelectableRoom(room));
            }
        }

        public void addMessage(Message message)
        {
            Message[] messagesToUpdate;
            SelectableRoom roomToBeUpdated;

            if (message.roomId == currentRoomId)
            {
                _messages.Add(message);
                _events.PublishOnUIThread(new scrollDownEvent());
            }

            if (message.roomId == matchId)
            {
                List<Message> list = new List<Message>(currentGameRoom.messages);
                list.Add(message);
                currentGameRoom.messages = list.ToArray();
            }
            else
            {
                try
                {
                    roomToBeUpdated = this.selectableJoinedRooms.Single(i => i.id == message.roomId);
                    messagesToUpdate = roomToBeUpdated.room.messages;
                }
                catch
                {
                    try
                    {
                        roomToBeUpdated = this.selectableJoinedRooms.Where(x => x.id == message.roomId).ToList()[0];
                        messagesToUpdate = roomToBeUpdated.room.messages;
                    }
                    catch
                    {
                        roomToBeUpdated = null;
                        messagesToUpdate = null;
                        // TODO: faire un popup approprie
                        Console.WriteLine("message sent to unjoigned room");
                        _events.PublishOnUIThread(new appWarningEvent("a message was sent to an unjoigned room"));
                    }

                }

                if (messagesToUpdate != null)
                {
                    List<Message> list = new List<Message>(messagesToUpdate);
                    list.Add(message);
                    this.selectableJoinedRooms[selectableJoinedRooms.IndexOf(roomToBeUpdated)].room.messages = list.ToArray();
                }

            }

        }
    }
}
