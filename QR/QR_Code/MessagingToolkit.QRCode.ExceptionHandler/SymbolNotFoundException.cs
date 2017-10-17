using System;

namespace MessagingToolkit.QRCode.ExceptionHandler
{
    [Serializable]
    public class SymbolNotFoundException : ArgumentException
    {
        internal string message = (string)null;

        public override string Message
        {
            get
            {
                return this.message;
            }
        }

        public SymbolNotFoundException(string message)
        {
            this.message = message;
        }
    }
}
