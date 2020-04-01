using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.EventModels
{
    public class endTurnRoutineEvent
    {
        dynamic _EndTurnFeedback;
        public endTurnRoutineEvent(dynamic EndTurnFeedBack)
        {
            _EndTurnFeedback = EndTurnFeedBack;
        }
        public dynamic EndTurnFeedBack
        {
            get { return _EndTurnFeedback; }
        }
    }
}
