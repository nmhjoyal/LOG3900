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
using WPFUI.Views;

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
		private Screen _firstVM;
		private Screen _secondVM;
		private Screen _topmenuVM;
		private int _countHelper;

		public ShellViewModel(IWindowManager windowManager, IEventAggregator events, SimpleContainer container, ISocketHandler socketHandler)
		{
			_windowManager = windowManager;
			_container = container;
			_events = events;
			_events.Subscribe(this);
			_firstVM = _container.GetInstance<LoginViewModel>();
			_secondVM = _container.GetInstance<EmptyViewModel>();
			_topmenuVM = _container.GetInstance<topMenuViewModel>();
			this._socketHandler = socketHandler;
			_countHelper = 0;
		}

		public IEventAggregator events
		{
			get { return _events; }
		}

		public void deactivator(string vmType, Screen screen)
		{
			switch (vmType)
			{
				case "WPFUI.ViewModels.LoginViewModel":
					(screen as LoginViewModel).Unsubscribe();
					break;
				case "WPFUI.ViewModels.partieJeuViewModel":
					_events.Unsubscribe(((screen as partieJeuViewModel).GetView()) as partieJeuView);
					(screen as partieJeuViewModel).Unsubscribe();
					break;
				case "WPFUI.ViewModels.chatBoxViewModel":
					_events.Unsubscribe(((screen as chatBoxViewModel).GetView()) as chatBoxView);
					_events.Unsubscribe(screen as chatBoxViewModel);
					break;

				default:
					break;
			}

		}

		public Screen FirstSubViewModel
		{
			get { return _firstVM; }
			set {
				    deactivator(_firstVM.GetType().ToString(), _firstVM);
					_firstVM = value;
					NotifyOfPropertyChange(() => FirstSubViewModel);
				}
		}

		public Screen SecondSubViewModel
		{
			get { return _secondVM; }
			set {
					deactivator(_secondVM.GetType().ToString(), _secondVM);
					_secondVM = value;
					NotifyOfPropertyChange(() => SecondSubViewModel);
				}
		}

		public Screen topMenu
		{
			get { return _topmenuVM; }
			set { _topmenuVM = null;
				  _topmenuVM = value;
				  NotifyOfPropertyChange(() => topMenu);
				}
		}

		public void Handle(LogInEvent message)
		{
			FirstSubViewModel = _container.GetInstance<MainMenuViewModel>();
			SecondSubViewModel = _container.GetInstance<chatBoxViewModel>();
		}

		public void Handle(viewProfileEvent message)
		{
			FirstSubViewModel = _container.GetInstance<profileViewModel>();
			SecondSubViewModel = _container.GetInstance<chatBoxViewModel>();
		}

		public void Handle(ManuelIEvent message)
		{
			FirstSubViewModel = _container.GetInstance<CreationJeuManuelle1ViewModel>();
			SecondSubViewModel = _container.GetInstance<chatBoxViewModel>();
		}

		public void Handle(AssisteIEvent message)
		{
			FirstSubViewModel = _container.GetInstance<CreationJeuAssiste1ViewModel>();
			SecondSubViewModel = _container.GetInstance<chatBoxViewModel>();
		}
		public void Handle(ManuelleIIEvent message)
		{
			FirstSubViewModel = _container.GetInstance<CreationJeuManuelleViewModel>();
			SecondSubViewModel = _container.GetInstance<chatBoxViewModel>();
		}

		public void Handle(LeaderboardEvent message)
		{
			FirstSubViewModel = _container.GetInstance<ClassementViewModel>();
			SecondSubViewModel = _container.GetInstance<chatBoxViewModel>();
		}

		public void Handle(goBackCreationMenuEvent message)
		{
			FirstSubViewModel = _container.GetInstance<MenuSelectionModeCreationViewModel>();
			SecondSubViewModel = _container.GetInstance<chatBoxViewModel>();
		}

		public void Handle(goBackEvent message)
		{
			FirstSubViewModel = _container.GetInstance<LoginViewModel>();
			SecondSubViewModel = _container.GetInstance<EmptyViewModel>();
		}

		public void Handle(goBackMainEvent message)
		{
			FirstSubViewModel = _container.GetInstance<MainMenuViewModel>();
			SecondSubViewModel = _container.GetInstance<chatBoxViewModel>();
		}

		public void Handle(signUpEvent message)
		{
			FirstSubViewModel = _container.GetInstance<NewUserViewModel>();
			SecondSubViewModel = _container.GetInstance<EmptyViewModel>();
		}

		public void Handle(createGameEvent message)
		{
			FirstSubViewModel = _container.GetInstance<MenuSelectionModeCreationViewModel>();
			SecondSubViewModel = _container.GetInstance<chatBoxViewModel>();
		}

		public void Handle(logOutEvent message)
		{
			FirstSubViewModel = _container.GetInstance<LoginViewModel>();
			SecondSubViewModel = _container.GetInstance<EmptyViewModel>();
		}

		public void Handle(joinChatroomEvent message)
		{
			FirstSubViewModel = _container.GetInstance<chatBoxViewModel>();
			SecondSubViewModel = _container.GetInstance<EmptyViewModel>();
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
			FirstSubViewModel = _container.GetInstance<ChoseGameViewModel>();
			SecondSubViewModel = _container.GetInstance<chatBoxViewModel>();
		}
		public void Handle(gameEvent message)
		{
			_countHelper++;
			FirstSubViewModel = _container.GetInstance<partieJeuViewModel>();
			SecondSubViewModel = _container.GetInstance<chatBoxViewModel>();
		}
		public void Handle(waitingRoomEvent message)
		{
			FirstSubViewModel = _container.GetInstance<WaitingRoomViewModel>();
			SecondSubViewModel = _container.GetInstance<chatBoxViewModel>();
		}
		public void Handle(createMatchEvent message)
		{
			FirstSubViewModel = _container.GetInstance<createMatchViewModel>();
			SecondSubViewModel = _container.GetInstance<chatBoxViewModel>();
		}
		public void Handle(freeDrawEvent message)
		{
			FirstSubViewModel = _container.GetInstance<FenetreDessinViewModel>();
			SecondSubViewModel = _container.GetInstance<chatBoxViewModel>();
		}
	}
}
