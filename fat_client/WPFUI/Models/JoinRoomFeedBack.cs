using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    class JoinRoomFeedBack
    {
        public Feedback feedback;
        public Room room_joined;
        public Boolean isPrivate;

        public JoinRoomFeedBack(Feedback fb, Room room_joined , Boolean isPrivate)
        {
            this.feedback = fb;
            this.room_joined = room_joined;
            this.isPrivate = isPrivate;
        }

    }
}
