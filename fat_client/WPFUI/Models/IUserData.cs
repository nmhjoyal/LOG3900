using Caliburn.Micro;
using WPFUI.EventModels;

namespace WPFUI.Models
{
    public interface IUserData
    {
        string avatarName { get; set; }
        string currentMessage { get; set; }
        string currentRoomId { get; set; }
        string ipAdress { get; set; }
        BindableCollection<Message> messages { get; set; }
        string password { get; set; }
        BindableCollection<SelectableRoom> selectableJoinedRooms { get; set; }
        BindableCollection<SelectableRoom> selectablePublicRooms { get; set; }
        string userName { get; set; } 
        string matchId { get; set; }
        int nbRounds { get; set; }
        MatchMode matchMode { get; set; }
        void addJoinedRoom(Room room);
        void addMessage(Message message);
        void addPublicRoom(Room room);
        void changeChannel(string roomID);
        void Handle(joinedRoomReceived message);
        void Handle(roomsRetrievedEvent message);
    }
}