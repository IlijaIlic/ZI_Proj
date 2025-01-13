using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.IO;
using System.Collections.ObjectModel;

namespace ZI_CRYPTER.Utils
{
    public class ViewModelBase : INotifyPropertyChanged
    {

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



        private static string _codeFSWPath = @"C:\Users\ilici\Desktop\trt\";
        public static string SharedFSWPath
        {
            get => _codeFSWPath;
        }



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



        public event PropertyChangedEventHandler PropertyChanged;

        public void OnProprtyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
