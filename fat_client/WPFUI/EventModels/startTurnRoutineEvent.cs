using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.EventModels
{
    public class startTurnRoutineEvent
    {
        private int _turnTime;
        public startTurnRoutineEvent(int time)
        {
            _turnTime = time;
        }

        public int turnTime
        {
            get { return _turnTime; }
        }
        
    }
}

