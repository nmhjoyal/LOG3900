using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.EventModels;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    class ChoseGameViewModel : Screen, IHandle<updateMatchesEvent>, IHandle<joinMatchEvent>
    {
        private IEventAggregator _events;
        private ISocketHandler _socketHandler;
        private IUserData userdata;
        public BindableCollection<Match> matches;
        public BindableCollection<Match> filteredMatches;
        public Boolean _addClicked;
        private int index;
        private bool clicked;
        public BindableCollection<Match> Matches
        {
            get { return this.filteredMatches; }
        }

        public int Index
        {
            get { return this.index; }
            set { this.index = value; }
        }
        public IUserData userData
        {
            get { return userdata; }
        }

        public IEventAggregator events
        {
            get { return _events; }
        }


        public ChoseGameViewModel(IEventAggregator events, ISocketHandler socketHandler, IUserData userdata)
        {
            _events = events;
            _addClicked = false;
            _events.Subscribe(this);
            this.userdata = userdata;
            _socketHandler = socketHandler;
            this.clicked = false;
            this.index = 0;
            this.filteredMatches = new BindableCollection<Match>();
            this.matches = new BindableCollection<Match>();
            this._socketHandler.onLobby(this.matches);
            this._socketHandler.socket.Emit("get_matches");
        }

        public void gameViewTesting()
        {
            _events.PublishOnUIThread(new gameEvent());
        }

        public void goBack()
        {
            this._socketHandler.socket.Off("update_matches");
            _events.PublishOnUIThread(new goBackMainEvent());
        }
        public void joinGame(string matchId)
        {
            if(!this.clicked)
            {
                this.clicked = true;
                this._socketHandler.socket.Emit("join_match", matchId);
                this.userdata.matchMode = this.matches.Single(match => match.matchId == matchId).matchMode;
            }
            // _events.PublishOnUIThread(new gameEvent());
        }
        public void createGame()
        {
            if (!_addClicked)
            {
                _addClicked = true;
                this._socketHandler.offLobby();
                _events.PublishOnUIThread(new createMatchEvent());
                _addClicked = false;
            }
        }

        private MatchMode getMatchMode(int index)
        {
            switch(index)
            {
                case 0:
                    return MatchMode.freeForAll;
                case 1:
                    return MatchMode.sprintCoop;
                case 2:
                    return MatchMode.oneVsOne;
                default:
                    return MatchMode.sprintSolo;
            }
        }

        public void Handle(updateMatchesEvent message)
        {
            this.matches.Clear();
            this.matches.AddRange(message.matches);
            this.updateMatches();
        }

        // When selection changes
        public void updateMatches()
        {
            this.filteredMatches.Clear();
            this.filteredMatches.AddRange(this.matches.Where(match => match.matchMode == this.getMatchMode(this.Index)));
        }

        public void Handle(joinMatchEvent message)
        {
            this.clicked = false;
        }
    }
}
