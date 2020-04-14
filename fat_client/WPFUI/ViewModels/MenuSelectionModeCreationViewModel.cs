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
    class MenuSelectionModeCreationViewModel : Screen
    {
        private IEventAggregator _events;
        private ISocketHandler _socketHandler;

        public MenuSelectionModeCreationViewModel(IEventAggregator events, ISocketHandler socketHandler)
        {
            _events = events;
            _socketHandler = socketHandler;
        }

        public void manuel2()
        {
            _events.PublishOnUIThread(new ManuelleIIEvent());
        }

        public void assiste1()
        {
            _events.PublishOnUIThread(new AssisteIEvent());
        }
        public void goBack()
        {
            _events.PublishOnUIThread(new goBackMainEvent());
        }
    }
}
