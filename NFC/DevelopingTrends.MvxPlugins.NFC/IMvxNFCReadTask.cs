using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NdefLibrary.Ndef;

namespace DevelopingTrends.MvxPlugins.NFC
{
    public interface IMvxNFCReadTask : IMvxNFC
    {
        Task<NdefMessage> ReadTag(CancellationToken cancellationToken, TimeSpan timeout);
        Task<NdefMessage> ReadTag(CancellationToken cancellationToken);
        Task<NdefMessage> ReadTag();


    }
}
