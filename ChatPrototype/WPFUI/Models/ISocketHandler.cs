using Quobject.SocketIoClientDotNet.Client;

namespace WPFUI.Models
{
    public interface ISocketHandler
    {
        Socket socket { get; set; }
        User user { get; set; }

        void connectionAttempt();
        void createUser(User user);
        void disconnect();
        void sendMessage();
    }
}