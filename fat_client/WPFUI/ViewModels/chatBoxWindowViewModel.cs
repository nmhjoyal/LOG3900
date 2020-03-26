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
    public class chatBoxWindowViewModel : Screen, IHandle<refreshMessagesEvent>, IHandle<addMessageEvent>
    {
        private IEventAggregator _events;
        private IUserData _userData;
        private BindableCollection<Models.Message> _messages;
        private string _currentMessage;
        private ISocketHandler _socketHandler;

        public string currentMessage
        {
            get { return _currentMessage; }
            set
            {
                _currentMessage = value;
                NotifyOfPropertyChange(() => currentMessage);
                _userData.currentMessage = value;
            }
        }

        public void keyDown(ActionExecutionContext context)
        {
            var keyArgs = context.EventArgs as KeyEventArgs;

            if (keyArgs != null && keyArgs.Key == Key.Enter)
            {
                //sendMessage();
            }
        }
        public BindableCollection<Models.Message> messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                NotifyOfPropertyChange(() => messages);
            }
        }

        public void sendMessage(string content = null)
        {
            Console.WriteLine("message sending attempted");
            if (content != null)
            {
                _userData.currentMessage = content;
                _socketHandler.sendMessage();
                currentMessage = "";
                _userData.currentMessage = "";
            }
            else if (currentMessage != null & currentMessage != "")
            {
                //messages.Add(new MessageModel(currentMessage, _userData.userName, DateTime.Now));
                _socketHandler.sendMessage();
                currentMessage = "";
                _userData.currentMessage = "";
            }

        }

        public chatBoxWindowViewModel(IUserData userdata, IEventAggregator events, ISocketHandler socketHandler)
        {
            _events = events;
            _events.Subscribe(this);
            _socketHandler = socketHandler;
            _userData = userdata;
            messages = userdata.messages;
            DisplayName = "chatBox";

        }

        public string welcomeMessage
        {
            get
            {
                return $"Welcome to the chatroom {_userData.userName} !";
            }
        }
        public void disconnect()
        {
            _socketHandler.disconnect();
            _events.PublishOnUIThread(new DisconnectEvent());
        }

        public void Handle(refreshMessagesEvent message)
        {
            this._messages = message._messages;
            NotifyOfPropertyChange(() => messages);
        }

        public void Handle(addMessageEvent message)
        {
            Console.WriteLine("hello");
            this._messages.Add(message.message);
            NotifyOfPropertyChange(() => messages);
        }

        public void windowMode()
        {
            _events.PublishOnUIThread(new windowChatEvent());
        }
    }
}
