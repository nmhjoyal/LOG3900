using Caliburn.Micro;
using WPFUI.EventModels;

namespace WPFUI.Models
{
    public interface IUserData
    {
        string currentMessage { get; set; }
        string currentRoomId { get; set; }
        string ipAdress { get; set; }
        BindableCollection<Room> joinedRooms { get; set; }
        BindableCollection<Message> messages { get; set; }
        string password { get; set; }
        BindableCollection<Room> publicRooms { get; set; }
        string userName { get; set; }

        void addRoom(Room room);
        void changeChannel(string roomID);
        void Handle(joinedRoomReceived message);
        void Handle(roomsRetrievedEvent message);
    }
}