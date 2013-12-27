using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NdefLibrary.Ndef;

namespace DevelopingTrends.MvxPlugins.NFC
{
    public interface IReadTask : INFC
    {
        Task<MessageReceived> ReadTag(CancellationToken cancellationToken, TimeSpan timeout);
        Task<MessageReceived> ReadTag(CancellationToken cancellationToken);
        Task<MessageReceived> ReadTag();


    }
}
