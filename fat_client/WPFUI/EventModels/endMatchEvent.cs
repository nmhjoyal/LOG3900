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
        private Player[] _players;

        public endMatchEvent(Player[] players)
        {
            _players = players;
        }

        public Player[] players
        {
            get { return _players; }
        }

    }
}
