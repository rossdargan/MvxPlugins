using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace DevelopingTrends.MvxPlugins.NFC
{
    public class MvxNFCMessageReceived : MvxMessage
    {
      
        public NdefLibrary.Ndef.NdefMessage Message { get; set; }


        public MvxNFCMessageReceived(NdefLibrary.Ndef.NdefMessage message, object sender)
            : base(sender)
        {
            Message = message;
        }
    }
}
