using System.Drawing;

namespace MessagingToolkit.QRCode.Codec.Data
{
    public class QRCodeBitmapImage : QRCodeImage
    {
        private Bitmap image;

        public QRCodeBitmapImage(Bitmap image)
        {
            this.image = image;
        }

        public virtual int Width
        {
            get
            {
                return this.image.Width;
            }
        }

        public virtual int Height
        {
            get
            {
                return this.image.Height;
            }
        }

        public virtual int GetPixel(int x, int y)
        {
            return this.image.GetPixel(x, y).ToArgb();
        }
    }
}
