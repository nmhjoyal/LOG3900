using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using WPFUI.Commands;
using WPFUI.EventModels;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
	class profileViewModel : Screen, IHandle<avatarUpdated>
	{
		private BindableCollection<Avatar> _avatars;
		private IUserData _userData;
		private IEventAggregator _events;
		private ISocketHandler _socketHandler;
		private StatsClient statsClient;
		private int _selectedAvatarIndex;
		private int nbAvatars;
		private PrivateProfile _initialPP;
		private PrivateProfile _newlPP;
		private PrivateProfile _ppToBeConfirmed;

		public IEventAggregator events
		{
			get { return _events; }
		}
		public PrivateProfile initialPP
		{
			get { return _initialPP; }
		}

		public PrivateProfile ppToBeConfirmed
		{
			get { return _ppToBeConfirmed; }
		}
		public string userName
		{
			get { return _userData.userName; }
		}

		public string userAvatarSource
		{
			get { return avatars.Single(x => x._name == _userData.avatarName)._source; }
		}
		public string selectedAvatarSource
		{
			get { return _avatars[_selectedAvatarIndex].source; }
		}

		public int selectedAvatarIndex
		{
			get { return _selectedAvatarIndex; }
			set { _selectedAvatarIndex = value;
				  NotifyOfPropertyChange(() => selectedAvatarIndex);
				  NotifyOfPropertyChange(() => selectedAvatarSource);
			}
		}

		private Rank _rank;

		public Rank rank
		{
			get { return _rank; }
			set { _rank = value; }
		}

		private StatsClient stats;

		public StatsClient Stats
		{
			get { return this.stats; }

		}

		private MatchHistory _matchHistory;

		public MatchHistory matchHistory
		{
			get { return this._matchHistory; }

		}
		
		public BindableCollection<MatchHistory> accesseur
		{
			get{
				return statsClient.matchesHistory;
			}
		}
		public profileViewModel(IUserData userdata, IEventAggregator events, ISocketHandler socketHandler)
		{
			_userData = userdata;
			_events = events;
			_events.Subscribe(this);
			_socketHandler = socketHandler;
			this.statsClient = JsonConvert.DeserializeObject<StatsClient>(this._socketHandler.TestGETWebRequest("/profile/stats/" + this._userData.userName).ToString());
			_avatars = new BindableCollection<Avatar>();
			fillAvatars();
			nbAvatars = _avatars.Count();
			_selectedAvatarIndex = 0;
			_initialPP = JsonConvert.DeserializeObject<PrivateProfile>(socketHandler.TestGETWebRequest("/profile/private/" + userdata.userName).ToString());
			_newlPP = new PrivateProfile(_initialPP.firstname, _initialPP.firstname,
										 _initialPP.lastname, _initialPP.password,_initialPP.avatar);
		}

		public void resetNewPP()
		{
			_newlPP = new PrivateProfile(_initialPP.firstname, _initialPP.firstname,
							             _initialPP.lastname, _initialPP.password, _initialPP.avatar);
			NotifyOfPropertyChange(null);
		}

		public string newFirstNameBox
		{
			get { return _newlPP.firstname; }
			set { _newlPP.firstname = value; }
		}

		public string newLastNameBox
		{
			get { return _newlPP.lastname; }
			set { _newlPP.lastname = value; }
		}

		public string fullName
		{
			get { return _initialPP.firstname + " " + _initialPP.lastname; }
		}

		public void saveChanges(string passwordTB, string password2TB, string newFirst, string newLast)
		{
			_socketHandler.avatarChangePending = _avatars[_selectedAvatarIndex].name;

			if(newFirst == null | newFirst == "")
			{
				_events.PublishOnUIThread(new appWarningEvent("The new firstname is empty"));
				resetNewPP();
			}
			else if (newLast == null | newLast == "")
			{
				_events.PublishOnUIThread(new appWarningEvent("The new lastname is empty"));
				resetNewPP();
			}
			else if ((passwordTB != password2TB))
			{
				_events.PublishOnUIThread(new appWarningEvent("The new passwords don't match"));
				resetNewPP();
			}
			else if( passwordTB == "" | passwordTB == null)
			{
				Console.WriteLine("no new pass");
				PrivateProfile newPP = new PrivateProfile(_userData.userName, newFirst, newLast, _initialPP.password, _avatars[_selectedAvatarIndex].name);
				_socketHandler.socket.Emit("update_profile", JsonConvert.SerializeObject(newPP));
				_ppToBeConfirmed = newPP;
				selectedAvatarIndex = 0;
			} else
			{
				Console.WriteLine("new pass");
				PrivateProfile newPP = new PrivateProfile(_userData.userName, newFirst, newLast, passwordTB, _avatars[_selectedAvatarIndex].name);
				_socketHandler.socket.Emit("update_profile", JsonConvert.SerializeObject(newPP));
				_ppToBeConfirmed = newPP;
				selectedAvatarIndex = 0;
			}
		}

		public void leftArrowAvatarChange()
		{
			if (selectedAvatarIndex >= 0 & selectedAvatarIndex < nbAvatars - 1)
			{
				selectedAvatarIndex++;
			} else if (selectedAvatarIndex == nbAvatars - 1)
			{
				selectedAvatarIndex = 0;
			}
		}

		public void rightArrowAvatarChange()
		{
			if (selectedAvatarIndex > 0 & selectedAvatarIndex <= nbAvatars - 1)
			{
				selectedAvatarIndex--;
			}
			else if (selectedAvatarIndex == 0)
			{
				selectedAvatarIndex = nbAvatars - 1;
			}

		}


		public void fillAvatars()
		{
			_avatars.Add(new Avatar("/Resources/apple.png", "APPLE"));
			_avatars.Add(new Avatar("/Resources/avocado.png", "AVOCADO"));
			_avatars.Add(new Avatar("/Resources/banana.png", "BANANA"));
			_avatars.Add(new Avatar("/Resources/cherry.png", "CHERRY"));
			_avatars.Add(new Avatar("/Resources/grape.png", "GRAPE"));
			_avatars.Add(new Avatar("/Resources/kiwi.png", "KIWI"));
			_avatars.Add(new Avatar("/Resources/lemon.png", "LEMON"));
			_avatars.Add(new Avatar("/Resources/orange.png", "ORANGE"));
			_avatars.Add(new Avatar("/Resources/pear.png", "PEAR"));
			_avatars.Add(new Avatar("/Resources/pineapple.png", "PINEAPPLE"));
			_avatars.Add(new Avatar("/Resources/strawberry.png", "STRAWBERRY"));
			_avatars.Add(new Avatar("/Resources/watermelon.png", "WATERMELON"));
		}

		public void Handle(avatarUpdated message)
		{
			if (_ppToBeConfirmed != null)
			{
				_initialPP = _ppToBeConfirmed;
				NotifyOfPropertyChange(null);
				_ppToBeConfirmed = null;
			}
		}


		public BindableCollection<Avatar> avatars
		{
			get { return _avatars; }
			set
			{
				_avatars = value;
				NotifyOfPropertyChange(() => avatars);
			}
		}
		public StatsClient StatsClient
		{
			get { return this.statsClient; }
		}
		
	}
}
