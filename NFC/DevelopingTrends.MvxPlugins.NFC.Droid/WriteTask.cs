using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Nfc;
using Android.Nfc.Tech;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Droid.Platform;
using NdefMessage = NdefLibrary.Ndef.NdefMessage;

namespace DevelopingTrends.MvxPlugins.NFC.Droid
{
    public class MvxNFCWriteTask : DroidBase, IWriteTask
    {
        private NdefMessage _messageToWrite;

        protected override async void NewIntent(Cirrious.CrossCore.Core.MvxValueEventArgs<Intent> e)
        {
            WriteResult writeResult = new WriteResult();
            writeResult.ReasonForFailure = FailureReasons.Unkown;


            var id = GetIdFromTag(e.Value);
            var intent = e.Value;
            Tag tag = (Tag) intent.GetParcelableExtra(NfcAdapter.ExtraTag);

            Ndef ndef = Ndef.Get(tag);
            if (ndef != null)
            {
                ndef.Connect();
                if (!ndef.IsWritable)
                {
                    writeResult.ReasonForFailure = FailureReasons.TagReadOnly;
                    _taskCompletionSource.TrySetResult(writeResult);
                    return;
                }
                byte[] message = _messageToWrite.ToByteArray();
                if (ndef.MaxSize < message.Length)
                {
                    writeResult.ReasonForFailure = FailureReasons.TagTooSmall;
                    _taskCompletionSource.TrySetResult(writeResult);
                    return;
                }

                try
                {
                    await ndef.WriteNdefMessageAsync(new Android.Nfc.NdefMessage(message));
                    writeResult.ReasonForFailure = FailureReasons.DidNotFail;
                    writeResult.NFCTag = new NFCTag()
                    {
                        Id = id
                    };
                    _taskCompletionSource.TrySetResult(writeResult);
                    return;
                }
                catch (TagLostException tagLost)
                {
                    writeResult.ReasonForFailure = FailureReasons.TagLostDuringWrite;
                    _taskCompletionSource.TrySetResult(writeResult);
                    return;
                }
                catch (Exception err)
                {
                    Mvx.Error("Error writing Tag: " + err.ToString());
                    writeResult.ReasonForFailure = FailureReasons.ErrorDuringWrite;
                    _taskCompletionSource.TrySetResult(writeResult);
                    return;
                }
            }
            else
            {
                writeResult.ReasonForFailure = FailureReasons.UnableToFormatTag;
                _taskCompletionSource.TrySetResult(writeResult);
                return;
            }


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

        private TaskCompletionSource<WriteResult> _taskCompletionSource=null;
        public async Task<WriteResult> WriteTag(NdefMessage message, CancellationToken cancellationToken, TimeSpan timeout)
        {
            {
                if (!IsSupported)
                {
                    if (_dontThrowExpceptionWhenNotSupported)
                    {
                        return null;
                    }
                    throw new NotSupportedException("This device does not support NFC (or perhaps it's disabled)");
                }

                if (_taskCompletionSource != null)
                {
                    //Mark it as cancelled
                    if (!_taskCompletionSource.Task.IsCompleted)
                    {
                        _taskCompletionSource.TrySetCanceled();
                    }
                }
                _taskCompletionSource = new TaskCompletionSource<WriteResult>();


                Task timeoutTask = null;
                if (timeout != default(TimeSpan))
                {
                    timeoutTask = Task.Delay(timeout);
                }


                using (cancellationToken.Register((s => ((TaskCompletionSource<WriteResult>)s).TrySetCanceled()), _taskCompletionSource))
                {
                    _messageToWrite = message;

                    AttachEvents();
                    StartForegroundMonitoring();


                    if (timeoutTask != null)
                    {
                        await Task.WhenAny(timeoutTask, _taskCompletionSource.Task);



                        if (timeoutTask.IsCompleted)
                        {
                            throw new TimeoutException("NFC message not recieved in time");
                        }
                    }
                    if (_taskCompletionSource.Task.IsCanceled)
                    {
                        StopForegroundDispatch();
                        DetachEvents();
                        return null;
                    }
                    var result = await _taskCompletionSource.Task;
                    //We don't need to stop the foreground dispatch. Prior to the message been sent the application calls
                    //OnPause which removes the foreground dispatch. By removing the events we prevent it from being added back.
                    //StopForegroundDispatch();

                    DetachEvents();

                    return result;
                }
            }
        }
    }
}