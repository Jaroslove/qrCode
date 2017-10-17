using System;

namespace MessagingToolkit.QRCode.Helper
{
    public sealed class StringHelper
    {
        private static readonly string PlatformDefaultEncoding = "ISO-8859-1";
        private static readonly bool AssumeShiftJis = "SHIFT-JIS".Equals(StringHelper.PlatformDefaultEncoding, StringComparison.InvariantCultureIgnoreCase) || "EUC-JP".Equals(StringHelper.PlatformDefaultEncoding, StringComparison.InvariantCultureIgnoreCase);
        public const string ShiftJis = "SHIFT-JIS";
        public const string GB2312 = "GB2312";
        private const string EucJP = "EUC-JP";
        private const string Utf8 = "UTF-8";
        private const string ISO88591 = "ISO-8859-1";

        private StringHelper()
        {
        }

        public static string GuessEncoding(byte[] bytes)
        {
            if (bytes.Length > 3 && (int)bytes[0] == 239 && (int)bytes[1] == 187 && (int)bytes[2] == 191)
                return "UTF-8";
            int length = bytes.Length;
            bool flag1 = true;
            bool flag2 = true;
            bool flag3 = true;
            int num1 = 0;
            int num2 = 0;
            int num3 = 0;
            bool flag4 = false;
            bool flag5 = false;
            bool flag6 = false;
            for (int index = 0; index < length && (flag1 || flag2 || flag3); ++index)
            {
                int num4 = (int)bytes[index] & (int)byte.MaxValue;
                if (num4 >= 128 && num4 <= 191)
                {
                    if (num1 > 0)
                        --num1;
                }
                else
                {
                    if (num1 > 0)
                        flag3 = false;
                    if (num4 >= 192 && num4 <= 253)
                    {
                        flag5 = true;
                        int num5 = num4;
                        while ((num5 & 64) != 0)
                        {
                            ++num1;
                            num5 <<= 1;
                        }
                    }
                }
                if ((num4 == 194 || num4 == 195) && index < length - 1)
                {
                    int num5 = (int)bytes[index + 1] & (int)byte.MaxValue;
                    if (num5 <= 191 && (num4 == 194 && num5 >= 160 || num4 == 195 && num5 >= 128))
                        flag4 = true;
                }
                if (num4 >= (int)sbyte.MaxValue && num4 <= 159)
                    flag1 = false;
                if (num4 >= 161 && num4 <= 223 && !flag6)
                    ++num3;
                if (!flag6 && (num4 >= 240 && num4 <= (int)byte.MaxValue || num4 == 128 || num4 == 160))
                    flag2 = false;
                if (num4 >= 129 && num4 <= 159 || num4 >= 224 && num4 <= 239)
                {
                    if (flag6)
                    {
                        flag6 = false;
                    }
                    else
                    {
                        flag6 = true;
                        if (index >= bytes.Length - 1)
                        {
                            flag2 = false;
                        }
                        else
                        {
                            int num5 = (int)bytes[index + 1] & (int)byte.MaxValue;
                            if (num5 < 64 || num5 > 252)
                                flag2 = false;
                            else
                                ++num2;
                        }
                    }
                }
                else
                    flag6 = false;
            }
            if (num1 > 0)
                flag3 = false;
            if (flag2 && StringHelper.AssumeShiftJis)
                return "SHIFT-JIS";
            if (flag3 && flag5)
                return "UTF-8";
            if (flag2 && (num2 >= 3 || 20 * num3 > length))
                return "SHIFT-JIS";
            if (!flag4 && flag1)
                return "ISO-8859-1";
            return StringHelper.PlatformDefaultEncoding;
        }
    }
}
