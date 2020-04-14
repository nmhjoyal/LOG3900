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
        private ISocketHandler _socketHandler;
       
        public LoginViewModel(IUserData userdata, IEventAggregator events, ISocketHandler socketHandler)
        {
            _userdata = userdata;
            _socketHandler = socketHandler;
            _events = events;
            _events.Subscribe(this);
        }

        public string userName
        {
            get { return _userName; }
            set { _userName = value;
                NotifyOfPropertyChange(() => userName);}
        }

        public void logIn(string password)
        {
            
            if (userName != null & userName != "" & password != null & password != "")
            {
                _userdata.userName = userName;
                _userdata.password = password;
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

        public void Unsubscribe()
        {
            _events.Unsubscribe(this);
        }

    }
}
