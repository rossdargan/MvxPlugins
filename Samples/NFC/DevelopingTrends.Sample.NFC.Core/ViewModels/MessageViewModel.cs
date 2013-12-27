using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cirrious.MvvmCross.ViewModels;
using NdefLibrary.Ndef;

namespace DevelopingTrends.Sample.NFC.Core.ViewModels
{
    public class MessageViewModel : MvxViewModel
    {

        public MessageViewModel(NdefLibrary.Ndef.NdefRecord message)
        {
            Type type = message.CheckSpecializedType(true);
            Type = type.Name;
            Info = "Feel free to add this mapping to the message view model";
            if (type == typeof (NdefLibrary.Ndef.NdefTextRecord))
            {
                var text = new NdefTextRecord(message);
                Info = string.Format("The message on the tag is \"{0}\". The language is \"{1}\"", text.Text,
                    text.LanguageCode);                
            }
            if (type == typeof(NdefLibrary.Ndef.NdefUriRecord))
            {
                var text = new NdefUriRecord(message);
                Info = string.Format("The URI on the tag is \"{0}\"", text.Uri);
            }
        }

        public string Type { get; set; }
        public string Info { get; set; }

    }
}
