using System;
using System.Collections.Generic;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Nfc;
using Cirrious.CrossCore;
using Cirrious.CrossCore.Core;
using Cirrious.CrossCore.Droid.Platform;
using Cirrious.CrossCore.Droid.Views;

namespace DevelopingTrends.MvxPlugins.NFC.Droid
{
    public abstract class MvxNFCReadBase : MvxMainThreadDispatchingObject,IMvxNFC
    {
        private bool _haveManifestPermission = false;
        protected bool _dontThrowExpceptionWhenNotSupported = true;
        protected NfcAdapter _nfcAdapter;

        public MvxNFCReadBase()
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

        protected string GetMessage(NdefMessage tag)
        {
            var messageByte = tag.ToByteArray();
            int header = 7;
            string message = Encoding.ASCII.GetString(messageByte, header, messageByte.Length - header);
            return message;
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
            string id = string.Empty;
            var tagAsObj = e.Value.GetParcelableExtra(NfcAdapter.ExtraTag);
            if (tagAsObj != null)
            {
                Tag tag = tagAsObj as Tag;
                byte[] idAsByte = tag.GetId();

                id = BitConverter.ToString(idAsByte);
            }
            var tagAsNdefMessage = e.Value.GetParcelableArrayExtra(NfcAdapter.ExtraNdefMessages);
            if (tagAsNdefMessage != null)
            {

                var tag = tagAsNdefMessage[0] as NdefMessage;
                byte[] message = tag.ToByteArray();
                var ndefMessage = NdefLibrary.Ndef.NdefMessage.FromByteArray(message);

                NewMessage(ndefMessage);
                //var records = tag.GetRecords();
                //foreach (var record in records)
                //{
                //    int type = record.Tnf;
                //    TypeNameFormat tnfType = (TypeNameFormat) type;
                //    byte[] payload = record.GetPayload();
                //    byte[] info = record.GetTypeInfo();
                //    string payloadString = Encoding.ASCII.GetString(payload, 0, payload.Length);
                //    string infostring = Encoding.ASCII.GetString(info, 0, info.Length);
                //    Mvx.Trace("ID: {0}, Type: {3}, Payload: {1}, TypeInfo :{2}",id,payloadString,infostring,type);                    
                //}
                //NewMessage(GetMessage(tag));
            }



        }
        /// <summary>
        /// Call this to disable foreground scanning
        /// </summary>
        protected void StopForegroundDispatch()
        {
            _nfcAdapter.DisableForegroundDispatch(TopActivity);
        }
        /// <summary>
        /// Pass any read NFC message to a child implementation
        /// </summary>
        /// <param name="message">The message</param>
        protected abstract void NewMessage(NdefLibrary.Ndef.NdefMessage message);
    }
}