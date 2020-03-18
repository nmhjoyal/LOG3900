using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Windows.Controls;
using System.Windows.Ink;

namespace WPFUI.Models
{
    public interface ISocketHandler
    {
        bool canConnect { get; set; }
        Socket socket { get; set; }
        string traitJSon { get; set; }
        User user { get; set; }

        void freeDraw(StrokeCollection Traits, DrawingAttributes AttributsDessin);

        void preview(StrokeCollection Traits, GamePreview gamePreview);

        void TestPOSTWebRequest(Object obj, string url);

        void TestGETWebRequest(string url);
        
        void connectionAttempt();
        void createRoom(string roomID);
        void createUser(PrivateProfile privateProfile);
        void disconnect();
        void getPublicChannels();
        void sendMessage();
        void SignOut();
    }
}