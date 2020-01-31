using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class User
    {

        public User(string username, string password)
        {
            _username = username;
            _password = password;
        }

        private string _username;

        public string username
        {
            get { return _username; }
            set { _username = value; }
        }

        private string _password;

        public string password
        {
            get { return _password; }
            set { _password = value; }
        }


    }
}
