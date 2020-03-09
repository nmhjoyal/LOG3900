﻿using Quobject.SocketIoClientDotNet.Client;
using System;

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
       // void On(object eVENT_DISCONNECT, Action p);
       // void On(object eVENT_DISCONNECT);
        //void sendMessage();
        void SignOut();
    }
}