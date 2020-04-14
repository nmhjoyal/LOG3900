using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.EventModels
{
    public class buttonsTopMenuEvent
    {
        Boolean _hide;

        public buttonsTopMenuEvent(Boolean hide)
        {
            _hide = hide;
        }

        public Boolean hide
        {
            get { return _hide; }
        }
    }
}
