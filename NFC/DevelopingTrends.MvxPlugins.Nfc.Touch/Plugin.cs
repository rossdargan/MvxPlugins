using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;


namespace DevelopingTrends.MvxPlugins.NFC.Touch
{
    public class Plugin : IMvxPlugin
    {

        public void Load()
        {
            Mvx.RegisterType<IMvxNFCWatcher, MvxNFCNullImplementation>();
            Mvx.RegisterType<IMvxNFCReadTask, MvxNFCNullImplementation>();
            Mvx.RegisterType<IMvxNFCWriteTask, MvxNFCNullImplementation>();
        }
    }
}