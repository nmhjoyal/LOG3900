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
    class createMatchViewModel: Screen
    {

        private IEventAggregator _events;
        private ISocketHandler _socketHandler;
        private IUserData userdata;

        public createMatchViewModel(IEventAggregator events, ISocketHandler socketHandler, IUserData userdata)
        {
            _events = events;
            _socketHandler = socketHandler;
            this.userdata = userdata;
            this._socketHandler.onCreateMatch();
        }
        public void goBack()
        {
            _events.PublishOnUIThread(new goBackMainEvent());
        }
        public void createMatch(MatchMode matchMode, int nbRounds, int timeLimit)
        {
            CreateMatch createMatch = new CreateMatch(nbRounds, timeLimit, matchMode);
            this._socketHandler.socket.Emit("create_match", JsonConvert.SerializeObject(createMatch));
            this.userdata.matchMode = matchMode;
        }
    }
}
