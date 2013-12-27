using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Networking.Proximity;
using Cirrious.CrossCore;

namespace DevelopingTrends.MvxPlugins.NFC.WindowsPhone
{
    public abstract class MvxNFCReadBase : IMvxNFC
    {
        protected bool _dontThrowExpceptionWhenNotSupported = true;
        protected ProximityDevice _proximityDevice;

        public MvxNFCReadBase()
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

        protected NdefLibrary.Ndef.NdefMessage GetMessage(ProximityMessage message)
        {
            
            var buffer = message.Data.ToArray();
            return NdefLibrary.Ndef.NdefMessage.FromByteArray(buffer);
        }
    }
}