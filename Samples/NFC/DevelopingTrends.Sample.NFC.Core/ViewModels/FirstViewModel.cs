using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Input;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.ViewModels;
using DevelopingTrends.MvxPlugins.NFC;

namespace DevelopingTrends.Sample.NFC.Core.ViewModels
{
    public class FirstViewModel 
		: MvxViewModel
    {
        private readonly IMvxMessenger _messenger;
        private readonly IMvxNFCReadTask _nfcReadTask;
        private readonly IMvxNFCWatcher _watcher;

        public FirstViewModel(IMvxMessenger messenger, IMvxNFCReadTask readTask, IMvxNFCWatcher watcher)
        {
            _messenger = messenger;
            _nfcReadTask = readTask;
            _watcher = watcher;

            _messageToken = messenger.Subscribe<MvxNFCMessageReceived>(MessageRecieved);
        }

        private void MessageRecieved(MvxNFCMessageReceived obj)
        {
            Hello = obj.Message;
        }

        private string _hello = "Hello MvvmCross";
        public string Hello
		{ 
			get { return _hello; }
			set { _hello = value; RaisePropertyChanged(() => Hello); }
		}

        public ICommand ScanCommand
        {
            get
            {
                return new MvxCommand(DoScan2);
            }
        }

        private bool isStarted = false;
        private void DoScan2()
        {
            if (!isStarted)
            {
                isStarted = true;
                _watcher.Start();
            }
        }

        private CancellationTokenSource cancellationTokenSource;
        private MvxSubscriptionToken _messageToken;
        private async void DoScan()
        {
            try
            {
                if (cancellationTokenSource != null)
                {
                    cancellationTokenSource.Cancel();
                    cancellationTokenSource = null;
                }
                else
                {


                    cancellationTokenSource = new CancellationTokenSource();
                    Hello = "Scanning...";
                    Hello = await _nfcReadTask.ReadString(cancellationTokenSource.Token, TimeSpan.FromSeconds(30));
                    cancellationTokenSource = null;
                }
            }
            catch (TimeoutException)
            {
                Hello = "Timeout";
            }
            catch (Exception err)
            {
                Hello = err.ToString();
            }
        }
    }
}
