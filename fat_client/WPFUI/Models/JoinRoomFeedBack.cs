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
        public Room joinedRoom;
        public Boolean isPrivate;

        public JoinRoomFeedBack(Feedback fb, Room joinedRoom, Boolean isPrivate)
        {
            this.feedback = fb;
            this.joinedRoom = joinedRoom;
            this.isPrivate = isPrivate;
        }

    }
}
