using System;

namespace MessagingToolkit.QRCode.ExceptionHandler
{
    [Serializable]
    public class AlignmentPatternNotFoundException : ArgumentException
    {
        internal string message = (string)null;

        public override string Message
        {
            get
            {
                return this.message;
            }
        }

        public AlignmentPatternNotFoundException(string message)
        {
            this.message = message;
        }
    }
}
