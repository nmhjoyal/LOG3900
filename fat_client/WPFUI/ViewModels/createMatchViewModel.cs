using Caliburn.Micro;
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


        public createMatchViewModel(IEventAggregator events, ISocketHandler socketHandler)
        {
            _events = events;
            _socketHandler = socketHandler;


        }
        public void goBack()
        {
            _events.PublishOnUIThread(new goBackMainEvent());
        }
        public void createMatch(MatchMode matchMode, int nbRounds, int timeLimit)
        {
            _events.PublishOnUIThread(new waitingRoomEvent());
        }
    }
}
