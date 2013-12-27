using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Nfc;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Droid.Platform;
using Cirrious.CrossCore.Droid.Views;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace DevelopingTrends.MvxPlugins.NFC.Droid
{
    internal class MvxNFCWatcher : MvxNFCReadBase, IMvxNFCWatcher
    {
        private bool _currentlyScanning = false;
        private readonly IMvxMessenger _messenger;
        private readonly object _lock = new object();

        public MvxNFCWatcher(IMvxMessenger messenger)
        {
            _messenger = messenger;
        }


        public bool Start()
        {
            if (!IsSupported)
            {
                if (_dontThrowExpceptionWhenNotSupported)
                {
                    return false;
                }
                throw new NotSupportedException("This device does not support NFC (or perhaps it's disabled)");
            }                  

            StartForegroundMonitoring();

            //attach events
            AttachEvents();            

            return true;
        }
             


        public void Stop()
        {
            lock (_lock)
            {
               
                if (IsSupported)
                {
                    
                    StopForegroundDispatch();
                    DetachEvents();
                    

                }
            }
        }

        

        protected override void NewMessage(NdefLibrary.Ndef.NdefMessage message)
        {
            
            var nfcMessage = new MvxNFCMessageReceived(message, this);
            _messenger.Publish(nfcMessage);
        }
    }
}