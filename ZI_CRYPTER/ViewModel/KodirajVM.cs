using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZI_CRYPTER.Model;
using ZI_CRYPTER.Utils;

namespace ZI_CRYPTER.ViewModel
{
    public class KodirajVM : Utils.ViewModelBase
    {
        private readonly PageModel _pageModel;
        public readonly ViewModelBase _vmBase;

        public ICommand CheckCommand { get; set; }
        public ICommand RemoveAllFilesCommand { get; set; }
        public ICommand AddFileCommand { get; set; }
        public ICommand CodeCommand { get; set; }



        public KodirajVM(ViewModelBase vmb)
        {
            _pageModel = new PageModel();
            _vmBase = vmb;

            CheckCommand = new RelayCommand(OnCheckChanged);
            RemoveAllFilesCommand = new RelayCommand(RemoveAllFiles);
            AddFileCommand = new RelayCommand(AddFile);
            CodeCommand = new RelayCommand(Code);

        }


        public string CodeKey
        {
            get => _vmBase.SharedCodeKey;
            set
            {
                _vmBase.SharedCodeKey = value;
                OnProprtyChanged(nameof(CodeKey));
            }
        }

        public string CodeAlg
        {
            get => _vmBase.SharedCodeAlg;
            set
            {
                _vmBase.SharedCodeAlg = value;
                OnProprtyChanged(nameof(CodeAlg));
            }
        }

        public FileSystemWatcher Wathcer
        {
            get => _vmBase.SharedWatcher;
            set
            {
                _vmBase.SharedWatcher = value;
                OnProprtyChanged(nameof(Wathcer));
            }
        }

        public bool FSWCheck
        {
            get => _vmBase.SharedFSWChecked;
            set
            {
                _vmBase.SharedFSWChecked = value;
                OnProprtyChanged();

            }
        }

        public ObservableCollection<string> FilesToCode
        { 
            get => _vmBase.SharedFilesToCode; 
        }


        private void Code(object parameter)
        {
            if (CodeAlg == "undef")
            {
                WindowInfoAlert wia = new WindowInfoAlert("Morate odabrati algoritam za kodiranje u postavkama!");
                wia.ShowDialog();
            }
            else if(CodeKey == "" )
            {
                WindowInfoAlert wia = new WindowInfoAlert("Morate uneti kljuc za kodiranje u postavkama!");
                wia.ShowDialog();

            }
            else if(FilesToCode.Count == 0)
            {
                WindowInfoAlert wia = new WindowInfoAlert("Nema fajlova za kodiranje!");
                wia.ShowDialog();
            }
            else
            {

                if (CodeAlg == "XTEA")
                {
                    
                    byte[] keyBytes = Encoding.ASCII.GetBytes(CodeKey);

                    if (FSWCheck)
                    {
                        foreach (var file in FilesToCode)
                        {
                            XTEA.EncryptFile(String.Concat(ViewModelBase.SharedFSWPath, file), String.Concat(ViewModelBase.SharedFSWPath, String.Concat("enc - ",file)), keyBytes);
                        }
                        WindowInfoAlert wia = new WindowInfoAlert("💥💥💥");
                        wia.ShowDialog();
                    }
                    else
                    {
                        foreach (var file in FilesToCode)
                        {
                            XTEA.EncryptFile( file, String.Concat(ViewModelBase.SharedFSWPath, String.Concat("enc - ", Path.GetFileName(file))), keyBytes);
                        }
                        WindowInfoAlert wia = new WindowInfoAlert("💥💥💥");
                        wia.ShowDialog();
                    }
                }
            }
        }


        private void RemoveAllFiles(object parameter)
        {
            if (FSWCheck)
            {
                WindowInfoAlert wia = new WindowInfoAlert("Ne možete ukloniti fajlove dok je FSW ukljucen!");
                
                wia.ShowDialog();
            } 
            else
            {
                FilesToCode.Clear();

            }
            OnProprtyChanged();
        }

        private void AddFile(object sender)
        {
            if (FSWCheck)
            {
                WindowInfoAlert wia = new WindowInfoAlert("Ne možete dodati fajlove dok je FSW ukljucen!");

                wia.ShowDialog();
            }
            else
            {

                Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
                ofd.Filter = "All files (*.*)|*.*";
                ofd.Multiselect = true;
                if (ofd.ShowDialog() == true)
                {
                    foreach (var file in ofd.FileNames)
                    {
                        if (!FilesToCode.Contains(file))
                            FilesToCode.Add(file);
                    }
                }
                OnProprtyChanged();
            }
        }

        private void OnCheckChanged(object parameter)
        {
            if (parameter is bool isChecked)
            {
                if (isChecked)
                {
                    Wathcer.Path = ViewModelBase.SharedFSWPath;
                    Wathcer.IncludeSubdirectories = true;
                    Wathcer.EnableRaisingEvents = true;

                    // Subscribe to FileSystemWatcher events
                    Wathcer.Created += OnFileCreated;
                    Wathcer.Deleted += OnFileDeleted;
                    Wathcer.Renamed += OnFileRenamed;

                    // Initialize files in the directory
                    InitializeFiles();
                }
                else
                {

                    Wathcer.EnableRaisingEvents = false;
                    Wathcer.Created -= OnFileCreated;
                    Wathcer.Deleted -= OnFileDeleted;
                    Wathcer.Renamed -= OnFileRenamed;

                    FilesToCode.Clear();
                }
                OnProprtyChanged();

            }
        }
        private void InitializeFiles()
        {
            FilesToCode.Clear();
            foreach (var file in Directory.GetFiles(ViewModelBase.SharedFSWPath))
            {
                FilesToCode.Add(Path.GetFileName(file));
            }
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() => FilesToCode.Add(Path.GetFileName(e.FullPath)));
        }

        private void OnFileDeleted(object sender, FileSystemEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() => FilesToCode.Remove(Path.GetFileName(e.FullPath)));
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                FilesToCode.Remove(Path.GetFileName(e.OldFullPath));
                FilesToCode.Add(Path.GetFileName(e.FullPath));
            });
        }
    }
}
