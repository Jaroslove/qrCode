using MessagingToolkit.QRCode.Geom;
using System;

namespace MessagingToolkit.QRCode.Helper
{
    public class ConsoleCanvas : DebugCanvas
    {
        public void Print(string str)
        {
            Console.WriteLine(str);
        }

        public void DrawPoint(Point point, int color)
        {
        }

        public void DrawCross(Point point, int color)
        {
        }

        public void DrawPoints(Point[] points, int color)
        {
        }

        public void DrawLine(Line line, int color)
        {
        }

        public void DrawLines(Line[] lines, int color)
        {
        }

        public void DrawPolygon(Point[] points, int color)
        {
        }

        public void DrawMatrix(bool[][] matrix)
        {
        }
    }
}
