using Caliburn.Micro;
using Quobject.SocketIoClientDotNet.Client;
using System.Collections.Generic;
using System.Windows.Ink;

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
        void joinRoom(string roomID);
        void offDrawing();
        void offPreviewing();
        void onDrawing(StrokeCollection Traits, Dictionary<Stroke, int> strokes);
        void onLobby(BindableCollection<Match> matches);
        void onCreateMatch();
        void offCreateMatch();
        void sendMessage();
        void SignOut();
        void TestGETWebRequest(string request);
        void TestPOSTWebRequest(object obj, string url);
    }
}