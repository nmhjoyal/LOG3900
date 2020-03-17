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
    class LeaderboardViewModel: Caliburn.Micro.Screen
    {
        private IEventAggregator _events;
        private ISocketHandler _socketHandler;
        public LeaderboardViewModel(IEventAggregator events, ISocketHandler socketHandler)
        {

            _socketHandler = socketHandler;
            _events = events;
            

        }
        public void goBack()
        {
            _events.PublishOnUIThread(new goBackMainEvent());
        }
    }

}
