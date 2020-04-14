using System;

namespace WPFUI.Commands
{
    public interface IselectAvailableRoomCommand
    {
        event EventHandler CanExecuteChanged;

        bool CanExecute(object parameter);
        void Execute(object parameter);
    }
}