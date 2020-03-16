using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.EventModels;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    class ShellViewModel: Conductor<object>, IHandle<LogInEvent>, IHandle<DisconnectEvent>, IHandle<userNameTakenEvent>
	{
		private IEventAggregator _events;
		private SimpleContainer _container;

		public ShellViewModel(IEventAggregator events, SimpleContainer container)
		{
			_container = container;
			_events = events;
			_events.Subscribe(this);
			ActivateItem(_container.GetInstance<LoginViewModel>());
		}

		public void Handle(LogInEvent message)
		{
			ActivateItem(_container.GetInstance<chatBoxViewModel>());
		}

		public void Handle(DisconnectEvent message)
		{
			ActivateItem(_container.GetInstance<LoginViewModel>());
		}

		public void Handle(userNameTakenEvent message)
		{
			string messageBoxText = "userName already taken";
			string caption = "Warning";
			MessageBoxButton button = MessageBoxButton.OK;
			MessageBoxImage icon = MessageBoxImage.Warning;

			MessageBox.Show(messageBoxText, caption, button, icon);
		}
	}
}
