﻿using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.EventModels;

namespace WPFUI.ViewModels
{
    public class chatBoxViewModel: Screen
    {
        private IEventAggregator _events;

        public chatBoxViewModel(IEventAggregator events)
        {
            _events = events;
        }

        public void disconnect()
        {
            _events.PublishOnUIThread(new DisconnectEvent());
        }
    }

}
