using Caliburn.Micro;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
		private string _newConfirmedPassword;
		private IUserData _userData;
		private IEventAggregator _events;
		private ISocketHandler _socketHandler;
		private StatsClient statsClient;

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
			Console.WriteLine("CONNECTIONS STATS CLIENT:"+this.statsClient.Connections[0].ToString());
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
	}
}
