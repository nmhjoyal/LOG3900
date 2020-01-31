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

        public string currentMessage
        {
            get { return _currentMessage; }
            set { _currentMessage = value;
                NotifyOfPropertyChange(() => currentMessage);}
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
                messages.Add(new MessageModel(currentMessage, _userData.userName, DateTime.Now));
                currentMessage = "";
            }

        }

        public chatBoxViewModel(IUserData userdata, IEventAggregator events)
        {
            _events = events;
            _messages = new BindableCollection<MessageModel>();
            addFakeMessages();
            _userData = userdata;
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
            _events.PublishOnUIThread(new DisconnectEvent());
        }

        private void clearUserData()
        {
            _messages = new BindableCollection<MessageModel>();
            _userData.ipAdress = "";
            _userData.userName = "";
        }

        private void addFakeMessages()
        {
            messages.Add(new MessageModel("this is a test message", "Barack Obama", DateTime.Now));
            messages.Add(new MessageModel("this is another test message", "Kobe", DateTime.Now));
            messages.Add(new MessageModel("yo le serveur c'est chaud", "Hubert", DateTime.Now));
            messages.Add(new MessageModel("WOW c'est vrm beau mtnt ! on va manger ?", "Karima", DateTime.Now));
            messages.Add(new MessageModel("non jte donnerais par leur code !!", "Amar", DateTime.Now));
            messages.Add(new MessageModel("aight, t'auras pas de sockets", "Zackary", DateTime.Now));
            messages.Add(new MessageModel("qui a fait une activite de 5hr ? Inacceptable", "Sebastien C.", DateTime.Now));
            messages.Add(new MessageModel("cette architecture est a chier", "Oliver G.", DateTime.Now));
        }
    }

}
