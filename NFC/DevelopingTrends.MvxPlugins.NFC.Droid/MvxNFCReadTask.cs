using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using NdefLibrary.Ndef;

namespace DevelopingTrends.MvxPlugins.NFC.Droid
{
    class MvxNFCReadTask : MvxNFCReadBase, IMvxNFCReadTask
    {
        protected override void NewMessage(NdefMessage message)
        {
            if (_result != null)
            {
                _result.TrySetResult(message);
            }
        }

        private TaskCompletionSource<NdefMessage> _result;

        public async System.Threading.Tasks.Task<NdefMessage> ReadString(System.Threading.CancellationToken cancellationToken, TimeSpan timeout)
        {
            if (!IsSupported)
            {
                if (_dontThrowExpceptionWhenNotSupported)
                {
                    return null;
                }
                throw new NotSupportedException("This device does not support NFC (or perhaps it's disabled)");
            }
            _result = new TaskCompletionSource<NdefMessage>(); //needs a message type
            Task timeoutTask = null;
            if (timeout != default(TimeSpan))
            {
                timeoutTask = Task.Delay(timeout);
            }


            using (cancellationToken.Register((s => ((TaskCompletionSource<NdefMessage>)s).TrySetCanceled()), _result))
            {
               
                try
                {
                    AttachEvents();
                    StartForegroundMonitoring();


                    if (timeoutTask != null)
                    {
                        await Task.WhenAny(timeoutTask, _result.Task);



                        if (timeoutTask.IsCompleted)
                        {
                            throw new TimeoutException("NFC message not recieved in time");
                        }
                    }
                    if (_result.Task.IsCanceled)
                    {
                        StopForegroundDispatch();
                        DetachEvents();
                        return null;
                    }
                    NdefMessage result = await _result.Task;
                    //We don't need to stop the foreground dispatch. Prior to the message been sent the application calls
                    //OnPause which removes the foreground dispatch. By removing the events we prevent it from being added back.
                    //StopForegroundDispatch();
                   
                    DetachEvents();

                    return result;

                }
                finally
                {
                   
                }
            }
        }

        public System.Threading.Tasks.Task<NdefMessage> ReadString(System.Threading.CancellationToken cancellationToken)
        {
            return ReadString(cancellationToken, default(TimeSpan));

        }
    }
}