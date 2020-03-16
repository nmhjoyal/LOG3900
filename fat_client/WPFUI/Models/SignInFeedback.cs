using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    class SignInFeedback
    {
        public Feedback feedback;
        public Room[] rooms_joined;
        public SignInFeedback(Feedback feedback, Room[] rooms_joined)
        {
            this.feedback = feedback;
            this.rooms_joined = rooms_joined;
        }
    }

}
