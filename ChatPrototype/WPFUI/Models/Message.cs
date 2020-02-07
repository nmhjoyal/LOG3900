using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class Message
    {

        private User _author;

        public User author
        {
            get { return _author; }
            set { _author = value; }
        }

        private string _content;

        public string content
        {
            get { return _content; }
            set { _content = value; }
        }

        public Message(string content, User author)
        {
            _content = content;
            _author = author;

        }

    }
}
