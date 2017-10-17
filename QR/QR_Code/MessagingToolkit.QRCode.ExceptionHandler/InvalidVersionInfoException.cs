using System;

namespace MessagingToolkit.QRCode.ExceptionHandler
{
    [Serializable]
    public class InvalidVersionInfoException : VersionInformationException
    {
        internal string message = (string)null;

        public override string Message
        {
            get
            {
                return this.message;
            }
        }

        public InvalidVersionInfoException(string message)
        {
            this.message = message;
        }
    }
}
