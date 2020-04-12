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
        void createRoom(string roomID, bool isPrivate);
        void createUser(PrivateProfile privateProfile);
        void deleteRoom(string roomID);
        void disconnect();
        void getPublicChannels();
        void joinRoom(string roomID);
        void leaveRoom(string roomID);
        void offCreateMatch();
        void offDrawing();
        void offLobby();
        void offMatch();
        void offPreviewing();
        void offWaitingRoom();
        void onCreateMatch();
        void onDrawing(StrokeCollection Traits, Dictionary<Stroke, int> strokes);
        void onLobby(BindableCollection<Match> matches);
        void onMatch(StartTurn startTurn, EndTurn endTurn, GuessesLeft guessesLeft);
        void onWaitingRoom(BindableCollection<Player> players);
        void sendMessage();
        void SignOut();
        object TestGETWebRequest(string url);
        void TestPOSTWebRequest(object obj, string url);
    }
}