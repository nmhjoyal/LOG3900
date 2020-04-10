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
    class ChoseGameViewModel:Screen
    {
        private IEventAggregator _events;
        private ISocketHandler _socketHandler;
        private IUserData userdata;
        public BindableCollection<Match> matches;
        public Boolean _addClicked;

        public BindableCollection<Match> Matches
        {
            get
            {
                return this.matches;
            }
        }
        
       
        public ChoseGameViewModel(IEventAggregator events, ISocketHandler socketHandler, IUserData userdata)
        {
            _events = events;
            _addClicked = false;
            _events.Subscribe(this);
            this.userdata = userdata;
            _socketHandler = socketHandler;
            this.matches = new BindableCollection<Match>();
            // this._socketHandler.onLobby(this.matches);
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
            this._socketHandler.socket.Emit("join_match", matchId);
            this.userdata.matchMode = this.matches.Single(match => match.matchId == matchId).matchMode;
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
    }
}
