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
    class ChatRoomChannelsViewModel
    {
        private IEventAggregator _events;
        private ISocketHandler _socketHandler;

        public ChatRoomChannelsViewModel(IEventAggregator events, ISocketHandler socketHandler)
        {
            _events = events;
            _socketHandler = socketHandler;
        }

        public void joinChat()
        {
            _events.PublishOnUIThread(new joinChatEvent());
        }
    }
}
