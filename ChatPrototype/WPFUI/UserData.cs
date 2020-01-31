using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI
{
    public class UserData : IUserData
    {
        private string _userName;
        private string _ipAdress;

        public string userName
        {
            get { return _userName; }
            set { _userName = value; }
        }

        public string ipAdress
        {
            get { return _ipAdress; }
            set { _ipAdress = value; }
        }


        public UserData(string userName, string ipAdress)
        {
            _userName = userName;
            _ipAdress = ipAdress;
        }


    }
}
