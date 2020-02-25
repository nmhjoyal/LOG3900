using Quobject.SocketIoClientDotNet.Client;

namespace WPFUI.Models
{
    public interface ISocketHandler
    {
        bool canConnect { get; set; }
        Socket socket { get; set; }
        User user { get; set; }

        void connectionAttempt();
        void createUser(PrivateProfile privateProfile);
        void disconnect();
        void sendMessage();
    }
}