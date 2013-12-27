using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DevelopingTrends.MvxPlugins.NFC
{
    public interface IWriteTask : INFC
    {
        Task<WriteResult> WriteTag(NdefLibrary.Ndef.NdefMessage message);
        Task<WriteResult> WriteTag(NdefLibrary.Ndef.NdefMessage message, CancellationToken cancellationToken);
        Task<WriteResult> WriteTag(NdefLibrary.Ndef.NdefMessage message, TimeSpan timeout);
        Task<WriteResult> WriteTag(NdefLibrary.Ndef.NdefMessage message, CancellationToken cancellationToken, TimeSpan timeout);
    }
}
