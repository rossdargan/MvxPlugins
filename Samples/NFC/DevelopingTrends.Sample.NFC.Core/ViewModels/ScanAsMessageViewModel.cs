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
        private readonly IWatcher _watcher;
        private MvxSubscriptionToken _messageToken;
        private string _scanLastFound;
        public ScanAsMessageViewModel(IMvxMessenger messenger, IWatcher watcher)
        {
            _watcher = watcher;
            _messageToken = messenger.SubscribeOnMainThread<MessageReceived>(MessageRecieved);
            _watcher.Start();
        }

        private void MessageRecieved(MessageReceived obj)
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
