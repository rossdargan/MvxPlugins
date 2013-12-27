using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevelopingTrends.MvxPlugins.NFC
{
    public interface IMvxNFCWatcher : IMvxNFC
    {
        /// <summary>
        /// Subscribe to the MVX message queue. MvxNFCMessageReceived will be send when a new NFC tag is read.
        /// </summary>
        bool Start();
        void Stop();        
    }
}
