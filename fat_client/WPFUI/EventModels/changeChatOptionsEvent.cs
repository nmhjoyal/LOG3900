using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.EventModels
{
    public class changeChatOptionsEvent
    {
        Boolean _visible;
        public changeChatOptionsEvent( Boolean visibleOptions)
        {
            _visible = visibleOptions;
        }

        public Boolean visible
        {
            get { return _visible; }
        }
    }
}
