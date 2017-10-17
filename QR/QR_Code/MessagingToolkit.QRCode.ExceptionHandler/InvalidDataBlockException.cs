using System;

namespace MessagingToolkit.QRCode.ExceptionHandler
{
    [Serializable]
    public class InvalidDataBlockException : ArgumentException
    {
        internal string message = (string)null;

        public override string Message
        {
            get
            {
                return this.message;
            }
        }

        public InvalidDataBlockException(string message)
        {
            this.message = message;
        }
    }
}
