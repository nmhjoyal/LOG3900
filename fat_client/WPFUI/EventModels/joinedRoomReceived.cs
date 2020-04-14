using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPFUI.Models;

namespace WPFUI.EventModels
{
    public class joinedRoomReceived
    {
        public Room[] _joinedRooms;
        public joinedRoomReceived(Room[] joinedRooms)
        {
            _joinedRooms = joinedRooms;
        }
    }
}
