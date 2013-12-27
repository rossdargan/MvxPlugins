using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Phone.Tasks;

namespace DevelopingTrends.MvxPlugins.NFC.WindowsPhone
{
    public class MvxNFCReadTask : MvxNFCReadBase, IMvxNFCReadTask
    {        
        public async Task<NdefLibrary.Ndef.NdefMessage> ReadString(CancellationToken cancellationToken, TimeSpan timeout)
        {
            if (!IsSupported)
            {
                if (_dontThrowExpceptionWhenNotSupported)
                {
                    return null;
                }
                throw new NotSupportedException("This device does not support NFC (or perhaps it's disabled)");
            }
            Task timeoutTask=null;
            if (timeout != default(TimeSpan))
            {
                timeoutTask = Task.Delay(timeout);
            }
            long subscription;

            TaskCompletionSource<NdefLibrary.Ndef.NdefMessage> result = new TaskCompletionSource<NdefLibrary.Ndef.NdefMessage>(); //needs a message type

            using (cancellationToken.Register((s => ((TaskCompletionSource<NdefLibrary.Ndef.NdefMessage>)s).TrySetCanceled()), result))
            {
                subscription = _proximityDevice.SubscribeForMessage("NDEF", (sender, message) =>
                {
                    result.SetResult(GetMessage(message));
                });
                try
                {

                    if (timeoutTask != null)
                    {
                        await Task.WhenAny(timeoutTask, result.Task);



                        if (timeoutTask.IsCompleted)
                        {
                            throw new TimeoutException("NFC message not recieved in time");
                        }
                    }
                    if (result.Task.IsCanceled)
                    {
                        return null;
                    }
                    return await result.Task;
                }
                finally
                {
                    _proximityDevice.StopSubscribingForMessage(subscription);
                }
            }
        }




        public Task<NdefLibrary.Ndef.NdefMessage> ReadString(CancellationToken cancellationToken)
        {
            
            return ReadString(cancellationToken,default(TimeSpan));
           
        }
    }
}
