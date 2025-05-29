using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
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
        public ICommand ChangeTargetLocationCommand { get; set; }



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

        public string FSWPath
        {
            get => _vmBase.SharedFSWPath;
            set
            {
                _vmBase.SharedFSWPath = value;
                OnProprtyChanged();
            }
        }

        public string XPath
        {
            get => _vmBase.SharedXPath;
            set
            {
                _vmBase.SharedXPath = value;
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
            else if (CodeKey == "")
            {
                WindowInfoAlert wia = new WindowInfoAlert("Morate uneti kljuc za kodiranje u postavkama!");
                wia.ShowDialog();

            }
            else if (FilesToCode.Count == 0)
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
                            XTEA.EncryptFile(Path.Combine(FSWPath, file), Path.Combine(XPath,"enc - " + file), keyBytes);
                        }
                        WindowInfoAlert wia = new WindowInfoAlert("Uspesno kodirano :D");
                        wia.ShowDialog();
                    }
                    else
                    {
                        foreach (var file in FilesToCode)
                        {
                            XTEA.EncryptFile(Path.GetFullPath(file), Path.Combine(XPath, "enc - " + Path.GetFileName(file)), keyBytes);
                        }
                        WindowInfoAlert wia = new WindowInfoAlert("Uspesno kodirano :D");
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
                try
                {
                    if (isChecked)
                    {
                        Wathcer.Path = _vmBase.SharedFSWPath;
                        Wathcer.IncludeSubdirectories = true;
                        Wathcer.EnableRaisingEvents = true;

                        Wathcer.Created += OnFileCreated;

                        InitializeFiles();
                    }
                    else
                    {

                        Wathcer.EnableRaisingEvents = false;
                        Wathcer.Created -= OnFileCreated;

                        FilesToCode.Clear();
                    }
                    OnProprtyChanged();
                }
                catch (Exception ex)
                {
                    WindowInfoAlert wia = new WindowInfoAlert("Proverite da li ste postavili Target folder u postavkama");
                    wia.ShowDialog();
                }
            }
        }
        private void InitializeFiles()
        {
            FilesToCode.Clear();
            foreach (var file in Directory.GetFiles(_vmBase.SharedFSWPath))
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
