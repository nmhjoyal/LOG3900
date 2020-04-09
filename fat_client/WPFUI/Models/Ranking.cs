using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class Ranking
    {
        private int _pos;

        public int pos
        {
            get { return _pos; }
            set { _pos = value; }
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

        public Ranking(string username, int pos, int score)
        {
            _username = username;
            _pos = pos;
            _score = score;
        }

        public void set(Ranking ranking)
        {
            this.username = ranking.username;
            this.pos = ranking.pos;
            this.score = ranking.score;
        }
    }
}
