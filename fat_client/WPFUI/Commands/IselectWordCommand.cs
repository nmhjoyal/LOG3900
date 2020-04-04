using System;

namespace WPFUI.Commands
{
    public interface IselectWordCommand
    {
        event EventHandler CanExecuteChanged;

        bool CanExecute(object parameter);
        void Execute(object parameter);
    }
}