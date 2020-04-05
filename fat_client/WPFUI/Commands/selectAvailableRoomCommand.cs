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
    public class selectAvailableRoomCommand : ICommand, IselectAvailableRoomCommand
    {
        public event EventHandler CanExecuteChanged;
        public IEventAggregator _events;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Console.WriteLine("Available Room clicked");

            _events.PublishOnUIThread(new refreshRoomsEvent((string)parameter, false));
        }

        public selectAvailableRoomCommand(IEventAggregator events)
        {
            _events = events;
        }
    }
}
