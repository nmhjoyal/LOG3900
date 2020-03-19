using Svg;
using Svg.Pathing;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Ink;
using System.Windows.Input;

namespace WPFUI.Potrace
{
    class Potrace
    {
        public static StrokeCollection potrace(string filePath)
        {
            Directory.SetCurrentDirectory(Directory.GetCurrentDirectory() + "/../../Potrace");
            /*
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            process.StandardInput.WriteLine("potrace.exe --svg -a 0 --flat Images/line.bmp");
            process.StandardInput.Flush();
            process.StandardInput.Close();
            process.WaitForExit();
            Console.WriteLine(File.ReadAllText("Images/line.svg"));
            */
            string a = File.ReadAllText("Images/line.svg");
            int startPathIndex = a.IndexOf("d=") + 3;
            int endPathIndex = a.Length - startPathIndex - 16;
            Console.WriteLine(a.Substring(startPathIndex, endPathIndex));
            SvgPathSegmentList test = SvgPathBuilder.Parse(a);
            StrokeCollection strokes = new StrokeCollection();
            for(int i = 0; i < test.Count; i++)
            {
                if(test[i].GetType() == typeof(SvgMoveToSegment)){
                    StylusPointCollection stylusPoints = new StylusPointCollection();
                    stylusPoints.Add(new StylusPoint(test[i].Start.X/15, 150 - test[i].Start.Y/15));
                    strokes.Add(new Stroke(stylusPoints));
                    Console.WriteLine("Move :" + test[i].Start.X + "," + test[i].Start.Y);
                } else if(test[i].GetType() == typeof(SvgLineSegment))
                {
                    Console.WriteLine("Line start :" + test[i].Start.X + "," + test[i].Start.Y);
                    Console.WriteLine("Line end :" + test[i].End.X + "," + test[i].End.Y);
                    strokes[strokes.Count - 1].StylusPoints.Add(new StylusPoint(test[i].End.X/15, 150 - test[i].End.Y/15));
                } else if(test[i].GetType() == typeof(SvgClosePathSegment))
                {
                    Console.WriteLine("Close start :" + test[i].Start.X + "," + test[i].Start.Y);
                    Console.WriteLine("Close end :" + test[i].End.X + "," + test[i].End.Y);
                }
                Console.WriteLine("***");
            }
            return strokes;
        }
    }
}
