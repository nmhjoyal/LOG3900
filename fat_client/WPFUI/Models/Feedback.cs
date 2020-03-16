using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    class Feedback
    {
        public Boolean status;
        public string log_message;
        public Feedback(Boolean status, string log_message)
        {
            this.status = status;
            this.log_message = log_message;
        }
    }
    
}
