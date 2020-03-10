using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    class DrawPoint
    {
        public Position pos;
        public RGB color;
        public string width;
       //public bool stylusTip;
        public DrawPoint(double x , double y, string couleur, string width/*, bool stylusTip*/)
        {
         
            this.pos = new Position(x, y);
            this.color = new RGB(couleur);
            this.width = width;
            // this.stylusTip = stylusTip;
        }
    }
    class Position
    {
        public double x;
        public double y;
        
        public Position(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
    }

    class RGB
    {
        public int r;
        public int g;
        public int b;
        

        public RGB(int r, int g, int b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public RGB(string hexColor)
        {
            this.toRGB(hexColor);
        }

        public void toRGB(string hexColor)
        {
            Console.WriteLine(hexColor);
            int color = Convert.ToInt32(hexColor.Substring(1, hexColor.Length - 1), 16);
            this.r = (color & 0xff0000) >> 16;
            this.g =(color & 0xff00) >> 8;
            this.b = (color & 0xff);

        }
    }

}
