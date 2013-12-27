using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopingTrends.MvxPlugins.NFC
{
    public enum FailureReasons
    {
        DidNotFail,
        TagReadOnly,
        TagTooSmall,
        ErrorDuringWrite,
        UnableToFormatTag,
        Unkown,
        TagLostDuringWrite
    }
    public class WriteResult
    {
        public bool DidSucceed
        {
            get
            {
                return NFCTag != null;
            }
        }

        public NFCTag NFCTag { get; set; }

        public FailureReasons ReasonForFailure
        {
            get; set;
        }
        
    }
}
