using System;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace DevelopingTrends.MvxPlugins.NFC.WindowsStore
{
    public class Watcher : ReadBase, IWatcher
    {
        private readonly IMvxMessenger _messenger;
        private readonly object _lock = new object();
        public Watcher(IMvxMessenger messenger)
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
                
                var nfcMessage = GetMessage(message);
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
