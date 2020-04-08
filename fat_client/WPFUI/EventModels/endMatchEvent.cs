using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.Models;

namespace WPFUI.EventModels
{
    public class endMatchEvent
    {
        private List<Player> _players;

        public endMatchEvent(List<Player> players)
        {
            _players = new List<Player>(players);
        }

        public List<Player> players
        {
            get { return _players; }
        }

    }
}
