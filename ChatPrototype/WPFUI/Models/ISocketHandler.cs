using Quobject.SocketIoClientDotNet.Client;

namespace WPFUI.Models
{
    public interface ISocketHandler
    {
        bool canConnect { get; set; }
        Socket socket { get; set; }
        User user { get; set; }

        void connect();
        void connectionAttempt();
        void createUser(User user);
        void disconnect();
        void sendMessage();
    }
}