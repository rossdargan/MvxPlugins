using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;
using DevelopingTrends.MvxPlugins.NFC;
using NdefLibrary.Ndef;

namespace DevelopingTrends.Sample.NFC.Core.ViewModels
{
    public class WriteTextViewModel : MvxViewModel
    {
        private readonly IWriteTask _writeTask;
        private string _message;
        private ICommand _writeTagCommand;
        private bool _writingTag;
        private string _result;

        public WriteTextViewModel(DevelopingTrends.MvxPlugins.NFC.IWriteTask writeTask)
        {
            _writeTask = writeTask;
            _writeTagCommand = new MvxCommand(DoWriteTag);
        }

        private async void DoWriteTag()
        {

            WritingTag = true;
            Result = "";
            
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
            else
            {
                switch(result.ReasonForFailure)

                {                    
                    case FailureReasons.TagReadOnly:
                        Result = "The tag was read only";
                        break;
                    case FailureReasons.TagTooSmall:
                        Result = "The tag was too small for this message";

                        break;
                    case FailureReasons.ErrorDuringWrite:
                        Result = "An error occured whislt trying to write the tag";

                        break;
                    case FailureReasons.UnableToFormatTag:
                        Result = "The tag was not formatted. Please format it first";

                        break;
                    case FailureReasons.Unkown:
                        Result = "An unkown error occured";

                        break;
                    case FailureReasons.TagLostDuringWrite:
                        Result = "The tag was removed whilst it was been written to";

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            WritingTag = false;
        }

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value; 
                RaisePropertyChanged(()=>Message);
            }
        }
        public string Result
        {
            get { return _result; }
            set
            {
                _result = value;
                RaisePropertyChanged(() => Result);
            }
        }
        public bool WritingTag
        {
            get { return _writingTag; }
            set { _writingTag = value; RaisePropertyChanged(()=>WritingTag);}
        }

        public ICommand WriteTagCommand
        {
            get
            {
                return _writeTagCommand;
            }
        }

    }
}
