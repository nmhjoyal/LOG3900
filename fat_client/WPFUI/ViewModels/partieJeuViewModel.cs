using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;
using WPFUI.Commands;
using WPFUI.EventModels;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    class partieJeuViewModel: Screen, IHandle<refreshMessagesEvent>, IHandle<addMessageEvent>,
                              IHandle<wordSelectedEvent>, IHandle<startTurnRoutineEvent>, IHandle<endTurnRoutineVMEvent>
    {
        private IEventAggregator _events;
        private ISocketHandler _socketHandler;
        private BindableCollection<Avatar> _avatars;
        private IUserData _userData;
        private BindableCollection<Models.Message> _messages;
        private string _currentMessage;
        public BindableCollection<dynamic> _wordChoices;
        public BindableCollection<dynamic> _turnScores;
        public int _currentRound;
        public int _timerContent;
        public DispatcherTimer _timer;
        private int _roundDuration;
        public IselectWordCommand _selectWordCommand { get; set; }

        public partieJeuViewModel(IEventAggregator events, ISocketHandler socketHandler, IUserData userdata)
        {
            _events = events;
            _events.Subscribe(this);
            _socketHandler = socketHandler;
            _userData = userdata;
            messages = userdata.messages;
            _timer = new DispatcherTimer();
            _wordChoices = new BindableCollection<dynamic>();
            _turnScores = new BindableCollection<dynamic>();
            _roundDuration = 30;
            _timerContent = _roundDuration;
            _selectWordCommand = new selectWordCommand(events);
            fillAvatars();
            startTimer();
            this._socketHandler.onMatch();
        }

        public void startTimer()
        {
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += timer_Tick;
            _timer.Start();

        }

        void timer_Tick(object sender, EventArgs e)
        {
         
            if (timerContent != 0)
            {
                timerContent = _timerContent - 1;
            } else
            {
                _timer.Stop();
            }
        }

        public int timerContent
        {
            get { return _timerContent; }
            set { _timerContent = value;
                  NotifyOfPropertyChange(() => timerContent);
                }   
        }

        public int currentRound
        {
            get { return _currentRound; }
        }

        public BindableCollection<dynamic> wordChoices
        {
            get { return _wordChoices; }
        }

        public BindableCollection<dynamic> turnScores
        {
            get { return _turnScores; }
        }

        public IEventAggregator events
        {
            get { return _events; }
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
            _events.PublishOnUIThread(new goBackMainEvent());
        }

        public string username
        {
            get { return _userData.userName; }
        }

        public void fillAvatars()
        {
            _avatars = new BindableCollection<Avatar>();
            _avatars.Add(new Avatar("/Resources/apple.png", "APPLE"));
            _avatars.Add(new Avatar("/Resources/avocado.png", "AVOCADO"));
            _avatars.Add(new Avatar("/Resources/banana.png", "BANANA"));
            _avatars.Add(new Avatar("/Resources/cherry.png", "CHERRY"));
            _avatars.Add(new Avatar("/Resources/grape.png", "GRAPE"));
            _avatars.Add(new Avatar("/Resources/kiwi.png", "KIWI"));
            _avatars.Add(new Avatar("/Resources/lemon.png", "LEMON"));
            _avatars.Add(new Avatar("/Resources/orange.png", "ORANGE"));
            _avatars.Add(new Avatar("/Resources/pear.png", "PEAR"));
            _avatars.Add(new Avatar("/Resources/pineapple.png", "PINEAPPLE"));
            _avatars.Add(new Avatar("/Resources/strawberry.png", "STRAWBERRY"));
            _avatars.Add(new Avatar("/Resources/watermelon.png", "WATERMELON"));
        }
        public string avatarSource
        {
            get { return _avatars.Single(i => i.name == _userData.avatarName).source; }
        }

        public void sendMessage()
        {
            _socketHandler.sendMessage();
            currentMessage = "";
            _userData.currentMessage = "";
        }

        public void newWords( List<string> choices)
        {
            _wordChoices.Clear();
            foreach (string word in choices)
            {
                dynamic w = new System.Dynamic.ExpandoObject();
                w.word = word;
                _wordChoices.Add(w);
            }
            wordChoices.Refresh();
        }

        public void newScores(List<UsernameUpdateScore> scores)
        {
            _turnScores.Clear();
            Console.WriteLine("!" + scores.Count);
            foreach (UsernameUpdateScore score in scores)
            {
                dynamic dynamicScore = new System.Dynamic.ExpandoObject();
                dynamicScore.position = 0;
                dynamicScore.name = score.username;
                dynamicScore.score = score.scoreTotal;
                _turnScores.Add(dynamicScore);
            }
            turnScores.Refresh();
        }

        public void HandleFirstRound()
        {
            _currentRound = this._userData.firstRound.currentRound;
            List<UsernameUpdateScore> scores = new List<UsernameUpdateScore>(this._userData.firstRound.scores);
            Console.WriteLine(scores.Count);
            this.newScores(scores);

            dynamic endTurn = new System.Dynamic.ExpandoObject();
            endTurn.currentRound = this._userData.firstRound.currentRound;
            endTurn.drawer = this._userData.firstRound.drawer;
            endTurn.nextIsYou = this._userData.firstRound.drawer == this._userData.userName;
            newWords(this._userData.firstRound.choices);
            newScores(this._userData.firstRound.scores);
            _events.PublishOnUIThread(new endTurnRoutineEvent(endTurn));
        }


        public void Handle(refreshMessagesEvent message)
        {
            this._messages = message._messages;
            NotifyOfPropertyChange(() => messages);
        }

        public void Handle(addMessageEvent message)
        {
            this._messages.Add(message.message);
            NotifyOfPropertyChange(() => messages);
        }

        public void Handle(wordSelectedEvent message)
        {
            Console.WriteLine("!" + message.word);
            /* TODO envoyer le mot au serveur */
            _socketHandler.socket.Emit("start_turn", message.word);
        }

        public void Handle(startTurnRoutineEvent message)
        {
            _timerContent = message.turnTime;
            _timer.Start();
        }

        public void Handle(endTurnRoutineVMEvent message)
        {
            this.HandleFirstRound();
        }
    }
}
