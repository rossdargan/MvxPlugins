using System;
using Android.App;
using Android.Content;
using Android.Nfc;
using Android.OS;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Droid.Views;

namespace DevelopingTrends.Sample.NFC.Droid.Views
{
    [Activity(Label = "Scan As Task")]
    public class ScanAsTaskView : MvxActivity
    {

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.ScanAsTaskView);


        }

        
    }
}