using System.Text;

namespace MessagingToolkit.QRCode.Helper
{
    public class QRCodeHelper
    {
        public static int Sqrt(int val)
        {
            int num1 = 0;
            int num2 = 32768;
            int num3 = 15;
            do
            {
                int num4 = val;
                int num5 = (num1 << 1) + num2;
                int num6 = num3-- & 31;
                int num7;
                int num8 = num7 = num5 << num6;
                if (num4 >= num7)
                {
                    num1 += num2;
                    val -= num8;
                }
            }
            while ((num2 >>= 1) > 0);
            return num1;
        }

        public static bool IsUniCode(string value)
        {
            return QRCodeHelper.FromASCIIByteArray(QRCodeHelper.AsciiStringToByteArray(value)) != QRCodeHelper.FromUnicodeByteArray(QRCodeHelper.UnicodeStringToByteArray(value));
        }

        public static bool IsUnicode(byte[] byteData)
        {
            return (int)QRCodeHelper.AsciiStringToByteArray(QRCodeHelper.FromASCIIByteArray(byteData))[0] != (int)QRCodeHelper.UnicodeStringToByteArray(QRCodeHelper.FromUnicodeByteArray(byteData))[0];
        }

        public static string FromASCIIByteArray(byte[] characters)
        {
            return new ASCIIEncoding().GetString(characters);
        }

        public static string FromUnicodeByteArray(byte[] characters)
        {
            return new UnicodeEncoding().GetString(characters);
        }

        public static byte[] AsciiStringToByteArray(string str)
        {
            return new ASCIIEncoding().GetBytes(str);
        }

        public static byte[] UnicodeStringToByteArray(string str)
        {
            return new UnicodeEncoding().GetBytes(str);
        }
    }
}
