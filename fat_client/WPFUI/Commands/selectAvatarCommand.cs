using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using WPFUI.EventModels;
using WPFUI.Models;

namespace WPFUI.Commands
{
    public class selectAvatarCommand : ICommand, IselectAvatarCommand
    {
        public event EventHandler CanExecuteChanged;
        public IEventAggregator _events;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _events.PublishOnUIThread(new refreshUIEvent( (string) parameter));
        }

        public selectAvatarCommand(IEventAggregator events)
        {
            _events = events;
        }
    }
}
