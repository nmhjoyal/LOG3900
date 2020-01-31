using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.EventModels;

namespace WPFUI.ViewModels
{
    class LoginViewModel: Screen
    {
        private IEventAggregator _events;
        private IUserData _userdata;
        private string _userName;
        private string _ipAdress;

        public LoginViewModel(IUserData userdata, IEventAggregator events)
        {
            _userdata = userdata;
            _events = events;
        }

        public string userName
        {
            get { return _userName; }
            set { _userName = value;
                NotifyOfPropertyChange(() => userName);}
        }


        public string ipAdress
        {
            get { return _ipAdress; }
            set { _ipAdress = value;
                  NotifyOfPropertyChange(() => ipAdress);}
        }

        public void setUserName()
        {
            _userdata.userName = userName;
        }

        public void setIpAdress()
        {
            _userdata.ipAdress = ipAdress;
        }

        public bool loginOk()
        {
            return (userName != null & ipAdress !=null);
        }
        public void logIn()
        {   
            if (loginOk())
            {
                setUserName();
                setIpAdress();
                _events.PublishOnUIThread(new LogInEvent());
            }
        }

    }
}
