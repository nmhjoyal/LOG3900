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
    class topMenuViewModel: Screen
    {
        private IEventAggregator _events;
        private ISocketHandler _socketHandler;

        public topMenuViewModel(IEventAggregator events, ISocketHandler socketHandler)
        {
            _events = events;
            _socketHandler = socketHandler;
            _events.Subscribe(this);
        }

        public void goToScores()
        {
            _events.PublishOnUIThread(new LeaderboardEvent());
        }

        public void goToProfileEdit()
        {
            _events.PublishOnUIThread(new viewProfileEvent());
        }

        public void disconnect()
        {
            _socketHandler.SignOut();
            _events.PublishOnUIThread(new logOutEvent());
        }

        public void goToMenu()
        {
            _events.PublishOnUIThread(new goBackMainEvent());
        }
    }
}
