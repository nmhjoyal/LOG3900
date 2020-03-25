using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.EventModels
{
    public class refreshAvailableRoomsEvent
    {
        private string _selectedAvailableRoomId;

        public refreshAvailableRoomsEvent( string selectedAvailableRoomId)
        {
            _selectedAvailableRoomId = selectedAvailableRoomId;
        }

        public string selectedAvailableRoomId
        {
            get { return _selectedAvailableRoomId; }
        }
    }
}
