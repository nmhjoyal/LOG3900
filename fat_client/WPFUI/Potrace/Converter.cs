using Svg;
using Svg.Pathing;
using Svg.Transforms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Ink;
using System.Windows.Input;
using System.Drawing;

namespace WPFUI.Potrace
{
    class Converter
    {
        private const string TRANSFORM_KEY = "transform=\"";
        private const string PATH_KEY = "d=\"";
        private const string JPG = ".jpg";
        private const string PNG = ".png";
        private const string BMP = ".bmp";
        private const string SVG = ".svg";
        private static Boolean isPotraceDirectory = false;
        public static StrokeCollection exec(string fileName, int width, int height)
        {
            if(!isPotraceDirectory)
            {
                Directory.SetCurrentDirectory(Directory.GetCurrentDirectory() + "/../../Potrace");
                isPotraceDirectory = true;
            }
            Process process = new Process();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            // Console.WriteLine("potrace.exe --svg -a 0 --flat -P " + width + "ptx" + height + "pt Images/" + fileName);
            process.StandardInput.WriteLine("mkbitmap.exe -o Images/mkbitmap-o/" + fileName.Substring(0, fileName.Length - 4) + BMP + " Images/" + fileName);
            process.StandardInput.Flush();
            process.StandardInput.WriteLine("potrace.exe --" + SVG + " -o Images/potrace-o/" + fileName.Substring(0, fileName.Length - 4) + SVG + " -a 0 --flat -W " + width + "pt -H " + height + "pt Images/mkbitmap-o/" + fileName.Substring(0, fileName.Length - 4) + BMP);
            process.StandardInput.Flush();
            process.StandardInput.Close();
            process.WaitForExit();
            string file = File.ReadAllText("Images/potrace-o/" + fileName.Substring(0, fileName.Length - 4) + SVG);
            SvgTransformCollection transforms = (SvgTransformCollection)new SvgTransformConverter().ConvertFrom(file.Substring(file.IndexOf(TRANSFORM_KEY) + TRANSFORM_KEY.Length));
            SvgPathSegmentList pathSegments = (SvgPathSegmentList)new SvgPathBuilder().ConvertFrom(file.Substring(file.IndexOf(PATH_KEY) + PATH_KEY.Length));

            PointF translate = new PointF(0, 0);
            PointF scale = new PointF(1, 1);

            for (int i = 0; i < transforms.Count; i++)
            {
                if(transforms[i].GetType() == typeof(SvgTranslate))
                {
                    translate = new PointF(((SvgTranslate)transforms[i]).X, ((SvgTranslate)transforms[i]).Y);
                } else if(transforms[i].GetType() == typeof(SvgScale))
                {
                    scale = new PointF(((SvgScale)transforms[i]).X, ((SvgScale)transforms[i]).Y);
                }
            }

            StrokeCollection strokes = new StrokeCollection();
            for(int i = 0; i < pathSegments.Count; i++)
            {
                if(pathSegments[i].GetType() == typeof(SvgMoveToSegment)){
                    StylusPointCollection stylusPoints = new StylusPointCollection();
                    stylusPoints.Add(new StylusPoint(scale.X * pathSegments[i].Start.X + translate.X, scale.Y * pathSegments[i].Start.Y + translate.Y));
                    strokes.Add(new Stroke(stylusPoints));
                } else if(pathSegments[i].GetType() == typeof(SvgLineSegment))
                {
                    strokes[strokes.Count - 1].StylusPoints.Add(new StylusPoint(scale.X * pathSegments[i].End.X + translate.X, scale.Y * pathSegments[i].End.Y + translate.Y));
                }
            }
            return strokes;
        }
    }
}
