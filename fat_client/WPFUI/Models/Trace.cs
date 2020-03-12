using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFUI.Models
{
    class Trace
    {
        public Point point;
        public Color color;
        public int width;
       //public bool stylusTip;
        public Trace(Point point, string color, int width/*, bool stylusTip*/)
        {
            this.point = new Point(point);
            this.color = new Color(color);
            this.width = width;
            // this.stylusTip = stylusTip;
        }
    }
    class Point
    {
        public double x;
        public double y;
        
        public Point(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        public Point(Point point)
        {
            this.x = point.x;
            this.y = point.y;
        }
    }

    class Color
    {
        public int r;
        public int g;
        public int b;
        

        public Color(int r, int g, int b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }

        public Color(string hexColor)
        {
            int color = Convert.ToInt32(hexColor.Substring(1, hexColor.Length - 1), 16);
            this.r = (color & 0xff0000) >> 16;
            this.g = (color & 0xff00) >> 8;
            this.b = (color & 0xff);
        }

        public String getHex()
        {
            return "#" + this.r.ToString("X2") + this.g.ToString("X2") + this.b.ToString("X2");
        }
    }

}
