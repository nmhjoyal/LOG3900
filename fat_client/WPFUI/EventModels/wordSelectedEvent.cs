using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.EventModels
{
    public class wordSelectedEvent
    {
        private string _word;
        public wordSelectedEvent(string word)
        {
            _word = word;
        }

        public string word
        {
            get{ return _word; }
        }
    }
}
