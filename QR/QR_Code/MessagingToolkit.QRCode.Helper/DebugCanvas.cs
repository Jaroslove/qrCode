using MessagingToolkit.QRCode.Geom;

namespace MessagingToolkit.QRCode.Helper
{
    public interface DebugCanvas
    {
        void Print(string str);

        void DrawPoint(Point point, int color);

        void DrawCross(Point point, int color);

        void DrawPoints(Point[] points, int color);

        void DrawLine(Line line, int color);

        void DrawLines(Line[] lines, int color);

        void DrawPolygon(Point[] points, int color);

        void DrawMatrix(bool[][] matrix);
    }
}
