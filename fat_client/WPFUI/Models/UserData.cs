using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.EventModels;

namespace WPFUI.Models
{
    public class UserData : IHandle<roomsRetrievedEvent>, IHandle<joinedRoomReceived>, IHandle<refreshInvitesEvent>, IUserData
    {
        private string _userName;
        private string _password;
        private string _ipAdress;
        private string _currentMessage;
        private string _currentRoomId;
        private string _avatarName;
        public BindableCollection<Invitation> _invites;
        private BindableCollection<Message> _messages;
        private BindableCollection<SelectableRoom> _selectablePublicRooms;
        private BindableCollection<SelectableRoom> _selectableJoinedRooms;
        private BindableCollection<PublicProfile> _modifiedProfiles;
        private BindableCollection<Avatar> _avatars = new BindableCollection<Avatar>();
        private Room _currentGameRoom;
        private string _matchId;
        private int _nbRounds;
        private IEventAggregator _events;
        private MatchMode _matchMode;

        public string getAvatarSource(string avatarName)
        {
            try { return _avatars.Single(i => i.name == avatarName).source; }
            catch { return "/Resources/apple.png"; }
        }

        public string userAvatarSource
        {
            get { return _avatarName; }
        }

        public void fillAvatars()
        {
            _avatars.Add(new Avatar("/Resources/apple.png", "APPLE"));
            _avatars.Add(new Avatar("/Resources/avocado.png", "AVOCADO"));
            _avatars.Add(new Avatar("/Resources/banana.png", "BANANA"));
            _avatars.Add(new Avatar("/Resources/cherry.png", "CHERRY"));
            _avatars.Add(new Avatar("/Resources/grape.png", "GRAPE"));
            _avatars.Add(new Avatar("/Resources/kiwi.png", "KIWI"));
            _avatars.Add(new Avatar("/Resources/lemon.png", "LEMON"));
            _avatars.Add(new Avatar("/Resources/orange.png", "ORANGE"));
            _avatars.Add(new Avatar("/Resources/pear.png", "PEAR"));
            _avatars.Add(new Avatar("/Resources/pineapple.png", "PINEAPPLE"));
            _avatars.Add(new Avatar("/Resources/strawberry.png", "STRAWBERRY"));
            _avatars.Add(new Avatar("/Resources/watermelon.png", "WATERMELON"));
            _avatars.Add(new Avatar("/Resources/chatBox/robot.png", "ADMIN"));
        }

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
            fillAvatars();
            _messages = new BindableCollection<Message>();
            _selectableJoinedRooms = new BindableCollection<SelectableRoom>();
            _selectablePublicRooms = new BindableCollection<SelectableRoom>();
            _modifiedProfiles = new BindableCollection<PublicProfile>();
            _currentRoomId = null;
            _currentGameRoom = null;
            _avatarName = null;
            _invites = new BindableCollection<Invitation>();
        }

        public void addModifiedProfile(PublicProfile profile)
        {
            if (profile.username == _userName)
            {
                _avatarName = profile.avatar;
            }

            PublicProfile alreadyExistingMPP = null;
            foreach (PublicProfile pp in _modifiedProfiles)
            {
                if (pp.username == profile.username)
                {
                    alreadyExistingMPP = pp;
                }
            }

            if (alreadyExistingMPP != null)
            {
                _modifiedProfiles.Remove(alreadyExistingMPP);
                _modifiedProfiles.Add(profile);
                fixAllRooms(profile);
            }
            else
            {
                _modifiedProfiles.Add(profile);
                fixAllRooms(profile);
            }
        }

        public void fixAllRooms(PublicProfile profile)
        {
            Boolean modifiedCurrentRoom = false;
            foreach (SelectableRoom sR in _selectableJoinedRooms)
            {
                sR.room.avatars[profile.username] = profile.avatar;
            }

            foreach (SelectableRoom sR in _selectableJoinedRooms)
            {
                foreach (Message m in sR.room.messages)
                {
                    if (m.senderName == profile.username)
                    {
                        m.avatarSource = getAvatarSource(profile.avatar);
                        if (m.roomId == _currentRoomId)
                        {
                            modifiedCurrentRoom = true;
                        }
                    }

                }
            }

            if (modifiedCurrentRoom)
            {
                changeChannel(_currentRoomId);
            }
        }


        public void changeChannel(string roomID)
        {
            try
            {
                this.messages = new BindableCollection<Message>((this.selectableJoinedRooms.Single(i => i.id == roomID)).room.messages);
            }
            catch
            {
                this.messages = new BindableCollection<Message>(this.selectableJoinedRooms.Where(x => x.id == roomID).ToList()[0].room.messages);
            }
            this.currentRoomId = roomID;
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
                    SelectableRoom sR = new SelectableRoom(r);
                    foreach (Message m in sR.room.messages)
                    {
                        string MessageAvatarName = "";
                        try { MessageAvatarName = sR.room.avatars[m.senderName]; }
                        catch { }
                        if (m.senderName == "Admin")
                        {
                            MessageAvatarName = "ADMIN";
                        }
                        m.avatarSource = getAvatarSource(MessageAvatarName);
                    }
                    this.selectableJoinedRooms.Add(sR);
                }
            }

            this.currentRoomId = this.selectableJoinedRooms[0].id;
            this.messages = new BindableCollection<Message>(this.selectableJoinedRooms[0].room.messages);
        }

        public void addJoinedRoom(Room room, Boolean isPrivate)
        {
            BindableCollection<SelectableRoom> roomAlreadyExists = new BindableCollection<SelectableRoom>(this.selectableJoinedRooms.Where(x => x.id == room.id));
            Console.WriteLine(JsonConvert.SerializeObject(room.avatars));
            if (roomAlreadyExists.Count() == 0)
            {
                SelectableRoom sR = new SelectableRoom(room);
                foreach (Message m in sR.room.messages)
                {
                    string MessageAvatarName = "";
                    try { MessageAvatarName = sR.room.avatars[m.senderName]; }
                    catch { }
                    if (m.senderName == "Admin")
                    {
                        MessageAvatarName = "ADMIN";
                    }
                    m.avatarSource = getAvatarSource(MessageAvatarName);
                }
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
            Dictionary<string, string> avatars = findTheMap(message.roomId);

            if (avatars != null)
            {
                string MessageAvatarName = "";
                try { MessageAvatarName = avatars[message.senderName]; }
                catch { }
                if (message.senderName == "Admin")
                {
                    MessageAvatarName = "ADMIN";
                }
                if (message.senderName == _userName)
                {
                    MessageAvatarName = this._avatarName;
                }
                try // Aide si l'avatar vient juste d'etre change et toutes les rooms ont pas pu update
                {
                    PublicProfile modifiedProfile = _modifiedProfiles.Single(i => i.username == message.senderName);
                    MessageAvatarName = modifiedProfile.avatar;
                }
                catch { }
                message.avatarSource = getAvatarSource(MessageAvatarName);
            }

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
                _events.PublishOnUIThread(new scrollDownEvent());
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
                        Console.WriteLine("message sent to unjoined room");
                        _events.PublishOnUIThread(new appWarningEvent("a message was sent to an unjoined room"));
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

        public Dictionary<string, string> findTheMap(string roomID)
        {
            try
            {
                IEnumerable<SelectableRoom> enumSR = _selectableJoinedRooms.Where(x => x.id == roomID);
                BindableCollection<SelectableRoom> sRs = new BindableCollection<SelectableRoom>(enumSR);
                if (sRs.Count() != 1)
                {
                    Console.WriteLine("il exite un doublon dans les rooms jointes");
                }
                return sRs[0].room.avatars;
            }
            catch
            {
                if(_currentGameRoom != null)
                {
                    if (_currentGameRoom.id == roomID)
                    {
                        return _currentGameRoom.avatars;
                    }
                    else
                    {
                        return null;
                    }

                } else
                {
                    return null;
                }



            }
        }

        public void Handle(refreshInvitesEvent message)
        {
            //
        }
    }
}
