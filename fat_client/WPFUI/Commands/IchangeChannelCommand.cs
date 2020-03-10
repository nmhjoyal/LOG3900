using System;

namespace WPFUI.Commands
{
    public interface IchangeChannelCommand
    {
        event EventHandler CanExecuteChanged;

        bool CanExecute(object parameter);
        void Execute(object parameter);
    }
}