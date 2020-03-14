using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class Avatar
    {
        public string _source;
        public string _backgroundColor;
        public string _name;

        public Avatar(string source, string name)
        {
            _backgroundColor = "#FF66CC2F";
            _source = source;
            _name = name;
        }

        public string source
        {
            get { return _source; }
        }

        public string backgroundColor
        {
            get { return _backgroundColor; }
        }

        public string name
        {
            get { return _name; }
        }

        public void changeColor(string color)
        {
            _backgroundColor = color;
        }

    }
}
