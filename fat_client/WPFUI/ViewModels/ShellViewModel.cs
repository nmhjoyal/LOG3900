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
    class ShellViewModel: Conductor<Screen>.Collection.AllActive, IHandle<LogInEvent>, IHandle<logOutEvent>, IHandle<joinChatEvent>, IHandle<createGameEvent>,
						  IHandle<DisconnectEvent>, IHandle<manuel1Event>, IHandle<userNameTakenEvent>,IHandle<signUpEvent>, IHandle<goBackEvent>, IHandle<joinChatroomEvent>,
						  IHandle<passwordMismatchEvent>, IHandle<viewProfileEvent>, IHandle<goBackMainEvent>, IHandle<freeDrawEvent>, IHandle<fullScreenChatEvent>
	{
		private IEventAggregator _events;
		private SimpleContainer _container;
		private IWindowManager _windowManager;
		private IUserData _userData;
		private ISocketHandler _socketHandler;

		public ShellViewModel(IWindowManager windowManager, IEventAggregator events, SimpleContainer container,
							  IUserData userdata, ISocketHandler socketHandler)
		{
			_container = container;
			_windowManager = windowManager;
			_events = events;
			_userData = userdata;
			_socketHandler = socketHandler;
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
		public void Handle(manuel1Event message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<CreationJeuManuelle1ViewModel>());
			Items.Add(_container.GetInstance<FenetreDessinViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}

		public void Handle(viewProfileEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<profileViewModel>());
			Items.Add(_container.GetInstance<EmptyViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}

		public void Handle(freeDrawEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<FenetreDessinViewModel>());
			Items.Add(_container.GetInstance<chatBoxViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}

		
		public void Handle(fullScreenChatEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<chatBoxViewModel>());
			Items.Add(_container.GetInstance<EmptyViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}

		public void Handle(goBackEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<LoginViewModel>());
			Items.Add(_container.GetInstance<EmptyViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}
		public void Handle(createGameEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<MenuSelectionModeCreationViewModel>());
			Items.Add(_container.GetInstance<EmptyViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}
		public void Handle(goBackMainEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<MainMenuViewModel>());
			Items.Add(_container.GetInstance<EmptyViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}

		public void Handle(signUpEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<NewUserViewModel>());
			Items.Add(_container.GetInstance<EmptyViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}

		public void Handle(logOutEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<LoginViewModel>());
			Items.Add(_container.GetInstance<EmptyViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}

		public void Handle(joinChatEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<chatBoxViewModel>());
			Items.Add(_container.GetInstance<EmptyViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}

		public void Handle(joinChatroomEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<MultiChannelChatBoxViewModel>());
			Items.Add(_container.GetInstance<EmptyViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}

		public void Handle(DisconnectEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<LoginViewModel>());
			Items.Add(_container.GetInstance<EmptyViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
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
