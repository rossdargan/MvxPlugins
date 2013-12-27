using System.Collections.Generic;
using System.Text;
using Android.Content;
using Android.Content.Res;
using Android.Nfc;
using Cirrious.CrossCore.Core;

namespace DevelopingTrends.MvxPlugins.NFC.Droid
{
    public abstract class MvxNFCReadBase : MvxNFCDroidBase
    {             
        protected override void NewIntent(MvxValueEventArgs<Intent> e)
        {
            string id=GetIdFromTag(e.Value);
            var tagAsNdefMessage = e.Value.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);
            if (tagAsNdefMessage != null)
            {

                var tag = tagAsNdefMessage[0] as NdefMessage;
                byte[] message = tag.ToByteArray();
                var ndefMessage = NdefLibrary.Ndef.NdefMessage.FromByteArray(message);

                NewMessage(ndefMessage);
            }
        }


        protected abstract void NewMessage(NdefLibrary.Ndef.NdefMessage ndefMessage);
    }
}