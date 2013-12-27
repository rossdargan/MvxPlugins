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
            WriteTextCommand = new MvxCommand(DoWriteText);
        }

        private void DoScanAsMessage()
        {
            ShowViewModel<ScanAsMessageViewModel>();
        }

        private void DoScanAsTask()
        {
            ShowViewModel<ScanAsTaskViewModel>();
        }
        private void DoWriteText()
        {
            ShowViewModel<WriteTextViewModel>();
        }

        public ICommand ScanAsTaskCommand
        {
            get; 
            set;
        }

        public ICommand  ScanAsMessageCommand { get; set; }
        public ICommand WriteTextCommand { get; set; }

    }
}
