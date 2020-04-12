using Caliburn.Micro;
using WPFUI.EventModels;

namespace WPFUI.Models
{
    public interface IUserData
    {
        string avatarName { get; set; }
        Room currentGameRoom { get; set; }
        string currentMessage { get; set; }
        string currentRoomId { get; set; }
        string ipAdress { get; set; }
        string matchId { get; set; }
        MatchMode matchMode { get; set; }
        BindableCollection<Message> messages { get; set; }
        int nbRounds { get; set; }
        string password { get; set; }
        BindableCollection<SelectableRoom> selectableJoinedRooms { get; set; }
        BindableCollection<SelectableRoom> selectablePublicRooms { get; set; }
        string userName { get; set; }

        void addGameRoom(Room room);
        void addJoinedRoom(Room room, bool isPrivate);
        void addMessage(Message message);
        void addPublicRoom(Room room);
        void changeChannel(string roomID);
        void Handle(joinedRoomReceived message);
        void Handle(roomsRetrievedEvent message);
    }
}