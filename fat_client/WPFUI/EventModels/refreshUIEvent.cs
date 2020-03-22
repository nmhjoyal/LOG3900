using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.EventModels
{
    public class refreshUIEvent
    {
        string _fruitSelected;
        public refreshUIEvent(string fruitSelected)
        {
            _fruitSelected = fruitSelected;
        }

        public string fruitSelected
        {
            get { return _fruitSelected; }
        }
    }
}
