using System;

namespace MessagingToolkit.QRCode.ExceptionHandler
{
    [Serializable]
    public class DecodingFailedException : ArgumentException
    {
        internal string message = (string)null;

        public override string Message
        {
            get
            {
                return this.message;
            }
        }

        public DecodingFailedException(string message)
        {
            this.message = message;
        }
    }
}
