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
        public BindableCollection<MatchModel> matches;

        public BindableCollection<MatchModel> Matches
        {
            get
            {
                return this.matches;
            }
        }
        
       
        public ChoseGameViewModel(IEventAggregator events, ISocketHandler socketHandler)
        {
            _events = events;
            _socketHandler = socketHandler;
            this.matches = new BindableCollection<MatchModel>();
            // this._socketHandler.onLobby(this.matches);
            this._socketHandler.socket.On("update_matches", (matches) => {
                this.matches.Clear();
                this.matches.AddRange(JsonConvert.DeserializeObject<BindableCollection<MatchModel>>(matches.ToString()));
            });
            this._socketHandler.socket.Emit("get_matches");
        }
        public void goBack()
        {
            this._socketHandler.socket.Off("update_matches");
            _events.PublishOnUIThread(new goBackMainEvent());
        }
        public void joinGame(string matchId)
        {
            Console.WriteLine(matchId);
            this._socketHandler.socket.Off("update_matches");
            _events.PublishOnUIThread(new gameEvent());
        }
        public void createGame()
        {
            this._socketHandler.socket.Off("update_matches");
            _events.PublishOnUIThread(new createMatchEvent());
        }
    }
}
