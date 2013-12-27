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
    public class ScanAsTaskViewModel
        : BaseScanViewModel
    {
        private readonly IMvxNFCReadTask _nfcReadTask;
        private bool _hasTimedOut;

        public bool HasTimedOut
        {
            get { return _hasTimedOut; }
            set
            {
                _hasTimedOut = value;
                RaisePropertyChanged(()=>HasTimedOut);
            }
        }


        public ScanAsTaskViewModel( IMvxNFCReadTask readTask)
        {
            _nfcReadTask = readTask;

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
            _cancellationTokenSource.Cancel();
            IsScanning = false;
        }

        private CancellationTokenSource _cancellationTokenSource;

        private async void DoScan()
        {
            try
            {
                ClearDisplay();
                IsScanning = true;
                HasTimedOut = false;

                _cancellationTokenSource = new CancellationTokenSource();

                var record = await _nfcReadTask.ReadTag(_cancellationTokenSource.Token, TimeSpan.FromSeconds(30));
                UpdateDisplay(record);
                _cancellationTokenSource = null;

            }
            catch (TimeoutException)
            {
                HasTimedOut = true;
            }
            catch (Exception err)
            {
                Mvx.Error(err.ToString());
            }
            IsScanning = false;

        }

    }
}
