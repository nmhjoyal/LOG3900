using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class PrivateProfile
    {
        public string firstname;
        public string lastname;
        public string username;
        public string password;
        public string avatar;
        public PrivateProfile(string username, string firstname, string lastname, string password, string avatar)
        {
            this.username = username;
            this.firstname = firstname;
            this.lastname = lastname;
            this.password = password;
            this.avatar = avatar;
        }

    }
}
