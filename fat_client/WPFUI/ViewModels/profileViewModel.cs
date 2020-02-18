using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.EventModels;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    class profileViewModel
    {
		private string _changedUsername;
		private string _changedFirstName;
		private string _changedLastName;
		private string _newchangedPassword;
		private string _newConfirmedPassword;
		private IUserData _userData;
		private IEventAggregator _events;
		private ISocketHandler _socketHandler;

		public profileViewModel(IUserData userdata, IEventAggregator events, ISocketHandler socketHandler)
		{
			_userData = userdata;
			_events = events;
			_socketHandler = socketHandler;
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
