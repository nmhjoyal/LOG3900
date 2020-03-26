using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class Ranking
    {
        private int _position;

        public int position
        {
            get { return _position; }
            set { _position = value; }
        }

        private string _username;

        public string username
        {
            get { return _username; }
            set { _username = value; }
        }

        public Ranking(string username, int position)
        {
            _username = username;
            _position = position;
        }
    }
}
