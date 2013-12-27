using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace DevelopingTrends.MvxPlugins.NFC.WindowsPhone
{
    public class MvxNFCWatcher : MvxNFCReadBase, IMvxNFCWatcher
    {
        private readonly IMvxMessenger _messenger;
        private readonly object _lock = new object();
        public MvxNFCWatcher(IMvxMessenger messenger)
        {
            _messenger = messenger;
        }

        private long _subscription = -1;
        public bool Start()
        {
            if (!IsSupported)
            {
                if (_dontThrowExpceptionWhenNotSupported)
                {
                    return false;
                }
                throw new NotSupportedException("This device does not support NFC (or perhaps it's disabled)");
            }
            if (_subscription != -1)
            {
                //Remove the current subscription
                Stop();
            }
            _subscription = _proximityDevice.SubscribeForMessage("NDEF", (sender, message) =>
            {
                var nfcMessage = new MvxNFCMessageReceived(GetMessage(message),this);
                _messenger.Publish(nfcMessage);
            });

            return true;
        }

        public void Stop()
        {
            lock (_lock)
            {
                if (_subscription != -1)
                {
                    _proximityDevice.StopSubscribingForMessage(_subscription);
                    _subscription = -1;
                }
            }
        }
    }
}
