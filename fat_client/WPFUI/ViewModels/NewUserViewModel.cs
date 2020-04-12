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
	class NewUserViewModel : Screen
	{
		private string _userName;
		private IUserData _userData;
		private IEventAggregator _events;
		private ISocketHandler _socketHandler;
		private BindableCollection<Avatar> _avatars;
		private int _selectedAvatarIndex;
		private int nbAvatars;

		public NewUserViewModel(IUserData userdata, IEventAggregator events, ISocketHandler socketHandler)
		{
			_userData = userdata;
			_events = events;
			_events.Subscribe(this);
			_socketHandler = socketHandler;
			_avatars = new BindableCollection<Avatar>();
			fillAvatars();
			_selectedAvatarIndex = 0;
			nbAvatars = avatars.Count();
			//_events.PublishOnUIThread(new signUpEvent());
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
			set { _avatars = value;
				  NotifyOfPropertyChange(() => avatars); }
		}

		public string selectedAvatarSource
		{
			get { return _avatars[_selectedAvatarIndex].source; }
		}

		public string selectedAvatarName
		{
			get { return _avatars[_selectedAvatarIndex].name; }
		}

		public int selectedAvatarIndex
		{
			get { return _selectedAvatarIndex; }
			set
			{
				_selectedAvatarIndex = value;
				NotifyOfPropertyChange(() => selectedAvatarIndex);
				NotifyOfPropertyChange(() => selectedAvatarSource);
			}
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

		public void createUser()
		{
			if (isValid())
			{
				_userData.userName = _userName;
				_userData.password = _password;

				object obj = _socketHandler.createUser(new PrivateProfile(_userName, _firstName, _lastName, _password, selectedAvatarName));
				Feedback fb = JsonConvert.DeserializeObject<Feedback>(obj.ToString());

				if (fb.status)
				{
					_events.PublishOnUIThread(new appSuccessEvent(fb.log_message));
					_events.PublishOnUIThread(new goBackEvent());
				}
				else
				{
					_events.PublishOnUIThread(new appWarningEvent(fb.log_message));
					//print error
				}
			} else
			{
				// print donnes invalides
				_events.PublishOnUIThread(new appWarningEvent("Please make sure no fields are empty and that your passwords match"));
			}
		}

		public Boolean isValid()
		{
			if (fieldsAreNotEmpty() & isSamePassword())
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
				return false;
			}
		}

		public void goBack()
		{
			_events.PublishOnUIThread(new goBackEvent());
		}


		public void leftArrowAvatarChange()
		{
			if (selectedAvatarIndex >= 0 & selectedAvatarIndex < nbAvatars - 1)
			{
				selectedAvatarIndex++;
			}
			else if (selectedAvatarIndex == nbAvatars - 1)
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
	}
}
