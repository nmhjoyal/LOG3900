using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
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
        private ISocketHandler _socketHandler;

        public string currentMessage
        {
            get { return _currentMessage; }
            set { _currentMessage = value;
                NotifyOfPropertyChange(() => currentMessage);
                _userData.currentMessage = value;
            }
        }

        public void keyDown(ActionExecutionContext context)
        {
            var keyArgs = context.EventArgs as KeyEventArgs;

            if (keyArgs != null && keyArgs.Key == Key.Enter)
            {
                sendMessage();
            }
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
                //messages.Add(new MessageModel(currentMessage, _userData.userName, DateTime.Now));
                _socketHandler.sendMessage();
                currentMessage = "";
                _userData.currentMessage = "";
            }

        }

        public chatBoxViewModel(IUserData userdata, IEventAggregator events, ISocketHandler socketHandler)
        {
            _events = events;
            _socketHandler = socketHandler;
            _userData = userdata;
            _messages = _userData.messages;
        }

        public string welcomeMessage
        {
            get
            {
                return $"Welcome to the chatroom {_userData.userName} !" + Environment.NewLine + $"Server IP adress: {_userData.ipAdress} ";
            }
        }
        public void disconnect()
        {
            clearUserData();
            _socketHandler.disconnect();
            _events.PublishOnUIThread(new DisconnectEvent());
        }

        private void clearUserData()
        {
            _userData.clearData();
        }

    }

}
