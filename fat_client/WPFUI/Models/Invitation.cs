using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class Invitation
    {
        public string _id;
        public string _username;

        public Invitation(string id, string username)
        {
            this._id = id;
            this._username = username;
        }

        public string id
        {
            get { return _id; }
        }

        public string username
        {
            get { return _username; }
        }

        public string uid
        {
            get { return _username + " invites you to join " + _id; }
        }
    }
}
