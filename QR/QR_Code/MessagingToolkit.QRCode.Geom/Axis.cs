using MessagingToolkit.QRCode.Codec.Reader;

namespace MessagingToolkit.QRCode.Geom
{
    public class Axis
    {
        internal int sin;
        internal int cos;
        internal int modulePitch;
        internal Point origin;

        public virtual Point Origin
        {
            set
            {
                this.origin = value;
            }
        }

        public virtual int ModulePitch
        {
            set
            {
                this.modulePitch = value;
            }
        }

        public Axis(int[] angle, int modulePitch)
        {
            this.sin = angle[0];
            this.cos = angle[1];
            this.modulePitch = modulePitch;
            this.origin = new Point();
        }

        public virtual Point translate(Point offset)
        {
            return this.translate(offset.X, offset.Y);
        }

        public virtual Point translate(Point origin, Point offset)
        {
            this.Origin = origin;
            return this.translate(offset.X, offset.Y);
        }

        public virtual Point translate(Point origin, int moveX, int moveY)
        {
            this.Origin = origin;
            return this.translate(moveX, moveY);
        }

        public virtual Point translate(Point origin, int modulePitch, int moveX, int moveY)
        {
            this.Origin = origin;
            this.modulePitch = modulePitch;
            return this.translate(moveX, moveY);
        }

        public virtual Point translate(int moveX, int moveY)
        {
            long decimalPoint = (long)QRCodeImageReader.DECIMAL_POINT;
            Point point = new Point();
            int num1 = moveX == 0 ? 0 : this.modulePitch * moveX >> (int)decimalPoint;
            int num2 = moveY == 0 ? 0 : this.modulePitch * moveY >> (int)decimalPoint;
            point.Translate(num1 * this.cos - num2 * this.sin >> (int)decimalPoint, num1 * this.sin + num2 * this.cos >> (int)decimalPoint);
            point.Translate(this.origin.X, this.origin.Y);
            return point;
        }
    }
}
