using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.Models;

namespace WPFUI.ViewModels
{
    class modifyProfileViewModel: Screen
    {
        private IUserData _userdata;
        private IEventAggregator _events;
        private ISocketHandler _socketHandler;
        private string _newUserName;
        private string _newFirstName;
        private string _newLastName;
        private string _oldPassword;
        private string _newPassword;
        private string _confirmedNewPassword;

        public modifyProfileViewModel(IUserData userdata, IEventAggregator events, ISocketHandler socketHandler)
        {
            _userdata = userdata;
            _events = events;
            _socketHandler = socketHandler;

        }

        public void saveChanges()
        {
            //Server stuff
        }
        public string confirmedNewPassword
        {
            get { return _confirmedNewPassword; }
            set { _confirmedNewPassword = value; }
        }

        public string newPassword
        {
            get { return _newPassword; }
            set { _newPassword = value; }
        }

        public string oldPassword
        {
            get { return _oldPassword; }
            set { _oldPassword = value; }
        }

        public string MyProperty
        {
            get { return _oldPassword; }
            set { _oldPassword = value; }
        }

        public string newLastName
        {
            get { return _newLastName; }
            set { _newLastName = value; }
        }

        public string newFirstName
        {
            get { return _newFirstName; }
            set { _newFirstName = value; }
        }

        public string newUserName
        {
            get { return _newUserName; }
            set { _newUserName = value; }
        }

    }
}
