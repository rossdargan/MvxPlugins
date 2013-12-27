using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Cirrious.MvvmCross.Droid.Views;

namespace DevelopingTrends.Sample.NFC.Droid.Views
{
    [Activity(Label = "Write Text To Tag")]

    public class WriteTextView : MvxActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.WriteTextView);

        }
    }
}