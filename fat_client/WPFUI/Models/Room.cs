using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class Room
    {
        string id;
        Message[] messages;
        Dictionary<string, string> avatars;

        public Room(string id,
        Message[] messages,
        Dictionary<string, string> avatars)
        {
            this.id = id;
            this.messages = messages;
            this.avatars = avatars;
        }

        public string roomName
        {
            get { return id; }
            set { id = value; }
        }
    }
}
