using Caliburn.Micro;

namespace WPFUI.Models
{
    public interface IUserData
    {
        string currentMessage { get; set; }
        string ipAdress { get; set; }
        BindableCollection<MessageModel> messages { get; set; }
        string password { get; set; }
        string userName { get; set; }

        void clearData();
    }
}