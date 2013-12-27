This plugin allows you to easily read NFC tags in a clean cross platform way using MVVM Cross.

Usage example
-------------------------------------------------------------------------------

** Reading a tag as an Async Method

        private readonly IMvxNFCReadTask _nfcReadTask;
        public ScanAsTaskViewModel( IMvxNFCReadTask readTask)
        {
            _nfcReadTask = readTask;
        }
        private async void DoScan()
        {
            var record = await _nfcReadTask.ReadString(CancellationToken.None);
            UpdateDisplay(record);            
        }


** Reading a tag as a message

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


Installation
-------------------------------------------------------------------------------
For now you will have to download the package and compile yourself. I will be realeasing it to nugett once I have added the ability to read on Windows 8, and added a dummy implementation for iOS.

Version History
-------------------------------------------------------------------------------
--Alpha release!


Status & Roadmap
-------------------------------------------------------------------------------
This is a alpha project, not currently in use anywhere (this will change soon).

V2 will include the ability to write tags.


Related Information
-------------------------------------------------------------------------------
The NdefLibrary in this project: https://ndef.codeplex.com/. Currently there isn't a nuget version of the project that will work with Xamarin, so I have had to include the library.