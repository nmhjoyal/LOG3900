using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using WPFUI.EventModels;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    class MainMenuViewModel: Screen
    {
        private IEventAggregator _events;
        private ISocketHandler _socketHandler;

        public MainMenuViewModel(IEventAggregator events, ISocketHandler socketHandler)
        {
            _events = events;
            _socketHandler = socketHandler;     
        }

        public void logOut()
        {
            _socketHandler.SignOut();
           
            _events.PublishOnUIThread(new logOutEvent());
        }
        public void viewProfile()
        {
            _events.PublishOnUIThread(new viewProfileEvent());
        }


        public void joinChatroom()
        {
            _events.PublishOnUIThread(new joinChatroomEvent());
        }

        public void joinGame()
        {
            _events.PublishOnUIThread(new joinGameEvent());
        }


    }
}
