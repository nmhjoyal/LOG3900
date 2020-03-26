using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.EventModels
{
    public class refreshMessagesEvent
    {
        public BindableCollection<Models.Message> _messages;
        public string _currentRoomId;
        public refreshMessagesEvent(BindableCollection<Models.Message> newMessages, string roomId)
        {
            _messages = newMessages;
            _currentRoomId = roomId;
        }
    }
}
