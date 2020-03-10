using Caliburn.Micro;

namespace WPFUI.Models
{
    public interface IUserData
    {
        BindableCollection<Room> channels { get; set; }
        string currentMessage { get; set; }
        string currentRoomId { get; set; }
        string ipAdress { get; set; }
        BindableCollection<Message> messages { get; set; }
        string password { get; set; }
        string userName { get; set; }

        void changeChannel(string roomID);
        void clearData();
    }
}