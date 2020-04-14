using System;

namespace WPFUI.Commands
{
    public interface IselectJoinedRoomCommand
    {
        event EventHandler CanExecuteChanged;

        bool CanExecute(object parameter);
        void Execute(object parameter);
    }
}