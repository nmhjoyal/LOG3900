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
    class ShellViewModel: Conductor<object>, IHandle<LogInEvent>, IHandle<logOutEvent>, IHandle<joinChatEvent>,
						  IHandle<DisconnectEvent>, IHandle<userNameTakenEvent>,IHandle<signUpEvent>, IHandle<goBackEvent>,
						  IHandle<passwordMismatchEvent>, IHandle<viewProfileEvent>, IHandle<goBackMainEvent>
	{
		private IEventAggregator _events;
		private SimpleContainer _container;

		public ShellViewModel(IEventAggregator events, SimpleContainer container)
		{
			_container = container;
			_events = events;
			_events.Subscribe(this);
			ActivateItem(_container.GetInstance<FenetreDessinViewModel>());
			//ActivateItem(_container.GetInstance<MainMenuViewModel>());
		}

		public void Handle(LogInEvent message)
		{
			ActivateItem(_container.GetInstance<MainMenuViewModel>());
		}

		public void Handle(viewProfileEvent message)
		{
			ActivateItem(_container.GetInstance<profileViewModel>());
		}

		public void Handle(goBackEvent message)
		{
			ActivateItem(_container.GetInstance<LoginViewModel>());
		}

		public void Handle(goBackMainEvent message)
		{
			ActivateItem(_container.GetInstance<MainMenuViewModel>());
		}

		public void Handle(signUpEvent message)
		{
			ActivateItem(_container.GetInstance<NewUserViewModel>());
		}

		public void Handle(logOutEvent message)
		{
			ActivateItem(_container.GetInstance<LoginViewModel>());
		}

		public void Handle(joinChatEvent message)
		{
			ActivateItem(_container.GetInstance<chatBoxViewModel>());
		}

		public void Handle(joinChatroomEvent message)
		{
			ActivateItem(_container.GetInstance<ChatRoomChannelsViewModel>());
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

		public void Handle(passwordMismatchEvent message)
		{
			string messageBoxText = "passwords don't match";
			string caption = "Warning";
			MessageBoxButton button = MessageBoxButton.OK;
			MessageBoxImage icon = MessageBoxImage.Warning;

			MessageBox.Show(messageBoxText, caption, button, icon);
		}
	}
}
