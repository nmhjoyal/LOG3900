using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class Rank
    {
        public Rank(string username, int score)
        {
            this._username = username;
            this._score = score;
        }

        private string _username;

        public string username
        {
            get { return _username; }
            set { _username = value; }
        }

        private int _score;

        public int score
        {
            get { return _score; }
            set { _score = value; }
        }

    }
}
