using Cryptography;
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

        public string DecodedFileName
        {
            get => _vmBase.SharedDecodedFileName;
            set
            {
                _vmBase.SharedDecodedFileName = value;
                OnProprtyChanged(nameof(ViewModelBase));
            }
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
            //WindowInfoAlert wia12 = new WindowInfoAlert(BLAKE.HashToHexString("hella"));
            //wia12.Show();
            Task.Factory.StartNew(() =>
            {

                if (FileToDecode.Count == 0)
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        WindowInfoAlert wia1 = new WindowInfoAlert("Niste odabrali fajl za dekodiranje!");
                        wia1.Owner = App.Current.MainWindow;

                        wia1.ShowDialog();
                    });

                    return;
                }
                if (DecodeAlg == "undef")
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        WindowInfoAlert wia2 = new WindowInfoAlert("Niste odabrali algoritam za dekodiranje");
                        wia2.Owner = App.Current.MainWindow;

                        wia2.ShowDialog();
                    });

                    return;
                }
                if (DecodeKey == "")
                {
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        WindowInfoAlert wia3 = new WindowInfoAlert("Niste uneli kljuc za dekodiranje!");
                        wia3.Owner = App.Current.MainWindow;

                        wia3.ShowDialog();
                    });

                    return;
                }
                try
                {
                    string output;
                    byte[] keyByte = Encoding.ASCII.GetBytes(DecodeKey);


                    if (DecodedFileName == "")
                        output = Path.Combine(DecodeOutput, Path.GetFileName(FileToDecode[0].Replace("enc - ", "dec - ")));

                    else
                    {
                        string ext = Path.GetExtension(FileToDecode[0]);
                        output = Path.Combine(DecodeOutput, DecodedFileName + ext);
                    }


                    if (DecodeAlg == "XTEA")
                    {
                        XTEA.DecryptFileParallelBuffered(FileToDecode[0], output, keyByte);
                    }
                    else if (DecodeAlg == "A5/1")
                    {
                        A51Faster.useA51(FileToDecode[0], output, keyByte);
                    }
                    else if (DecodeAlg == "XTEA + OFB")
                    {
                        XTEA.OFB(FileToDecode[0], output, keyByte, Encoding.ASCII.GetBytes("asdfasdf"));
                    }
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        WindowInfoAlert wia = new WindowInfoAlert("Uspesno dekodirano!");
                        wia.ShowDialog();
                    });

                }
                catch (Exception exce)
                {
                    if (exce is UnauthorizedAccessException)
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            WindowInfoAlert wia = new WindowInfoAlert("Za zeljeni output direktorijum su potrebne privilegije administratora!");
                            wia.Owner = App.Current.MainWindow;

                            wia.ShowDialog();
                        });

                    }
                    else
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {

                            WindowInfoAlert wia = new WindowInfoAlert(exce.Message);

                            wia.Owner = App.Current.MainWindow;
                            wia.ShowDialog();
                        });

                    }
                }
            });

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
