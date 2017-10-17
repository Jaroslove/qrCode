using System;

namespace MessagingToolkit.QRCode.ExceptionHandler
{
    [Serializable]
    public class FinderPatternNotFoundException : Exception
    {
        internal string message = (string)null;

        public override string Message
        {
            get
            {
                return this.message;
            }
        }

        public FinderPatternNotFoundException(string message)
        {
            this.message = message;
        }
    }
}
