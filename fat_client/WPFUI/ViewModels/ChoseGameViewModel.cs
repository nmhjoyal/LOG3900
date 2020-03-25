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
    class ChoseGameViewModel:Screen
    {
        private IEventAggregator _events;
        private ISocketHandler _socketHandler;
        
       
        public ChoseGameViewModel(IEventAggregator events, ISocketHandler socketHandler)
        {
            _events = events;
            _socketHandler = socketHandler;
           

        }
        public void goBack()
        {
            _events.PublishOnUIThread(new goBackMainEvent());
        }
        public void rejoindrePartie()
        {
            _events.PublishOnUIThread(new gameEvent());
        }
        public void createGame()
        {
            _events.PublishOnUIThread(new createGameEvent());
        }
    }
}
