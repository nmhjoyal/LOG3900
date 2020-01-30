using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.EventModels;

namespace WPFUI.ViewModels
{
    class LoginViewModel: Screen
    {
        private IEventAggregator _events;
        public LoginViewModel(IEventAggregator events)
        {
            _events = events;
        }

        public void logIn()
        {
            _events.PublishOnUIThread(new LogInEvent());

        }

    }
}
