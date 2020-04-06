using Caliburn.Micro;
using Quobject.SocketIoClientDotNet.Client;
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
    class ShellViewModel: Conductor<Screen>.Collection.AllActive, IHandle<LogInEvent>, IHandle<logOutEvent>,
						  IHandle<DisconnectEvent>, IHandle<userNameTakenEvent>,IHandle<signUpEvent>, IHandle<goBackEvent>,
						  IHandle<passwordMismatchEvent>, IHandle<viewProfileEvent>, IHandle<goBackMainEvent>,
						  IHandle<joinGameEvent>, IHandle<ManuelIEvent>, IHandle<ManuelleIIEvent>, IHandle<createGameEvent>,IHandle<freeDrawEvent>, 
						  IHandle<joinChatroomEvent>, IHandle<goBackCreationMenuEvent>, IHandle<AssisteIEvent>, IHandle<LeaderboardEvent>, IHandle<gameEvent>, 
						  IHandle<waitingRoomEvent>, IHandle<createMatchEvent>
						  
	{
		private IEventAggregator _events;
		private SimpleContainer _container;
		private IWindowManager _windowManager;
		private ISocketHandler _socketHandler;

		public ShellViewModel(IWindowManager windowManager, IEventAggregator events, SimpleContainer container, ISocketHandler socketHandler)
		{
			_windowManager = windowManager;
			_container = container;
			_events = events;
			_events.Subscribe(this);
			Items.Add(_container.GetInstance<LoginViewModel>());
			Items.Add(_container.GetInstance<EmptyViewModel>());
			this._socketHandler = socketHandler;
		}

		public IEventAggregator events
		{
			get { return _events; }
		}

		public Screen FirstSubViewModel
		{
			get { return Items.ElementAt(0); }
		}

		public Screen SecondSubViewModel
		{
			get { return Items.ElementAt(1); }
		}

		public Screen topMenu
		{
			get { return _container.GetInstance<topMenuViewModel>(); }
		}

		public void Handle(LogInEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<MainMenuViewModel>());
			Items.Add(_container.GetInstance<chatBoxViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}

		public void Handle(viewProfileEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<profileViewModel>());
			Items.Add(_container.GetInstance<chatBoxViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}

		public void Handle(ManuelIEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<CreationJeuManuelle1ViewModel>());
			Items.Add(_container.GetInstance<EmptyViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}

		public void Handle(AssisteIEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<CreationJeuAssiste1ViewModel>());
			Items.Add(_container.GetInstance<chatBoxViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}
		public void Handle(ManuelleIIEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<CreationJeuManuelleViewModel>());
			Items.Add(_container.GetInstance<EmptyViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}

		public void Handle(LeaderboardEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<LeaderboardViewModel>());
			Items.Add(_container.GetInstance<chatBoxViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}

		public void Handle(goBackCreationMenuEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<MenuSelectionModeCreationViewModel>());
			Items.Add(_container.GetInstance<chatBoxViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}

		public void Handle(goBackEvent message)
		{
			//ActivateItem(_container.GetInstance<LoginViewModel>());
			Items.Clear();
			Items.Add(_container.GetInstance<LoginViewModel>());
			Items.Add(_container.GetInstance<EmptyViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}

		public void Handle(goBackMainEvent message)
		{
			//ActivateItem(_container.GetInstance<MainMenuViewModel>());
			Items.Clear();
			Items.Add(_container.GetInstance<MainMenuViewModel>());
			Items.Add(_container.GetInstance<chatBoxViewModel>());
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

		public void Handle(createGameEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<MenuSelectionModeCreationViewModel>());
			Items.Add(_container.GetInstance<chatBoxViewModel>());
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

		public void Handle(joinChatroomEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<chatBoxViewModel>());
			Items.Add(_container.GetInstance<EmptyViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
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
			Items.Clear();
			Items.Add(_container.GetInstance<ChoseGameViewModel>());
			Items.Add(_container.GetInstance<chatBoxViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}
		public void Handle(gameEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<partieJeuViewModel>());
			Items.Add(_container.GetInstance<chatBoxViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}
		public void Handle(waitingRoomEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<WaitingRoomViewModel>());
			Items.Add(_container.GetInstance<chatBoxViewModel>());
			NotifyOfPropertyChange(() => FirstSubViewModel);
			NotifyOfPropertyChange(() => SecondSubViewModel);
		}
		public void Handle(createMatchEvent message)
		{
			Items.Clear();
			Items.Add(_container.GetInstance<createMatchViewModel>());
			Items.Add(_container.GetInstance<chatBoxViewModel>());
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
	}
}
