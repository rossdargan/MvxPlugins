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
            Mvx.RegisterType<IWriteTask, WriteTask>();
            
            Mvx.RegisterType<IReadTask, ReadTask>();

            IMvxMessenger messenger = Mvx.Resolve<IMvxMessenger>();
            Mvx.RegisterSingleton<IWatcher>(() => new Watcher(messenger));
            
        }
    }
}
