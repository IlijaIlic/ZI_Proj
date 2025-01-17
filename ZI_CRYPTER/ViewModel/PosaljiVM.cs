using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZI_CRYPTER.Model;
using ZI_CRYPTER.Utils;

namespace ZI_CRYPTER.ViewModel
{
    class PosaljiVM : Utils.ViewModelBase
    {
        private readonly PageModel _pageModel;
        public readonly ViewModelBase _vmBase;

        public ICommand AddFileToSendCommand { get; set; }
        public ICommand PosaljiCommand { get; set; }

        public PosaljiVM(ViewModelBase vmb)
        {
            _pageModel = new PageModel();
            _vmBase = vmb;

            AddFileToSendCommand = new RelayCommand(AddFileToSend);
            PosaljiCommand = new RelayCommand(Posalji); 
        }

        public string SendIP1
        {
            get => _vmBase.SharedSendIP1;
            set
            {
                _vmBase.SharedSendIP1 = value;
                OnProprtyChanged(nameof(SharedSendIP1));
            }
        }

        public string SendIP2
        {
            get => _vmBase.SharedSendIP2;
            set
            {
                _vmBase.SharedSendIP2 = value;
                OnProprtyChanged(nameof(SharedSendIP2));
            }
        }

        public string SendIP3
        {
            get => _vmBase.SharedSendIP3;
            set
            {
                _vmBase.SharedSendIP3 = value;
                OnProprtyChanged(nameof(SharedSendIP3));
            }
        }

        public string SendIP4
        {
            get => _vmBase.SharedSendIP4;
            set
            {
                _vmBase.SharedSendIP4 = value;
                OnProprtyChanged(nameof(SharedSendIP4));
            }
        }

        public string SendPort
        {
            get => _vmBase.SharedSendPort;
            set
            {
                _vmBase.SharedSendPort = value;
                OnProprtyChanged(nameof(SharedSendPort));
            }
        }

       public ObservableCollection<string> FileToSend
       {
            get => _vmBase.SharedFileToSend;
            set
            {
                _vmBase.SharedFileToSend = value;
                OnProprtyChanged(nameof(SharedFileToSend));
            }
       }

        public string InfoText
        {
            get => _vmBase.SharedInfoText;
            set
            {
                _vmBase.SharedInfoText = value;
                OnProprtyChanged(nameof(SharedInfoText));
            }
        }


        #region EventHandlersKlijent

        private async void Posalji(object sender)
        {
            if(SendIP1 != "" && SendPort != "")
            {
                await Soketi.AdvanceKlijent(String.Concat(SendIP1, ".", SendIP2, ".", SendIP3, ".", SendIP4), SendPort, FileToSend[0], InfoText); 
            }
        }

        private void AddFileToSend(object parameter)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "All files (*.*)|*.*";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == true)
            {
                FileToSend.Clear();
                FileToSend.Add(ofd.FileName);
            }
            OnProprtyChanged();
        }

   
        #endregion

        
    }
}
