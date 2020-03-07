using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFUI.EventModels;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    class ShellViewModel: Conductor<Screen>.Collection.AllActive, IHandle<LogInEvent>, IHandle<logOutEvent>, IHandle<joinChatEvent>,
						  IHandle<DisconnectEvent>, IHandle<userNameTakenEvent>,IHandle<signUpEvent>, IHandle<goBackEvent>,
						  IHandle<passwordMismatchEvent>, IHandle<viewProfileEvent>, IHandle<goBackMainEvent>,
						  IHandle<joinGameEvent>
	{
		private IEventAggregator _events;
		private SimpleContainer _container;
		private IWindowManager _windowManager;

		public ShellViewModel(IWindowManager windowManager, IEventAggregator events, SimpleContainer container)
		{
			_windowManager = windowManager;
			_container = container;
			_events = events;
			_events.Subscribe(this);
			Items.Add(_container.GetInstance<LoginViewModel>());
			Items.Add(_container.GetInstance<EmptyViewModel>());
		}

		public Screen FirstSubViewModel
		{
			get { return Items.ElementAt(0); }
		}

		public Screen SecondSubViewModel
		{
			get { return Items.ElementAt(1); }
		}

		public void Handle(LogInEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<MainMenuViewModel>());
			Items.Add(_container.GetInstance<EmptyViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}

		public void Handle(viewProfileEvent message)
		{
			//ActivateItem(_container.GetInstance<profileViewModel>());
		}

		public void Handle(goBackEvent message)
		{
			//ActivateItem(_container.GetInstance<LoginViewModel>());
		}

		public void Handle(goBackMainEvent message)
		{
			//ActivateItem(_container.GetInstance<MainMenuViewModel>());
		}

		public void Handle(signUpEvent message)
		{
			//ActivateItem(_container.GetInstance<NewUserViewModel>());
		}

		public void Handle(logOutEvent message)
		{
			//ActivateItem(_container.GetInstance<LoginViewModel>());
		}

		public void Handle(joinChatEvent message)
		{
			//ActivateItem(_container.GetInstance<chatBoxViewModel>());
		}

		public void Handle(joinChatroomEvent message)
		{
			//ActivateItem(_container.GetInstance<ChatRoomChannelsViewModel>());
		}

		public void Handle(DisconnectEvent message)
		{
			//ActivateItem(_container.GetInstance<LoginViewModel>());
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

		public void Handle(joinGameEvent message)
		{
			//ActivateItem(_container.GetInstance<gameViewModel>());
		}
	}
}
