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
    class LoginViewModel: Screen
    {
        private IEventAggregator _events;
        private IUserData _userdata;
        private string _userName;
        private string _ipAdress;
        private ISocketHandler _socketHandler;
        private string _password;

        public string password
        {
            get { return _password; }
            set { _password = value;
                  NotifyOfPropertyChange(() => password);
            }
        }


        public LoginViewModel(IUserData userdata, IEventAggregator events, ISocketHandler socketHandler)
        {
            _userdata = userdata;
            _socketHandler = socketHandler;
            _events = events;
        }

        public string userName
        {
            get { return _userName; }
            set { _userName = value;
                NotifyOfPropertyChange(() => userName);}
        }

        public void setUserName()
        {
            _userdata.userName = userName;
            _userdata.password = password;
        }

        public void logIn()
        {   
          if (userName != null & userName != "" & password != null & password != "")
            {
                setUserName();
                _socketHandler.connectionAttempt();

            }
            else if (userName == null | userName == "")
            {
                _events.PublishOnUIThread(new appWarningEvent("The username should not be blank "));
            }
            else if (password == null | password == "")
            {
                _events.PublishOnUIThread(new appWarningEvent("The password should not be blank "));
            }
        }

        public void signUp()
        {
            _events.PublishOnUIThread(new signUpEvent());
        }

    }
}
