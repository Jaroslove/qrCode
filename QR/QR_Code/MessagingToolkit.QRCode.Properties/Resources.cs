using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace MessagingToolkit.QRCode.Properties
{
    [DebuggerNonUserCode]
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [CompilerGenerated]
    internal class Resources
    {
        private static ResourceManager resourceMan;
        private static CultureInfo resourceCulture;

        internal Resources()
        {
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals((object)MessagingToolkit.QRCode.Properties.Resources.resourceMan, (object)null))
                    MessagingToolkit.QRCode.Properties.Resources.resourceMan = new ResourceManager("MessagingToolkit.QRCode.Properties.Resources", typeof(MessagingToolkit.QRCode.Properties.Resources).Assembly);
                return MessagingToolkit.QRCode.Properties.Resources.resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get
            {
                return MessagingToolkit.QRCode.Properties.Resources.resourceCulture;
            }
            set
            {
                MessagingToolkit.QRCode.Properties.Resources.resourceCulture = value;
            }
        }
    }
}
