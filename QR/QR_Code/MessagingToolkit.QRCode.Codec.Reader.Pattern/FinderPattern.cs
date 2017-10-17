using MessagingToolkit.QRCode.ExceptionHandler;
using MessagingToolkit.QRCode.Geom;
using MessagingToolkit.QRCode.Helper;
using System;
using System.Collections;

namespace MessagingToolkit.QRCode.Codec.Reader.Pattern
{
    public class FinderPattern
    {
        internal static readonly int[] VersionInfoBit = new int[34]
        {
      31892,
      34236,
      39577,
      42195,
      48118,
      51042,
      55367,
      58893,
      63784,
      68472,
      70749,
      76311,
      79154,
      84390,
      87683,
      92361,
      96236,
      102084,
      102881,
      110507,
      110734,
      117786,
      119615,
      126325,
      127568,
      133589,
      136944,
      141498,
      145311,
      150283,
      152622,
      158308,
      161089,
      167017
        };
        internal static DebugCanvas canvas = QRCodeDecoder.Canvas;
        public const int UL = 0;
        public const int UR = 1;
        public const int DL = 2;
        internal Point[] center;
        internal int version;
        internal int[] sincos;
        internal int[] width;
        internal int[] moduleSize;

        public virtual int Version
        {
            get
            {
                return this.version;
            }
        }

        public virtual int SqrtNumModules
        {
            get
            {
                return 17 + 4 * this.version;
            }
        }

        public static FinderPattern findFinderPattern(bool[][] image)
        {
            Line[] lineCross = FinderPattern.FindLineCross(FinderPattern.FindLineAcross(image));
            Point[] center;
            try
            {
                center = FinderPattern.GetCenter(lineCross);
            }
            catch (FinderPatternNotFoundException ex)
            {
                throw ex;
            }
            int[] angle = FinderPattern.GetAngle(center);
            Point[] pointArray = FinderPattern.Sort(center, angle);
            int[] width = FinderPattern.GetWidth(image, pointArray, angle);
            int[] moduleSize = new int[3]
            {
        (width[0] << QRCodeImageReader.DECIMAL_POINT) / 7,
        (width[1] << QRCodeImageReader.DECIMAL_POINT) / 7,
        (width[2] << QRCodeImageReader.DECIMAL_POINT) / 7
            };
            int version = FinderPattern.calcRoughVersion(pointArray, width);
            if (version > 6)
            {
                try
                {
                    version = FinderPattern.CalcExactVersion(pointArray, angle, moduleSize, image);
                }
                catch (VersionInformationException ex)
                {
                }
            }
            return new FinderPattern(pointArray, version, angle, width, moduleSize);
        }

        internal FinderPattern(Point[] center, int version, int[] sincos, int[] width, int[] moduleSize)
        {
            this.center = center;
            this.version = version;
            this.sincos = sincos;
            this.width = width;
            this.moduleSize = moduleSize;
        }

        public virtual Point[] GetCenter()
        {
            return this.center;
        }

        public virtual Point GetCenter(int position)
        {
            if (position >= 0 && position <= 2)
                return this.center[position];
            return (Point)null;
        }

        public virtual int GetWidth(int position)
        {
            return this.width[position];
        }

        public virtual int[] GetAngle()
        {
            return this.sincos;
        }

        public virtual int GetModuleSize()
        {
            return this.moduleSize[0];
        }

        public virtual int GetModuleSize(int place)
        {
            return this.moduleSize[place];
        }

        internal static Line[] FindLineAcross(bool[][] image)
        {
            int num1 = 0;
            int num2 = 1;
            int length1 = image.Length;
            int length2 = image[0].Length;
            Point point = new Point();
            ArrayList arrayList = ArrayList.Synchronized(new ArrayList(10));
            int[] buffer = new int[5];
            int pointer = 0;
            int num3 = num1;
            bool flag1 = false;
            while (true)
            {
                bool flag2 = image[point.X][point.Y];
                if (flag2 == flag1)
                {
                    ++buffer[pointer];
                }
                else
                {
                    if (!flag2 && FinderPattern.CheckPattern(buffer, pointer))
                    {
                        int x1;
                        int x2;
                        int y2;
                        int y1;
                        if (num3 == num1)
                        {
                            x1 = point.X;
                            for (int index = 0; index < 5; ++index)
                                x1 -= buffer[index];
                            x2 = point.X - 1;
                            y1 = y2 = point.Y;
                        }
                        else
                        {
                            x1 = x2 = point.X;
                            y1 = point.Y;
                            for (int index = 0; index < 5; ++index)
                                y1 -= buffer[index];
                            y2 = point.Y - 1;
                        }
                        arrayList.Add((object)new Line(x1, y1, x2, y2));
                    }
                    pointer = (pointer + 1) % 5;
                    buffer[pointer] = 1;
                    flag1 = !flag1;
                }
                if (num3 == num1)
                {
                    if (point.X < length1 - 1)
                        point.Translate(1, 0);
                    else if (point.Y < length2 - 1)
                    {
                        point.Set(0, point.Y + 1);
                        buffer = new int[5];
                    }
                    else
                    {
                        point.Set(0, 0);
                        buffer = new int[5];
                        num3 = num2;
                    }
                }
                else if (point.Y < length2 - 1)
                    point.Translate(0, 1);
                else if (point.X < length1 - 1)
                {
                    point.Set(point.X + 1, 0);
                    buffer = new int[5];
                }
                else
                    break;
            }
            Line[] lines = new Line[arrayList.Count];
            for (int index = 0; index < lines.Length; ++index)
                lines[index] = (Line)arrayList[index];
            FinderPattern.canvas.DrawLines(lines, Color_Fields.LIGHTGREEN);
            return lines;
        }

        internal static bool CheckPattern(int[] buffer, int pointer)
        {
            int[] numArray = new int[5] { 1, 1, 3, 1, 1 };
            int num1 = 0;
            for (int index = 0; index < 5; ++index)
                num1 += buffer[index];
            int num2 = (num1 << QRCodeImageReader.DECIMAL_POINT) / 7;
            for (int index = 0; index < 5; ++index)
            {
                int num3 = num2 * numArray[index] - num2 / 2;
                int num4 = num2 * numArray[index] + num2 / 2;
                int num5 = buffer[(pointer + index + 1) % 5] << QRCodeImageReader.DECIMAL_POINT;
                if (num5 < num3 || num5 > num4)
                    return false;
            }
            return true;
        }

        internal static Line[] FindLineCross(Line[] lineAcross)
        {
            ArrayList arrayList1 = ArrayList.Synchronized(new ArrayList(10));
            ArrayList arrayList2 = ArrayList.Synchronized(new ArrayList(10));
            ArrayList arrayList3 = ArrayList.Synchronized(new ArrayList(10));
            for (int index = 0; index < lineAcross.Length; ++index)
                arrayList3.Add((object)lineAcross[index]);
            for (int index1 = 0; index1 < arrayList3.Count - 1; ++index1)
            {
                arrayList2.Clear();
                arrayList2.Add(arrayList3[index1]);
                for (int index2 = index1 + 1; index2 < arrayList3.Count; ++index2)
                {
                    if (Line.IsNeighbor((Line)arrayList2[arrayList2.Count - 1], (Line)arrayList3[index2]))
                    {
                        arrayList2.Add(arrayList3[index2]);
                        Line line = (Line)arrayList2[arrayList2.Count - 1];
                        if (arrayList2.Count * 5 > line.Length && index2 == arrayList3.Count - 1)
                        {
                            arrayList1.Add(arrayList2[arrayList2.Count / 2]);
                            for (int index3 = 0; index3 < arrayList2.Count; ++index3)
                                arrayList3.Remove(arrayList2[index3]);
                        }
                    }
                    else if (FinderPattern.cantNeighbor((Line)arrayList2[arrayList2.Count - 1], (Line)arrayList3[index2]) || index2 == arrayList3.Count - 1)
                    {
                        Line line = (Line)arrayList2[arrayList2.Count - 1];
                        if (arrayList2.Count * 6 > line.Length)
                        {
                            arrayList1.Add(arrayList2[arrayList2.Count / 2]);
                            for (int index3 = 0; index3 < arrayList2.Count; ++index3)
                                arrayList3.Remove(arrayList2[index3]);
                            break;
                        }
                        break;
                    }
                }
            }
            Line[] lineArray = new Line[arrayList1.Count];
            for (int index = 0; index < lineArray.Length; ++index)
                lineArray[index] = (Line)arrayList1[index];
            return lineArray;
        }

        internal static bool cantNeighbor(Line line1, Line line2)
        {
            if (Line.IsCross(line1, line2))
                return true;
            if (line1.Horizontal)
                return Math.Abs(line1.GetP1().Y - line2.GetP1().Y) > 1;
            return Math.Abs(line1.GetP1().X - line2.GetP1().X) > 1;
        }

        internal static int[] GetAngle(Point[] centers)
        {
            Line[] lines = new Line[3];
            for (int index = 0; index < lines.Length; ++index)
                lines[index] = new Line(centers[index], centers[(index + 1) % lines.Length]);
            Line longest = Line.GetLongest(lines);
            Point p1 = new Point();
            for (int index = 0; index < centers.Length; ++index)
            {
                if (!longest.GetP1().Equals(centers[index]) && !longest.GetP2().Equals(centers[index]))
                {
                    p1 = centers[index];
                    break;
                }
            }
            FinderPattern.canvas.Print("originPoint is: " + (object)p1);
            Point point = new Point();
            Point p2 = !(p1.Y <= longest.GetP1().Y & p1.Y <= longest.GetP2().Y) ? (!(p1.X >= longest.GetP1().X & p1.X >= longest.GetP2().X) ? (!(p1.Y >= longest.GetP1().Y & p1.Y >= longest.GetP2().Y) ? (longest.GetP1().Y >= longest.GetP2().Y ? longest.GetP2() : longest.GetP1()) : (longest.GetP1().X >= longest.GetP2().X ? longest.GetP2() : longest.GetP1())) : (longest.GetP1().Y >= longest.GetP2().Y ? longest.GetP1() : longest.GetP2())) : (longest.GetP1().X >= longest.GetP2().X ? longest.GetP1() : longest.GetP2());
            int length = new Line(p1, p2).Length;
            return new int[2]
            {
        (p2.Y - p1.Y << QRCodeImageReader.DECIMAL_POINT) / length,
        (p2.X - p1.X << QRCodeImageReader.DECIMAL_POINT) / length
            };
        }

        internal static Point[] GetCenter(Line[] crossLines)
        {
            ArrayList arrayList = ArrayList.Synchronized(new ArrayList(10));
            for (int index1 = 0; index1 < crossLines.Length - 1; ++index1)
            {
                Line crossLine1 = crossLines[index1];
                for (int index2 = index1 + 1; index2 < crossLines.Length; ++index2)
                {
                    Line crossLine2 = crossLines[index2];
                    if (Line.IsCross(crossLine1, crossLine2))
                    {
                        int x;
                        int y;
                        if (crossLine1.Horizontal)
                        {
                            x = crossLine1.Center.X;
                            y = crossLine2.Center.Y;
                        }
                        else
                        {
                            x = crossLine2.Center.X;
                            y = crossLine1.Center.Y;
                        }
                        arrayList.Add((object)new Point(x, y));
                    }
                }
            }
            Point[] points = new Point[arrayList.Count];
            for (int index = 0; index < points.Length; ++index)
                points[index] = (Point)arrayList[index];
            if (points.Length != 3)
                throw new FinderPatternNotFoundException("Invalid number of Finder Pattern detected");
            FinderPattern.canvas.DrawPolygon(points, Color_Fields.RED);
            return points;
        }

        internal static Point[] Sort(Point[] centers, int[] angle)
        {
            Point[] pointArray = new Point[3];
            switch (FinderPattern.GetURQuadrant(angle))
            {
                case 1:
                    pointArray[1] = FinderPattern.GetPointAtSide(centers, 1, 2);
                    pointArray[2] = FinderPattern.GetPointAtSide(centers, 2, 4);
                    break;
                case 2:
                    pointArray[1] = FinderPattern.GetPointAtSide(centers, 2, 4);
                    pointArray[2] = FinderPattern.GetPointAtSide(centers, 8, 4);
                    break;
                case 3:
                    pointArray[1] = FinderPattern.GetPointAtSide(centers, 4, 8);
                    pointArray[2] = FinderPattern.GetPointAtSide(centers, 1, 8);
                    break;
                case 4:
                    pointArray[1] = FinderPattern.GetPointAtSide(centers, 8, 1);
                    pointArray[2] = FinderPattern.GetPointAtSide(centers, 2, 1);
                    break;
            }
            for (int index = 0; index < centers.Length; ++index)
            {
                if (!centers[index].Equals(pointArray[1]) && !centers[index].Equals(pointArray[2]))
                    pointArray[0] = centers[index];
            }
            return pointArray;
        }

        internal static int GetURQuadrant(int[] angle)
        {
            int num1 = angle[0];
            int num2 = angle[1];
            if (num1 >= 0 && num2 > 0)
                return 1;
            if (num1 > 0 && num2 <= 0)
                return 2;
            if (num1 <= 0 && num2 < 0)
                return 3;
            return num1 < 0 && num2 >= 0 ? 4 : 0;
        }

        internal static Point GetPointAtSide(Point[] points, int side1, int side2)
        {
            Point point1 = new Point();
            Point point2 = new Point(side1 == 1 || side2 == 1 ? 0 : int.MaxValue, side1 == 2 || side2 == 2 ? 0 : int.MaxValue);
            for (int index = 0; index < points.Length; ++index)
            {
                switch (side1)
                {
                    case 1:
                        if (point2.X < points[index].X)
                        {
                            point2 = points[index];
                            break;
                        }
                        if (point2.X == points[index].X)
                        {
                            if (side2 == 2)
                            {
                                if (point2.Y < points[index].Y)
                                    point2 = points[index];
                            }
                            else if (point2.Y > points[index].Y)
                                point2 = points[index];
                            break;
                        }
                        break;
                    case 2:
                        if (point2.Y < points[index].Y)
                        {
                            point2 = points[index];
                            break;
                        }
                        if (point2.Y == points[index].Y)
                        {
                            if (side2 == 1)
                            {
                                if (point2.X < points[index].X)
                                    point2 = points[index];
                            }
                            else if (point2.X > points[index].X)
                                point2 = points[index];
                            break;
                        }
                        break;
                    case 4:
                        if (point2.X > points[index].X)
                        {
                            point2 = points[index];
                            break;
                        }
                        if (point2.X == points[index].X)
                        {
                            if (side2 == 2)
                            {
                                if (point2.Y < points[index].Y)
                                    point2 = points[index];
                            }
                            else if (point2.Y > points[index].Y)
                                point2 = points[index];
                            break;
                        }
                        break;
                    case 8:
                        if (point2.Y > points[index].Y)
                        {
                            point2 = points[index];
                            break;
                        }
                        if (point2.Y == points[index].Y)
                        {
                            if (side2 == 1)
                            {
                                if (point2.X < points[index].X)
                                    point2 = points[index];
                            }
                            else if (point2.X > points[index].X)
                                point2 = points[index];
                            break;
                        }
                        break;
                }
            }
            return point2;
        }

        internal static int[] GetWidth(bool[][] image, Point[] centers, int[] sincos)
        {
            int[] numArray = new int[3];
            for (int index = 0; index < 3; ++index)
            {
                bool flag1 = false;
                int y = centers[index].Y;
                int x1;
                for (x1 = centers[index].X; x1 > 0; --x1)
                {
                    if (image[x1][y] && !image[x1 - 1][y])
                    {
                        if (!flag1)
                            flag1 = true;
                        else
                            break;
                    }
                }
                bool flag2 = false;
                int x2;
                for (x2 = centers[index].X; x2 < image.Length; ++x2)
                {
                    if (image[x2][y] && !image[x2 + 1][y])
                    {
                        if (!flag2)
                            flag2 = true;
                        else
                            break;
                    }
                }
                numArray[index] = x2 - x1 + 1;
            }
            return numArray;
        }

        internal static int calcRoughVersion(Point[] center, int[] width)
        {
            int decimalPoint = QRCodeImageReader.DECIMAL_POINT;
            int num1 = new Line(center[0], center[1]).Length << decimalPoint;
            int num2 = (width[0] + width[1] << decimalPoint) / 14;
            int num3 = (num1 / num2 - 10) / 4;
            if ((num1 / num2 - 10) % 4 >= 2)
                ++num3;
            return num3;
        }

        internal static int CalcExactVersion(Point[] centers, int[] angle, int[] moduleSize, bool[][] image)
        {
            bool[] target = new bool[18];
            Point[] points = new Point[18];
            Axis axis = new Axis(angle, moduleSize[1]);
            axis.Origin = centers[1];
            for (int index1 = 0; index1 < 6; ++index1)
            {
                for (int index2 = 0; index2 < 3; ++index2)
                {
                    Point point = axis.translate(index2 - 7, index1 - 3);
                    target[index2 + index1 * 3] = image[point.X][point.Y];
                    points[index2 + index1 * 3] = point;
                }
            }
            FinderPattern.canvas.DrawPoints(points, Color_Fields.RED);
            int num;
            try
            {
                num = FinderPattern.checkVersionInfo(target);
            }
            catch (InvalidVersionInfoException ex1)
            {
                FinderPattern.canvas.Print("Version info error. now retry with other place one.");
                axis.Origin = centers[2];
                axis.ModulePitch = moduleSize[2];
                for (int index1 = 0; index1 < 6; ++index1)
                {
                    for (int index2 = 0; index2 < 3; ++index2)
                    {
                        Point point = axis.translate(index1 - 3, index2 - 7);
                        target[index2 + index1 * 3] = image[point.X][point.Y];
                        points[index1 + index2 * 3] = point;
                    }
                }
                FinderPattern.canvas.DrawPoints(points, Color_Fields.RED);
                try
                {
                    num = FinderPattern.checkVersionInfo(target);
                }
                catch (VersionInformationException ex2)
                {
                    throw ex2;
                }
            }
            return num;
        }

        internal static int checkVersionInfo(bool[] target)
        {
            int num = 0;
            int index1;
            for (index1 = 0; index1 < FinderPattern.VersionInfoBit.Length; ++index1)
            {
                num = 0;
                for (int index2 = 0; index2 < 18; ++index2)
                {
                    if (target[index2] ^ (FinderPattern.VersionInfoBit[index1] >> index2) % 2 == 1)
                        ++num;
                }
                if (num <= 3)
                    break;
            }
            if (num <= 3)
                return 7 + index1;
            throw new InvalidVersionInfoException("Too many errors in version information");
        }
    }
}
