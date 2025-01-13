using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Printing.IndexedProperties;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZI_CRYPTER.Model;
using ZI_CRYPTER.Utils;

namespace ZI_CRYPTER.ViewModel
{
    public class DekodirajVM : Utils.ViewModelBase
    {
        private readonly PageModel _pageModel;
        public readonly ViewModelBase _vmBase;
        public ICommand ChooseOutputLocationCommand { get; set; }
        public ICommand AddFileToDecodeCommand { get; set; }
        public ICommand DecodeCommand { get; set; }

        public DekodirajVM(ViewModelBase vmb)
        {
            _pageModel = new PageModel();

            _vmBase = vmb;
            ChooseOutputLocationCommand = new RelayCommand(ChangeOutputLocation);
            AddFileToDecodeCommand = new RelayCommand(AddFileToDecode);
            DecodeCommand = new RelayCommand(Decode);

        }

        public string DecodeOutput
        {
            get => _vmBase.SharedDecodeOutputPutanja;
            set
            {
                _vmBase.SharedDecodeOutputPutanja = value;
                OnProprtyChanged(nameof(ViewModelBase));
            }
        }

        public string DecodeKey
        {
            get => _vmBase.SharedDecodeKey;
            set
            {
                _vmBase.SharedDecodeKey = value;
                OnProprtyChanged(nameof(ViewModelBase));
            }
        }
        public string DecodeAlg
        {
            get => _vmBase.SharedDecodeAlg;
            set
            {
                _vmBase.SharedDecodeAlg = value;
                OnProprtyChanged(nameof(ViewModelBase));
            }
        }

        public ObservableCollection<string> FileToDecode
        {
            get => _vmBase.SharedFileToDecode;
        }

        private void AddFileToDecode(object parameter)
        {
            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "All files (*.*)|*.*";
            ofd.Multiselect = false;
            if (ofd.ShowDialog() == true)
            {
                FileToDecode.Clear();
                FileToDecode.Add(ofd.FileName);
            }
            OnProprtyChanged();
        }

        private void Decode(object parameter)
        {
            if (FileToDecode.Count == 0)
            {
                WindowInfoAlert wia1 = new WindowInfoAlert("Niste odabrali fajl za dekodiranje!");

                wia1.ShowDialog();
                return;
            }
            if (DecodeAlg == "undef")
            {
                WindowInfoAlert wia2 = new WindowInfoAlert("Niste odabrali algoritam za dekodiranje");

                wia2.ShowDialog();
                return;
            }
            if (DecodeKey == "")
            {
                WindowInfoAlert wia3 = new WindowInfoAlert("Niste uneli kljuc za dekodiranje!");

                wia3.ShowDialog();
                return;
            }
            
            if(DecodeAlg == "XTEA")
            {
                byte[] keyByte = Encoding.ASCII.GetBytes(DecodeKey);
                XTEA.DecryptFile(FileToDecode[0], String.Concat(String.Concat(DecodeOutput,"\\") , Path.GetFileName(FileToDecode[0].Replace("enc - ", "dec - "))), keyByte);
        
            }
            WindowInfoAlert wia = new WindowInfoAlert("💥💥💥");
            wia.ShowDialog();
        }
        private void ChangeOutputLocation(object parameter)
        {
            var folderDialog = new OpenFolderDialog
            {

            };

            if (folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
                _vmBase.SharedDecodeOutputPutanja = folderName;
                OnProprtyChanged(nameof(DecodeOutput));

            }

        }

       
    }
}
