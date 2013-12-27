using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Plugins;
using Cirrious.MvvmCross.Plugins.Messenger;

namespace DevelopingTrends.MvxPlugins.NFC.Droid
{
    public class Plugin : IMvxPlugin
    {

        public void Load()
        {
            Mvx.RegisterType<IMvxNFCWatcher, MvxNFCWatcher>();
            Mvx.RegisterType<IMvxNFCReadTask, MvxNFCReadTask>();
            Mvx.RegisterType<IMvxNFCWriteTask,MvxNFCWriteTask>();
        }
    }
}