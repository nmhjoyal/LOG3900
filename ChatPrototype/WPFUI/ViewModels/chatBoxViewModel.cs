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
        private IUserData _userData;
        private BindableCollection<MessageModel> _messages;
        private string _currentMessage;

        public string currentMessage
        {
            get { return _currentMessage; }
            set { _currentMessage = value;
                NotifyOfPropertyChange(() => currentMessage);}
        }


        public BindableCollection<MessageModel> messages
        {
            get { return _messages; }
            set { _messages = value;
                  NotifyOfPropertyChange(() => messages); }
        }

        public void sendMessage()
        {
            if (currentMessage != null & currentMessage != "")
            {
                messages.Add(new MessageModel(currentMessage, _userData.userName, DateTime.Now));
            }

        }


        public chatBoxViewModel(IUserData userdata, IEventAggregator events)
        {
            _events = events;
            _messages = new BindableCollection<MessageModel>();
            messages.Add(new MessageModel("this is a test message", "Barack Obama", DateTime.Now));
            messages.Add(new MessageModel("this is another test message", "Kobe", DateTime.Now));
            _userData = userdata;
        }

        public string welcomeMessage
        {
            get
            {
                return $"Welcome {_userData.userName} ! Server IP: {_userData.ipAdress} ";
            }
        }
        public void disconnect()
        {
            clearUserData();
            _events.PublishOnUIThread(new DisconnectEvent());
        }

        private void clearUserData()
        {
            _messages = new BindableCollection<MessageModel>();
            _userData.ipAdress = "";
            _userData.userName = "";
        }
    }

}
