namespace MessagingToolkit.QRCode.Helper
{
    public class ContentConverter
    {
        internal static char n = '\n';

        public static string Convert(string targetString)
        {
            if (targetString == null)
                return targetString;
            if (targetString.IndexOf("MEBKM:") > -1)
                targetString = ContentConverter.ConvertDocomoBookmark(targetString);
            if (targetString.IndexOf("MECARD:") > -1)
                targetString = ContentConverter.ConvertDocomoAddressBook(targetString);
            if (targetString.IndexOf("MATMSG:") > -1)
                targetString = ContentConverter.ConvertDocomoMailto(targetString);
            if (targetString.IndexOf("http\\://") > -1)
                targetString = ContentConverter.ReplaceString(targetString, "http\\://", "\nhttp://");
            return targetString;
        }

        private static string ConvertDocomoBookmark(string targetString)
        {
            targetString = ContentConverter.RemoveString(targetString, "MEBKM:");
            targetString = ContentConverter.RemoveString(targetString, "TITLE:");
            targetString = ContentConverter.RemoveString(targetString, ";");
            targetString = ContentConverter.RemoveString(targetString, "URL:");
            return targetString;
        }

        private static string ConvertDocomoAddressBook(string targetString)
        {
            targetString = ContentConverter.RemoveString(targetString, "MECARD:");
            targetString = ContentConverter.RemoveString(targetString, ";");
            targetString = ContentConverter.ReplaceString(targetString, "N:", "NAME1:");
            targetString = ContentConverter.ReplaceString(targetString, "SOUND:", ((int)ContentConverter.n).ToString() + "NAME2:");
            targetString = ContentConverter.ReplaceString(targetString, "TEL:", ((int)ContentConverter.n).ToString() + "TEL1:");
            targetString = ContentConverter.ReplaceString(targetString, "EMAIL:", ((int)ContentConverter.n).ToString() + "MAIL1:");
            targetString += (string)(object)ContentConverter.n;
            return targetString;
        }

        private static string ConvertDocomoMailto(string s)
        {
            string s1 = s;
            char ch = '\n';
            return ContentConverter.ReplaceString(ContentConverter.ReplaceString(ContentConverter.ReplaceString(ContentConverter.RemoveString(ContentConverter.RemoveString(s1, "MATMSG:"), ";"), "TO:", "MAILTO:"), "SUB:", ((int)ch).ToString() + "SUBJECT:"), "BODY:", ((int)ch).ToString() + "BODY:") + (object)ch;
        }

        private static string ReplaceString(string s, string s1, string s2)
        {
            string str = s;
            for (int length = str.IndexOf(s1, 0); length > -1; length = str.IndexOf(s1, length + s2.Length))
                str = str.Substring(0, length) + s2 + str.Substring(length + s1.Length);
            return str;
        }

        private static string RemoveString(string s, string s1)
        {
            return ContentConverter.ReplaceString(s, s1, "");
        }
    }
}
