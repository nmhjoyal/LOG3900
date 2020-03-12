using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class ClientMessage
    {
        public string content;
        public string roomId;

        public ClientMessage(string content, string roomId)
        {
            this.content = content;
            this.roomId = roomId;
        }
    }
}
