using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class Message
    {

        string username;
        string content;
        int date;
        string roomId;
        public Message(string username,
        string content,
        int date,
        string roomId)
        {
            this.username = username;
            this.content = content;
            this.date = date;
            this.roomId = roomId;
        }

    }
}
