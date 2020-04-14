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
    public class selectWordCommand : ICommand, IselectAvatarCommand, IselectWordCommand
    {
        public event EventHandler CanExecuteChanged;
        public IEventAggregator _events;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _events.PublishOnUIThread(new wordSelectedEvent((string)parameter));
        }

        public selectWordCommand(IEventAggregator events)
        {
            _events = events;
        }
    }
}
