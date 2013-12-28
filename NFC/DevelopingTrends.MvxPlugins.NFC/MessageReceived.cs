using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace DevelopingTrends.MvxPlugins.NFC
{
    public class MessageReceived : MvxMessage
    {
        /// <summary>
        /// TagID will only be populated on Android as MS does not have apis that allow access to this.
        /// </summary>
        public string TagId { get; set; }
        public NdefLibrary.Ndef.NdefMessage Message { get; set; }


        public MessageReceived(string tagId, NdefLibrary.Ndef.NdefMessage message, object sender)
            : base(sender)
        {
            TagId = tagId;
            Message = message;
        }
    }
}
