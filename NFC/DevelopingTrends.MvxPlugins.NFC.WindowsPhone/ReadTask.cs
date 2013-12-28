using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Phone.Tasks;

namespace DevelopingTrends.MvxPlugins.NFC.WindowsPhone
{
    public class ReadTask : ReadBase, IReadTask
    {        
        public async Task<MessageReceived> ReadTag(CancellationToken cancellationToken, TimeSpan timeout)
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

            TaskCompletionSource<MessageReceived> result = new TaskCompletionSource<MessageReceived>(); //needs a message type

            using (cancellationToken.Register((s => ((TaskCompletionSource<MessageReceived>)s).TrySetCanceled()), result))
            {
                subscription = _proximityDevice.SubscribeForMessage("NDEF", (sender, message) =>
                {
                    string id=sender.DeviceId;
                    result.TrySetResult(GetMessage(message));
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


        public Task<MessageReceived> ReadTag(CancellationToken cancellationToken)
        {
            
            return ReadTag(cancellationToken,default(TimeSpan));
           
        }
        public Task<MessageReceived> ReadTag()
        {
            return ReadTag(CancellationToken.None, default(TimeSpan));
        }
    }
}
