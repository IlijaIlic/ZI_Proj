using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using System.Collections.ObjectModel;
using System.Security.AccessControl;

namespace ZI_CRYPTER.Utils
{
    public class ViewModelBase : INotifyPropertyChanged
    {


        // SettingsVM
        private string _sharedXPath = "";
        public string SharedXPath
        {
            get => _sharedXPath;
            set
            {
                _sharedXPath = value;
                OnProprtyChanged(nameof(SharedXPath));
            }
        }
       
        // ReceiveVM
        private bool _sharedReceiveChecked;
        public bool SharedReceiveChecked
        {
            get => _sharedReceiveChecked;
            set
            {
                _sharedReceiveChecked = value;
                OnProprtyChanged(nameof(SharedReceiveChecked));
            }
        }

        // ReceiveVM 
        private string _sharedReceivePort;
        public string SharedReceivePort
        {
            get => _sharedReceivePort;
            set
            {
                _sharedReceivePort = value;
                OnProprtyChanged(nameof(SharedReceivePort));
            }
        }


        // ReceiveVM 
        private string _sharedReceiveKey;
        public string SharedReceiveKey
        {
            get => _sharedReceiveKey;
            set
            {
                _sharedReceiveKey = value;
                OnProprtyChanged(nameof(SharedReceiveKey));
            }
        }


        // ReceiveVM 
        private string _sharedReceiveAlg;
        public string SharedReceiveAlg
        {
            get => _sharedReceiveAlg;
            set
            {
                _sharedReceiveAlg = value;
                OnProprtyChanged(nameof(SharedReceiveAlg));
            }
        }


        // ReceiveVM - nekorisceno
        private ObservableCollection<string> _sharedFileToReceive;
        public ObservableCollection<string> SharedFileToReceive
        {
            get => _sharedFileToReceive;
            set
            {
                _sharedFileToReceive = value;
                OnProprtyChanged(nameof(SharedFileToReceive));
            }
        }


        //ReceiveVM
        private string _sharedReceiveOutput;
        public string SharedReceiveOutput
        {
            get => _sharedReceiveOutput;
            set
            {
                _sharedReceiveOutput = value;
                OnProprtyChanged(nameof(SharedReceiveOutput));
            }
        }


        // ReceiveVM
        private string _sharedInfoTextRec;
        public string SharedInfoTextRec
        {
            get => _sharedInfoTextRec;
            set
            {
                _sharedInfoTextRec = value;
                OnProprtyChanged(nameof(SharedInfoTextRec));
            }
        }



        // PosaljiVM 
        private ObservableCollection<string> _sharedFileToSend;
        public ObservableCollection<string> SharedFileToSend
        {
            get => _sharedFileToSend;
            set
            {
                _sharedFileToSend = value;
                OnProprtyChanged(nameof(SharedFileToSend));
            }
        }


        // PosaljiVM 
        private string _sharedSendIP1;
        public string SharedSendIP1
        {
            get => _sharedSendIP1;
            set
            {
                _sharedSendIP1 = value;
                OnProprtyChanged(nameof(SharedSendIP1));
            }
        }


        // PosaljiVM 
        private string _sharedSendIP2;
        public string SharedSendIP2
        {
            get => _sharedSendIP2;
            set
            {
                _sharedSendIP2 = value;
                OnProprtyChanged(nameof(SharedSendIP2));
            }
        }


        // PosaljiVM 
        private string _sharedSendIP3;
        public string SharedSendIP3
        {
            get => _sharedSendIP3;
            set
            {
                _sharedSendIP3 = value;
                OnProprtyChanged(nameof(SharedSendIP3));
            }
        }


        // PosaljiVM 
        private string _sharedSendIP4;
        public string SharedSendIP4
        {
            get => _sharedSendIP4;
            set
            {
                _sharedSendIP4 = value;
                OnProprtyChanged(nameof(SharedSendIP4));
            }
        }


        // PosaljiVM
        private string _sharedSendPort;
        public string SharedSendPort
        {
            get => _sharedSendPort;
            set
            {
                _sharedSendPort = value;
                OnProprtyChanged(nameof(SharedSendPort));
            }
        }


        // PosaljiVM
        private string _sharedInfoText;
        public string SharedInfoText
        {
            get => _sharedInfoText;
            set
            {
                _sharedInfoText = value;
                OnProprtyChanged(nameof(SharedInfoText));
            }
        }




        // KodirajVM
        private ObservableCollection<string> _sharedFilesToCode;
        public ObservableCollection<string> SharedFilesToCode
        {
            get => _sharedFilesToCode;
            set
            {
                _sharedFilesToCode = value;
                OnProprtyChanged(nameof(SharedFilesToCode));
            }
        }

        //KodirajVM
        private ObservableCollection<string> _sharedCodedFiles;
        public ObservableCollection<string> SharedCodedFiles
        {
            get => _sharedCodedFiles;
            set
            {
                _sharedCodedFiles = value;
                OnProprtyChanged(nameof(SharedCodedFiles));
            }
        }


        // KodirajVM
        private string _codeFSWPath = "";
        public string SharedFSWPath
        {
            get => _codeFSWPath;
            set
            {
                _codeFSWPath = value;
                OnProprtyChanged(nameof(SharedFSWPath));

            }
        }


        // KodirajVM
        private FileSystemWatcher _watcher;
        public FileSystemWatcher SharedWatcher
        {
            get
            {
                if (_watcher == null)
                {
                    _watcher = new FileSystemWatcher
                    {
                        NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
                    };
                }
                return _watcher;
            }
            set
            {
                _watcher = value;
                OnProprtyChanged(nameof(SharedWatcher));
            }
        }


        // KodirajVM
        private bool _fswChecked;
        public bool SharedFSWChecked
        {
            get => _fswChecked;
            set
            {
                _fswChecked = value;
                OnProprtyChanged(nameof(SharedFSWChecked));
            }
        }


        // KodirajVM
        private string _codeAlg;
        public string SharedCodeAlg
        {
            get => _codeAlg;
            set
            {
                _codeAlg = value;
                OnProprtyChanged(nameof(SharedCodeAlg));
            }
        }


        // KodirajVM
        private string _codeKey;
        public string SharedCodeKey
        {
            get => _codeKey;
            set
            {
                _codeKey = value;
                OnProprtyChanged(nameof(SharedCodeKey));
            }
        }




        // DekodriajVM
        private string _decodeKey;
        public string SharedDecodeKey
        {
            get => _decodeKey;
            set
            {
                _decodeKey = value;
                OnProprtyChanged(nameof(SharedDecodeKey));
            }
        }


        // DekodirajVM
        private string _decodeAlg;
        public string SharedDecodeAlg
        {
            get => _decodeAlg;
            set
            {
                _decodeAlg = value;
                OnProprtyChanged(nameof(SharedDecodeAlg));
            }
        }


        // DekodirajVM
        private string _decodeOutputPutanja;
        public string SharedDecodeOutputPutanja
        {
            get => _decodeOutputPutanja;
            set
            {
                _decodeOutputPutanja = value;
                OnProprtyChanged(nameof(SharedDecodeOutputPutanja));
            }
        }


        // DekodirajVM
        private ObservableCollection<string> _sharedFileToDecode;
        public ObservableCollection<string> SharedFileToDecode
        {
            get => _sharedFileToDecode;
            set
            {
                _sharedFileToDecode = value;
                OnProprtyChanged(nameof(SharedFileToDecode));
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void OnProprtyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
