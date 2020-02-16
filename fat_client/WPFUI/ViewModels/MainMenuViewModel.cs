using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.EventModels;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    class MainMenuViewModel
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
            _events.PublishOnUIThread(new logOutEvent());
        }

        public void joinChatroom()
        {
            _events.PublishOnUIThread(new joinChatroomEvent());
        }
    }
}
