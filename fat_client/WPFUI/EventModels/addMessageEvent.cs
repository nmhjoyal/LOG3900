using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.EventModels
{
    public class addMessageEvent
    {
        public Models.Message message;
        public addMessageEvent(Models.Message message)
        {
            this.message = message;
        }
    }
}
