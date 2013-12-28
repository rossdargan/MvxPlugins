using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Networking.Proximity;
using Cirrious.CrossCore;

namespace DevelopingTrends.MvxPlugins.NFC.WindowsStore
{
    public abstract class ReadBase : INFC
    {
        protected bool _dontThrowExpceptionWhenNotSupported = true;
        protected ProximityDevice _proximityDevice;

        public ReadBase()
        {
            try
            {
                _proximityDevice = ProximityDevice.GetDefault();            

            }
            catch (System.UnauthorizedAccessException)
            {
                //You don't have permission to read NFC
                Mvx.Error("You don't have permission to read NFC. Please update your manifest file");
            }
        }

        public bool IsSupported
        {
            get { return _proximityDevice!=null; }
        }

        protected MessageReceived GetMessage(ProximityMessage message)
        {
            
            var buffer = message.Data.ToArray();
            return new MessageReceived("id!!", NdefLibrary.Ndef.NdefMessage.FromByteArray(buffer),this);
        }
    }
}