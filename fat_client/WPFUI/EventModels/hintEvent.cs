using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.EventModels
{
    class hintEvent
    {
        private bool hintEnable;
        public hintEvent(bool hintEnable)
        {
            this.hintEnable = hintEnable;
        }

        public bool HintEnable
        {
            get { return this.hintEnable; }
        }
    }
}
