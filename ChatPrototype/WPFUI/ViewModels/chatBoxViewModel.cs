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
    public class chatBoxViewModel: Screen
    {
        private IEventAggregator _events;
        private List<MessageModel> _messages;

        public chatBoxViewModel(IEventAggregator events)
        {
            _events = events;
            _messages = new List<MessageModel>();
        }

        public void disconnect()
        {
            _events.PublishOnUIThread(new DisconnectEvent());
        }
    }

}
