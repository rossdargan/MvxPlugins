using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace DevelopingTrends.MvxPlugins.NFC.WindowsPhone
{
    public class Plugin
    : IMvxPlugin          
    {

        public void Load()
        {
            Mvx.RegisterType<IMvxNFCReadTask, MvxNFCReadTask>();

            IMvxMessenger messenger = Mvx.Resolve<IMvxMessenger>();
            Mvx.RegisterSingleton<IMvxNFCWatcher>(() => new MvxNFCWatcher(messenger));

        }
    }
}
