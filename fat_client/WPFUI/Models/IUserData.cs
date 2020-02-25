using Caliburn.Micro;

namespace WPFUI.Models
{
    public interface IUserData
    {
        BindableCollection<Channel> channels { get; set; }
        string currentMessage { get; set; }
        string ipAdress { get; set; }
        BindableCollection<MessageModel> messages { get; set; }
        string password { get; set; }
        string userName { get; set; }

        void changeChannel(int channelID);
        void clearData();
    }
}