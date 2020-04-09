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
        public BindableCollection<Match> matches;

        public BindableCollection<Match> Matches
        {
            get
            {
                return this.matches;
            }
        }
        
       
        public ChoseGameViewModel(IEventAggregator events, ISocketHandler socketHandler)
        {
            _events = events;
            _events.Subscribe(this);
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
        }
        public void createGame()
        {
            this._socketHandler.offLobby();
            _events.PublishOnUIThread(new createMatchEvent());
        }
    }
}
