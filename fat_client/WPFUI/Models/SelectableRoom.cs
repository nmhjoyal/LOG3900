using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class SelectableRoom
    {
        private Room _room;
        private string _backgroundColor;

        public SelectableRoom(Room room)
        {
            _room = room;
            _backgroundColor = "#FF66CC2F";
        }

        public string id
        {
            get { return _room.id; }
        }

        public string backgroundColor
        {
            get { return _backgroundColor; }
        }

        public void changeColor(string color)
        {
            _backgroundColor = color;
        }

        public void resetColor()
        {
            _backgroundColor = "#FF66CC2F";
        }
    }
}
