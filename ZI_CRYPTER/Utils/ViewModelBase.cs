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
       

        // ReceiveVM - nekorisceno
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


        // ReceiveVM - nekorisceno
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


        // ReceiveVM - nekorisceno
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
        private ObservableCollection<string> SharedFileToReceive
        {
            get => _sharedFileToReceive;
            set
            {
                _sharedFileToReceive = value;
                OnProprtyChanged(nameof(SharedFileToReceive));
            }
        }


        // PosaljiVM - nekorisceno
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


        // PosaljiVM nije gotovo 4 u 1 ???? - nekorisceno
        private string _sharedSendIP;
        public string SharedSendIP
        {
            get => _sharedSendIP;
            set
            {
                _sharedSendIP = value;
                OnProprtyChanged(nameof(SharedSendIP));
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


        // KodirajVM
        private static string _codeFSWPath = @"C:\Users\ilici\Desktop\trt\";
        public static string SharedFSWPath
        {
            get => _codeFSWPath;
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
