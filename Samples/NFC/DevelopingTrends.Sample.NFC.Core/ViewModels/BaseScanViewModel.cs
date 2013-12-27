using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cirrious.MvvmCross.ViewModels;

namespace DevelopingTrends.Sample.NFC.Core.ViewModels
{
    public class BaseScanViewModel : MvxViewModel
    {
        public BaseScanViewModel()
        {
            Messages = new ObservableCollection<MessageViewModel>();
        }

        protected void ClearDisplay()
        {
            Messages.Clear();
        }
        protected void UpdateDisplay(NdefLibrary.Ndef.NdefMessage messages)
        {
            Messages.Clear();
            foreach (var message in messages)
            {
                Messages.Add(new MessageViewModel(message));
            }
        }
        private bool _isScanning;

        public bool IsScanning
        {
            get { return _isScanning; }
            set
            {
                _isScanning = value;
                RaisePropertyChanged(() => IsScanning);
            }
        }

        public ObservableCollection<MessageViewModel> Messages
        {
            get;
            set;

        }
    }
}
