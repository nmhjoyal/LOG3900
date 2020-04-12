using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.Commands;
using WPFUI.EventModels;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
	class profileViewModel : Screen
	{
		private string _changedUsername;
		private string _changedFirstName;
		private string _changedLastName;
		private string _newchangedPassword;
		private BindableCollection<Avatar> _avatars;
		private string _newConfirmedPassword;
		private IUserData _userData;
		private IEventAggregator _events;
		private ISocketHandler _socketHandler;
		private StatsClient statsClient;
		private string _selectedAvatar;
		public IselectAvatarCommand _selectAvatarCommand { get; set; }

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
			_socketHandler = socketHandler;
			this.statsClient = JsonConvert.DeserializeObject<StatsClient>(this._socketHandler.TestGETWebRequest("/profile/stats/" + this._userData.userName).ToString());
			_avatars = new BindableCollection<Avatar>();
			fillAvatars();
			_selectAvatarCommand = new selectAvatarCommand(events);
			_selectedAvatar = null;
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
		public string newConfirmedPassword
		{
			get { return _newConfirmedPassword; }
			set { _newConfirmedPassword = value; }
		}
		public string newchangedPassword
		{
			get { return _newchangedPassword; }
			set { _newchangedPassword = value; }
		}
		public string changedLastName
		{
			get { return _changedLastName; }
			set { _changedLastName = value; }
		}
		public string changedUsername
		{
			get { return _changedUsername; }
			set { _changedUsername = value; }
		}
		public string changedFirstName
		{
			get { return _changedFirstName; }
			set { _changedFirstName = value; }
		}

		public void goBack()
		{
			_events.PublishOnUIThread(new goBackMainEvent());
		}

		public void Handle(refreshUIEvent message)
		{
			foreach (Avatar a in avatars)
			{
				a.resetColor();
			}

			int avatarIndex = avatars.IndexOf(avatars.Single(i => i._name == message.fruitSelected));
			_selectedAvatar = message.fruitSelected;
			avatars[avatarIndex].changeColor("Black");
			avatars.Refresh();
			NotifyOfPropertyChange(null);
		}
	}
}
