using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    class chatBoxViewModel: Screen




    {
        private BindableCollection<MessageModel> _messages = new BindableCollection<MessageModel>();
        public chatBoxViewModel()
        {
            _messages.Add(new MessageModel { content = "hello", senderName = "Bob", timeStamp = new DateTime(2020, 1, 29, 7, 0, 0) });
            _messages.Add(new MessageModel { content = "bonjour", senderName = "Justin", timeStamp = new DateTime(2020, 1, 29, 8, 0, 0) });

        }
        public BindableCollection<MessageModel> messages
        {
            get { return _messages; }
            set { _messages = value; }
        }

        private string _message;

        public string message
        {
            get { return _message; }
            set { _message = value;
                  NotifyOfPropertyChange(() => message);
            }
        }

        private string _protoMessages = "";

        public string protoMessages
        {
            get { return _protoMessages; }
            set { _protoMessages = value;
                NotifyOfPropertyChange(() => protoMessages);
            }
        }



        public void sendMessage()
        {
            protoMessages += message + "\n";
            message = "";
        }


    }
    
}
