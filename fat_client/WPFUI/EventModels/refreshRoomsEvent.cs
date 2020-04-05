using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.EventModels
{
    public class refreshRoomsEvent
    {
        private string _selectedRoomId;
        private Boolean _joined;

        public refreshRoomsEvent( string selectedRoomId, Boolean joined)
        {
            _selectedRoomId = selectedRoomId;
            _joined = joined;
        }

        public string selectedRoomId
        {
            get { return _selectedRoomId; }
        }

        public Boolean joined
        {
            get { return _joined; }
        }
    }
}
