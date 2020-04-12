using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.EventModels
{
    public class appSuccessEvent
    {
        private string _sucessContent;

        public appSuccessEvent(string message)
        {
            _sucessContent = message;
        }

        public string messageContent
        {
            get { return _sucessContent; }
        }
    }
}
