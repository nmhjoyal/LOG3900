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
        public refreshMessagesEvent(BindableCollection<Models.Message> newMessages)
        {
            _messages = newMessages;
        }
    }
}
