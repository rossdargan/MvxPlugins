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
        private string _scanLastFound;

        public ScanAsMessageViewModel(IMvxMessenger messenger, IMvxNFCWatcher watcher)
        {
            _watcher = watcher;

            _messageToken = messenger.SubscribeOnMainThread<MvxNFCMessageReceived>(MessageRecieved);
        }

        private void MessageRecieved(MvxNFCMessageReceived obj)
        {
            UpdateDisplay(obj.Message);
            ScanLastFound = "Last Message: " + DateTime.Now.ToLocalTime();
        }

        public ICommand ScanCommand
        {
            get
            {
                return new MvxCommand(DoScan);
            }
        }
        public ICommand StopCommand
        {
            get
            {
                return new MvxCommand(DoStop);
            }
        }

        private void DoStop()
        {
            _watcher.Stop();
            IsScanning = false;
        }
  

        private void DoScan()
        {
            try
            {
                IsScanning = true;
                _watcher.Start();

            }
            catch (Exception err)
            {
                Mvx.Error(err.ToString());
            }
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
