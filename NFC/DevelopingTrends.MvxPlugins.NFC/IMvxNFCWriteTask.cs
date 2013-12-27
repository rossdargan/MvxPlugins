using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopingTrends.MvxPlugins.NFC
{
    public interface IMvxNFCWriteTask : IMvxNFC
    {
        Task<MvxNfcWriteResult> WriteTag(NdefLibrary.Ndef.NdefMessage message);
        Task<MvxNfcWriteResult> WriteTag(NdefLibrary.Ndef.NdefMessage message, CancellationToken cancellationToken);
        Task<MvxNfcWriteResult> WriteTag(NdefLibrary.Ndef.NdefMessage message, TimeSpan timeout);
        Task<MvxNfcWriteResult> WriteTag(NdefLibrary.Ndef.NdefMessage message, CancellationToken cancellationToken, TimeSpan timeout);
    }
}
