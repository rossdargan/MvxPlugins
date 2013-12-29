NFC Plugin
==================

This plugin allows you to read and write NDEF formatted messages to NFC cards from a MVVMCross view model. The project supports writing on Android, Windows Phone and Windows Store devices (that have nfc chips). It does not support WPF, Mac or iOS devices.

Installation
----------------
The easiest way to install the plugin is via NuGet. Just search for NFC, and ensure you have pre-release selected. For more info about plugins take a look at the [guide](https://github.com/MvvmCross/MvvmCross/wiki/MvvmCross-plugins).

Important Information
---------------------
Please note this project should be considered an alpha release on all platforms. I will be shortly using it on a project and I'll update the status once it's been used in the wild.

Reading a Tag
-------------
The project supports either a one off read (as a task), or the ability to read several tags at the same time (using the messenger plugin).

Reading as a task:

	private readonly IReadTask _nfcReadTask;
	public ScanAsTaskViewModel( IReadTask readTask)
	{
		_nfcReadTask = readTask;
	}
	private async void DoScan()
	{
		if(_nfcReadTask.IsSupported)
		{
			var record = await _nfcReadTask.ReadString(CancellationToken.None);
			UpdateDisplay(record);
		}
	}

Reading using the messenger plugin:

	private readonly IWatcher _watcher;
	private MvxSubscriptionToken _messageToken;
	public ScanAsMessageViewModel(IMvxMessenger messenger, IWatcher watcher)
	{
		_watcher = watcher;
		_messageToken = messenger.SubscribeOnMainThread<MessageReceived>(MessageRecieved);
		if(_watcher.IsSupported)
		{
			_watcher.Start();
		}
	}

	private void MessageRecieved(MessageReceived obj)
	{
		UpdateDisplay(obj.Message);
	}

The MessageReceived objects TagId will only be set on an android device as the Microsoft API's do not provide access to this.

The NDEF message contains a series or NdefRecords. You should consult this [project](https://ndef.codeplex.com/) to see how to read all the types of messages that can be recieved.

Writing a tag
-------------

Tags can only be written as a task. To write a task you need to create a NDEF message (again consult this [project](https://ndef.codeplex.com/) to see the types of messages you can create.

	public WriteTextViewModel(DevelopingTrends.MvxPlugins.NFC.IWriteTask writeTask)
	{
		_writeTask = writeTask;
	}
	private async void DoWriteTag()
    {
		NdefLibrary.Ndef.NdefMessage message = new NdefMessage();
		NdefTextRecord record = new NdefTextRecord();
		record.LanguageCode = "en";
		record.TextEncoding = NdefTextRecord.TextEncodingType.Utf8;
		record.Text = Message;
		message.Add(record);

		var result = await _writeTask.WriteTag(message);
		if (result.DidSucceed)
		{
			Result = "Wrote Message to tag with ID " + result.NFCTag.Id;
		}
	}

Again note the tag id on the result object will only be set on an android device. Also note that android provides quite specific error messages (such as tag not formatted, or the tag was removed during a write). Windows devices only tell you when a message was succesfully written. 

Finally you must remember that nfc tags must be formatted with an NDEF record before they can be written to on a windows platform.

Questions/Comments
-------------------
This library was written by Ross Dargan ([@rossdargan](https://twitter.com/rossdargan/)). Feel free to log an issue, or create a pull request.