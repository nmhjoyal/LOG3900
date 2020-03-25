using System;

namespace WPFUI.Commands
{
    public interface IselectAvatarCommand
    {
        event EventHandler CanExecuteChanged;

        bool CanExecute(object parameter);
        void Execute(object parameter);
    }
}