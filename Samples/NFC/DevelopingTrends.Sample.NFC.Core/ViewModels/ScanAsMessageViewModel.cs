using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Input;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using DevelopingTrends.MvxPlugins.NFC;
using NdefLibrary.Ndef;

namespace DevelopingTrends.Sample.NFC.Core.ViewModels
{
    public class ScanAsMessageViewModel
        : BaseScanViewModel
    {
        private readonly IMvxNFCWatcher _watcher;
        private MvxSubscriptionToken _messageToken;
        public ScanAsMessageViewModel(IMvxMessenger messenger, IMvxNFCWatcher watcher)
        {
            _watcher = watcher;
            _messageToken = messenger.SubscribeOnMainThread<MvxNFCMessageReceived>(MessageRecieved);
            _watcher.Start();
        }

        private void MessageRecieved(MvxNFCMessageReceived obj)
        {
            UpdateDisplay(obj.Message);
        }
   
        private void DoScan()
        {
            

        }

        public string ScanLastFound
        {
            get { return _scanLastFound; }
            set
            {
                _scanLastFound = value;
                RaisePropertyChanged(() => ScanLastFound);
                
            }
        }
    }
}
