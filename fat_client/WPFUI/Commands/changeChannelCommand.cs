using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPFUI.EventModels;
using WPFUI.Models;

namespace WPFUI.Commands
{
    public class changeChannelCommand : ICommand, IchangeChannelCommand
    {
        public event EventHandler CanExecuteChanged;

        private IUserData _userdata;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            _userdata.changeChannel((string)parameter);
        }

        public changeChannelCommand(IUserData userdata)
        {
            _userdata = userdata;
        }
    }
}
