using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.EventModels
{
    public class guessResponseEvent
    {
        public Boolean _isGoodGuess;
        public guessResponseEvent(Boolean isGoodGuess)
        {
            _isGoodGuess = isGoodGuess;
        }
    }
}
