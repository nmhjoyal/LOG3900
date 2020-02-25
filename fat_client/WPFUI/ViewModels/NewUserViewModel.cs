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
    class NewUserViewModel : Screen
    {
		private string _userName;
		private IUserData _userData;
		private IEventAggregator _events;
		private ISocketHandler _socketHandler;
		public NewUserViewModel(IUserData userdata, IEventAggregator events, ISocketHandler socketHandler)
		{
			_userData = userdata;
			_events = events;
			_socketHandler = socketHandler;
			//_events.PublishOnUIThread(new signUpEvent());
		}
		public string userName
		{
			get { return _userName; }
			set { _userName = value; }
		}
		
		private string _firstName;

		public string firstName
		{
			get { return _firstName; }
			set { _firstName = value; }
		}

		private string _lastName;

		public string lastName
		{
			get { return _lastName; }
			set { _lastName = value; }
		}

		private string _password;

		public string password
		{
			get { return _password; }
			set { _password = value; }
		}

		private string _confirmedPassword;

		public string confirmedPassword
		{
			get { return _confirmedPassword; }
			set { _confirmedPassword = value; }
		}

		private string _avatar;

		public string avatar
		{
			get { return _avatar; }
			set { _avatar = value; }
		}

		public void createUser()
		{
			if (isValid() & isSamePassword())
			{
				_userData.userName = _userName;
				_userData.password = _password;

				_socketHandler.createUser(new PrivateProfile(_userName,_firstName,_lastName,_password,_avatar));
				
			}
		}

		public Boolean isValid()
		{
			if (fieldsAreNotEmpty())
			{
				return true;
			}
			else
			{
				return false;
			}
				
		}

		public Boolean fieldsAreNotEmpty()
		{
			return (_userName != "" & _firstName != "" & _lastName != "" & _password != "" & _confirmedPassword != "");
			
		}

		public Boolean isSamePassword()
		{
			if (_confirmedPassword == _password)
			{
				return true;
			}
			else
			{
				_events.PublishOnUIThread(new passwordMismatchEvent());
				return false;
			}
		}

		public void goBack()
		{
			_events.PublishOnUIThread(new goBackEvent());
		}

	}
}
