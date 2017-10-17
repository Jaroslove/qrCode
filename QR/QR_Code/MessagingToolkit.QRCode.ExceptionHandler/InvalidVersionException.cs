using System;

namespace MessagingToolkit.QRCode.ExceptionHandler
{
    [Serializable]
    public class InvalidVersionException : VersionInformationException
    {
        internal string message;

        public override string Message
        {
            get
            {
                return this.message;
            }
        }

        public InvalidVersionException(string message)
        {
            this.message = message;
        }
    }
}
