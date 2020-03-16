using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    public class Trait
    {
        public string path;
        public string couleur;
        public string width;
        public bool stylusTip;
        public Trait(string path, string couleur, string width, bool stylusTip)
        {
            this.path = path;
            this.couleur = couleur;
            this.width = width;
            this.stylusTip = stylusTip;
        }
    }
}
