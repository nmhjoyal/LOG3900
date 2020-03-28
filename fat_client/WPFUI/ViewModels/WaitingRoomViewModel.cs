using Caliburn.Micro;
using Newtonsoft.Json;
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
    class WaitingRoomViewModel: Screen, IHandle<refreshMessagesEvent>, IHandle<addMessageEvent>
    {
        private IEventAggregator _events;
        private ISocketHandler _socketHandler;
        private IUserData _userData;
        private BindableCollection<Models.Message> _messages;
        private string _currentMessage;
        private BindableCollection<Player> players;

        public BindableCollection<Player> Players
        {
            get
            {
                return this.players;
            }
        }
        public WaitingRoomViewModel(IEventAggregator events, ISocketHandler socketHandler, IUserData userdata)
        {
            _events = events;
            _events.Subscribe(this);
            _socketHandler = socketHandler;
            _userData = userdata;
            userdata.messages = new BindableCollection<Models.Message>(userdata.selectableJoinedRooms.Single<SelectableRoom>(i => i.id == _userData.matchId).room.messages);
            this._messages = userdata.messages;
            this.players = new BindableCollection<Player>();
            this._socketHandler.onWaitingRoom(this.players);
            this._socketHandler.socket.Emit("get_players", this._userData.matchId);
        }
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

        public BindableCollection<Models.Message> messages
        {
            get { return _messages; }
            set
            {
                _messages = value;
                NotifyOfPropertyChange(() => messages);
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
        public void goBack()
        {
            this._socketHandler.socket.Emit("leave_match");
        }

        public string username
        {
            get { return _userData.userName; }
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
        public void start()
        {
            _events.PublishOnUIThread(new gameEvent());
        }

        public void addVirtualPlayer()
        {
            this._socketHandler.socket.Emit("add_vp");
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
    }
}
