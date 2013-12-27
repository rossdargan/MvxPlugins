using System;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Nfc;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Droid.Platform;
using Cirrious.CrossCore.Droid.Views;

namespace DevelopingTrends.MvxPlugins.NFC.Droid
{
    public abstract class DroidBase : INFC
    {
        protected bool _haveManifestPermission = false;
        protected bool _dontThrowExpceptionWhenNotSupported = true;
        protected NfcAdapter _nfcAdapter;

        protected DroidBase()
        {
            _nfcAdapter = NfcAdapter.GetDefaultAdapter(TopActivity);
            if (_nfcAdapter != null)
            {
                string permission = "android.permission.NFC";
                var res = TopActivity.CheckCallingOrSelfPermission(permission);
                if (res == Permission.Denied)
                {
                    Mvx.Error("You don't have permission to read NFC. Please update your manifest file");

                }
                else
                {
                    _haveManifestPermission = true;
                }
            }

        }

        protected Activity TopActivity
        {
            get { return Mvx.Resolve<IMvxAndroidCurrentTopActivity>().Activity; }
        }

        public bool IsSupported
        {
            get
            {
                return _nfcAdapter != null && _nfcAdapter.IsEnabled && _haveManifestPermission; 
            }
        }

        private void droidEvents_ResumeCalled(object sender, EventArgs e)
        {
            StartForegroundMonitoring();
        }

        /// <summary>
        /// Start foreground listening for NFC messages
        /// </summary>
        protected void StartForegroundMonitoring()
        {
            var topActivity = TopActivity;
            if (_nfcAdapter == null)
            {
                _nfcAdapter = NfcAdapter.GetDefaultAdapter(topActivity);
            }
            Type activityType = topActivity.GetType();
            Mvx.Trace(activityType.Name);
            var intent = new Intent(topActivity, activityType).AddFlags(ActivityFlags.SingleTop);
            var pendingIntent = PendingIntent.GetActivity(topActivity, 0, intent, 0);

            var tagDetected = new IntentFilter(NfcAdapter.ActionTagDiscovered);
            var filters = new[] { tagDetected };
            _nfcAdapter.EnableForegroundDispatch(topActivity, pendingIntent, filters, null);
        }

        /// <summary>
        /// Attach the events to the top activity so we can detect nfc reads, and the view's lifecycle
        /// </summary>
        protected void AttachEvents()
        {
            MvxEventSourceActivity droidEvents = TopActivity as MvxEventSourceActivity;
            if (droidEvents != null)
            {
                droidEvents.NewIntentCalled += droidEvents_NewIntentCalled;
                droidEvents.PauseCalled += droidEvents_PauseCalled;
                droidEvents.ResumeCalled += droidEvents_ResumeCalled;
            }
        }

        /// <summary>
        /// Remove any attached events to free memory
        /// </summary>
        protected void DetachEvents()
        {
            MvxEventSourceActivity droidEvents = TopActivity as MvxEventSourceActivity;

            if (droidEvents != null)
            {
                droidEvents.NewIntentCalled -= droidEvents_NewIntentCalled;
                droidEvents.PauseCalled -= droidEvents_PauseCalled;
                droidEvents.ResumeCalled -= droidEvents_ResumeCalled;
            }
        }

        /// <summary>
        /// This is called every time the view is no longer top of the view stack - we should stop
        /// scanning for nfc tags 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void droidEvents_PauseCalled(object sender, EventArgs e)
        {
            StopForegroundDispatch();
        }

        /// <summary>
        /// This is fired when a NFC tag is scanned (and we are doing foreground scanning)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void droidEvents_NewIntentCalled(object sender, Cirrious.CrossCore.Core.MvxValueEventArgs<Intent> e)
        {

            NewIntent(e);


        }

        /// <summary>
        /// Call this to disable foreground scanning
        /// </summary>
        protected void StopForegroundDispatch()
        {
            _nfcAdapter.DisableForegroundDispatch(TopActivity);
        }

        protected abstract void NewIntent(Cirrious.CrossCore.Core.MvxValueEventArgs<Intent> e);


        protected string GetIdFromTag(Intent e)
        {
            string id = string.Empty;
            var tagAsObj = e.GetParcelableExtra(NfcAdapter.ExtraTag);
            if (tagAsObj != null)
            {
                Tag tag = tagAsObj as Tag;
                byte[] idAsByte = tag.GetId();

                id = BitConverter.ToString(idAsByte);
            }
            return id;
        }
    }
}