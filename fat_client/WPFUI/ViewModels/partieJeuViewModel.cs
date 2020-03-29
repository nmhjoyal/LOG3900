﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Caliburn.Micro;
using WPFUI.EventModels;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    class partieJeuViewModel: Screen, IHandle<refreshMessagesEvent>, IHandle<addMessageEvent>
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
            _currentRound = 1;
            _roundDuration = 30;
            _timerContent = _roundDuration;
            fillAvatars();
            startTimer();
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

        public void sendMessage(string content = null)
        {
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

        public void mockEndTurn()
        {
            dynamic endTurn = new System.Dynamic.ExpandoObject();
            endTurn.currentRound = 1;
            endTurn.drawer = "Karima";
            endTurn.nextIsYou = true;
            newWords();
            newScores();
            _events.PublishOnUIThread(new endTurnRoutineEvent(endTurn));
        }

        public void newWords()
        {
            _wordChoices.Clear();
            dynamic word1 = new System.Dynamic.ExpandoObject();
            word1.word = "Corona";
            dynamic word2 = new System.Dynamic.ExpandoObject();
            word2.word = "Coors Lite";
            dynamic word3 = new System.Dynamic.ExpandoObject();
            word3.word = "Molson Ex";
            _wordChoices.Add(word1);
            _wordChoices.Add(word2);
            _wordChoices.Add(word3);
            wordChoices.Refresh();
        }

        public void newScores()
        {
            _turnScores.Clear();
            dynamic score1 = new System.Dynamic.ExpandoObject();
            score1.position = 1;
            score1.name = "Karima";
            score1.score = 200;
            dynamic score2 = new System.Dynamic.ExpandoObject();
            score2.position = 2;
            score2.name = "Seb";
            score2.score = 150;

            dynamic score3 = new System.Dynamic.ExpandoObject();
            score3.position = 2;
            score3.name = "Nicowle";
            score3.score = 140;

            _turnScores.Add(score1);
            _turnScores.Add(score2);
            _turnScores.Add(score3);
            turnScores.Refresh();
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
    }
}
