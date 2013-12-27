using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Cirrious.MvvmCross.ViewModels;

namespace DevelopingTrends.Sample.NFC.Core.ViewModels
{
    public class HomeViewModel : MvxViewModel
    {
        public HomeViewModel()
        {
            ScanAsTaskCommand = new MvxCommand(DoScanAsTask);
            ScanAsMessageCommand = new MvxCommand(DoScanAsMessage);
        }

        private void DoScanAsMessage()
        {
            ShowViewModel<ScanAsMessageViewModel>();
        }

        private void DoScanAsTask()
        {
            ShowViewModel<ScanAsTaskViewModel>();
        }

        public ICommand ScanAsTaskCommand
        {
            get; 
            set;
        }

        public ICommand  ScanAsMessageCommand { get; set; }
    }
}
