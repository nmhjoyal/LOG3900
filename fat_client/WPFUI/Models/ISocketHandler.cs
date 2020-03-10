using Quobject.SocketIoClientDotNet.Client;
using System.Windows.Controls;

namespace WPFUI.Models
{
    public interface ISocketHandler
    {
        bool canConnect { get; set; }
        Socket socket { get; set; }
        string traitJSon { get; set; }
        User user { get; set; }

        void connectionAttempt();
        void createRoom(string roomID);
        void createUser(PrivateProfile privateProfile);
        void disconnect();
        void getPublicChannels();
        void getStrokes(InkCanvas Canvas);
        void sendStroke(string path, string couleur, string width, bool stylusTip);
        void SignOut();
    }
}