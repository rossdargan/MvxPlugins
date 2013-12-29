using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Windows.Networking.Proximity;
using Windows.Storage.Streams;
using Cirrious.CrossCore;
using NdefLibrary.Ndef;

namespace DevelopingTrends.MvxPlugins.NFC.WindowsPhone
{
    public class WriteTask : IWriteTask
    {
        protected bool _dontThrowExpceptionWhenNotSupported = true;
        protected ProximityDevice _proximityDevice;
        public WriteTask()
        {
            try
            {
                _proximityDevice = ProximityDevice.GetDefault();

            }
            catch (System.UnauthorizedAccessException)
            {
                //You don't have permission to read NFC
                Mvx.Error("You don't have permission to read NFC. Please update your manifest file");
            }
        }

        public bool IsSupported
        {
            get { return _proximityDevice != null; }
        }

        public Task<WriteResult> WriteTag(NdefMessage message)
        {
            return WriteTag(message, CancellationToken.None);
        }

        public Task<WriteResult> WriteTag(NdefLibrary.Ndef.NdefMessage message, System.Threading.CancellationToken cancellationToken)
        {
            return WriteTag(message, cancellationToken, TimeSpan.FromTicks(0));

        }

        public Task<WriteResult> WriteTag(NdefLibrary.Ndef.NdefMessage message, TimeSpan timeout)
        {
            return WriteTag(message, CancellationToken.None, timeout);
        }

        public async Task<WriteResult> WriteTag(NdefMessage message, CancellationToken cancellationToken, TimeSpan timeout)
        {
            if (!IsSupported)
            {
                if (_dontThrowExpceptionWhenNotSupported)
                {
                    return null;
                }
                throw new NotSupportedException("This device does not support NFC (or perhaps it's disabled)");
            }
            Task timeoutTask = null;
            if (timeout != default(TimeSpan))
            {
                timeoutTask = Task.Delay(timeout);
            }
            long subscription;

            TaskCompletionSource<WriteResult> resultSource = new TaskCompletionSource<WriteResult>(); //needs a message type

            using (cancellationToken.Register((s => ((TaskCompletionSource<WriteResult>)s).TrySetCanceled()), resultSource))
            {
                
                byte[] theMessage=message.ToByteArray();
                subscription = _proximityDevice.PublishBinaryMessage("NDEF:WriteTag", theMessage.AsBuffer(), (sender, id) =>
                {
                    WriteResult result = new WriteResult();
                    result.NFCTag = new NFCTag();
                    result.ReasonForFailure = FailureReasons.DidNotFail;
                    resultSource.TrySetResult(result);
                });                    
                try
                {

                    if (timeoutTask != null)
                    {
                        await Task.WhenAny(timeoutTask, resultSource.Task);



                        if (timeoutTask.IsCompleted)
                        {
                            throw new TimeoutException("NFC message not recieved in time");
                        }
                    }
                    if (resultSource.Task.IsCanceled)
                    {
                        return null;
                    }
                    return await resultSource.Task;
                }
                finally
                {
                    _proximityDevice.StopPublishingMessage(subscription);

                }
            }
        }
    }
}
