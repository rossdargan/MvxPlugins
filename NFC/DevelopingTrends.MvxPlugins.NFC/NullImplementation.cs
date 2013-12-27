using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopingTrends.MvxPlugins.NFC
{
    public class NullImplementation : IWriteTask, IReadTask, IWatcher

    {
        public bool Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public bool IsSupported
        {
            get
            {
                return false;
            }


        }

        public Task<WriteResult> WriteTag(NdefLibrary.Ndef.NdefMessage message)
        {
            throw new NotImplementedException();
        }

        public Task<WriteResult> WriteTag(NdefLibrary.Ndef.NdefMessage message, System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<WriteResult> WriteTag(NdefLibrary.Ndef.NdefMessage message, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<WriteResult> WriteTag(NdefLibrary.Ndef.NdefMessage message, System.Threading.CancellationToken cancellationToken, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<MessageReceived> ReadTag(System.Threading.CancellationToken cancellationToken, TimeSpan timeout)
        {
            throw new NotImplementedException();
        }

        public Task<MessageReceived> ReadTag(System.Threading.CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<MessageReceived> ReadTag()
        {
            throw new NotImplementedException();
        }
    }
}
