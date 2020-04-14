using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.EventModels
{
    public class appWarningEvent
    {
        private string _warningContent;

        public appWarningEvent(string warningContent)
        {
            _warningContent = warningContent;
        }

        public string warningContent
        {
            get { return _warningContent; }
        }
    }
}
