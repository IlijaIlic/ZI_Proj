using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows.Input;
using System.Windows.Threading;
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

        public ObservableCollection<string> CodedFiles
        {
            get => _vmBase.SharedCodedFiles;

        }


        private void Code(object parameter)
        {
            Task.Factory.StartNew(() =>
            {
                if (Checker())
                {

                    byte[] keyBytes = Encoding.ASCII.GetBytes(CodeKey);
                    try
                    {
                        if (!FSWCheck)
                            ObicnoKodiranje(keyBytes);
                        else
                        {
                            App.Current.Dispatcher.Invoke(() =>
                            {
                                WindowInfoAlert wia = new WindowInfoAlert("Morate iskljuciti FSW!");
                                wia.Owner = App.Current.MainWindow;

                                wia.ShowDialog();
                            });
                            return;
                        }

                        App.Current.Dispatcher.Invoke(() =>
                        {
                            WindowInfoAlert wia = new WindowInfoAlert("Uspesno kodirano :D");
                            wia.Owner = App.Current.MainWindow;

                            wia.ShowDialog();
                        });

                    }
                    catch (Exception ex)
                    {
                        if (ex is UnauthorizedAccessException)
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
                                WindowInfoAlert wia = new WindowInfoAlert(ex.Message);
                                wia.Owner = App.Current.MainWindow;

                                wia.ShowDialog();
                            });
                        }

                    }
                }

            });
        }

        private bool Checker()
        {
            if (CodeAlg == "undef")
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    WindowInfoAlert wia = new WindowInfoAlert("Morate odabrati algoritam za kodiranje u postavkama!");
                    wia.Owner = App.Current.MainWindow;
                    wia.ShowDialog();
                });
                return false;
            }
            else if (CodeKey == "")
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    WindowInfoAlert wia = new WindowInfoAlert("Morate uneti kljuc za kodiranje u postavkama!");
                    wia.Owner = App.Current.MainWindow;

                    wia.ShowDialog();

                });
                return false;

            }
            //else if (FilesToCode.Count == 0)
            //{
            //    App.Current.Dispatcher.Invoke(() =>
            //    {
            //        WindowInfoAlert wia = new WindowInfoAlert("Nema fajlova za kodiranje!");
            //        wia.Owner = App.Current.MainWindow;

            //        wia.ShowDialog();
            //        return false;
            //    });
            //}
            else if (XPath == "")
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    WindowInfoAlert wia = new WindowInfoAlert("Morate podesiti output lokaciju!");
                    wia.Owner = App.Current.MainWindow;

                    wia.ShowDialog();
                });
                return false;
            }
            else if (FSWCheck == true && FSWPath == "")
            {
                App.Current.Dispatcher.Invoke(() =>
                {
                    WindowInfoAlert wia = new WindowInfoAlert("Morate podesiti folder za FSW!");
                    wia.Owner = App.Current.MainWindow;

                    wia.ShowDialog();
                });
                return false;
            }
            return true;
        }

        private void FSWKodiranje(byte[] keyBytes, string filePath, string fileName)
        {
            if (CodeAlg == "XTEA")
            {

                XTEA.EncryptFileParallelBuffered(filePath, Path.Combine(XPath, "enc - " + fileName), keyBytes);

            }
            else if (CodeAlg == "A5/1")
            {

                A51Faster.useA51(filePath, Path.Combine(XPath, "enc - " + fileName), keyBytes);

            }
            else if (CodeAlg == "XTEA + OFB")
            {

                XTEA.OFB(filePath, Path.Combine(XPath, "enc - " + fileName), keyBytes, Encoding.ASCII.GetBytes("asdfasdf"));

            }
        }

        private void ObicnoKodiranje(byte[] keyBytes)
        {
            if (CodeAlg == "XTEA")
            {
                foreach (var file in FilesToCode)
                {
                    XTEA.EncryptFileParallelBuffered(Path.GetFullPath(file), Path.Combine(XPath, "enc - " + Path.GetFileName(file)), keyBytes);
                }
            }
            else if (CodeAlg == "A5/1")
            {
                foreach (var file in FilesToCode)
                {
                    A51Faster.useA51(Path.GetFullPath(file), Path.Combine(XPath, "enc - " + Path.GetFileName(file)), keyBytes);
                }
            }
            else if (CodeAlg == "XTEA + OFB")
            {
                foreach (var file in FilesToCode)
                {
                    XTEA.OFB(Path.GetFullPath(file), Path.Combine(XPath, "enc - " + Path.GetFileName(file)), keyBytes, Encoding.ASCII.GetBytes("asdfasdf"));
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
                CodedFiles.Clear();

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

                CodedFiles.Clear();
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


                foreach (var item in FilesToCode)
                {
                    CodedFiles.Add(item);
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
                        if (!Checker())
                        {
                            App.Current.Dispatcher.Invoke(() =>
                            {
                                FSWCheck = false;
                                OnProprtyChanged(nameof(FSWCheck));
                            });

                            return;
                        }
                        FilesToCode.Clear();
                        CodedFiles.Clear();

                        byte[] keyBytes = Encoding.ASCII.GetBytes(CodeKey);

                        Wathcer.Path = _vmBase.SharedFSWPath;
                        Wathcer.IncludeSubdirectories = true;
                        Wathcer.EnableRaisingEvents = true;

                        Wathcer.Created += OnFileCreated;

                        Task.Factory.StartNew(() =>
                        {
                            InitializeFiles();
                            codeSlowFiles(keyBytes);
                        });
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

                    WindowInfoAlert wia = new WindowInfoAlert(ex.Message);
                    wia.ShowDialog();
                    FSWCheck = false;
                    OnProprtyChanged();

                }
            }
        }

        private void codeSlowFiles(byte[] keyBytes)
        {
            while (FilesToCode.ToList().Count > 0)
            {
                foreach (var file in FilesToCode.ToList())
                {
                    try
                    {
                        FilesToCode.Remove(file);
                        string name = Path.GetFileName(file);
                        FSWKodiranje(keyBytes, file, name);
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            CodedFiles.Add(name);
                        });
                    }
                    catch (Exception exce)
                    {
                        if (exce is not UnauthorizedAccessException)
                        {
                            App.Current.Dispatcher.Invoke(() =>
                            {
                                WindowInfoAlert wia = new WindowInfoAlert(exce.Message);
                                wia.Owner = App.Current.MainWindow;
                                wia.ShowDialog();
                            });
                            FilesToCode.Add(file);
                        }
                        else
                        {
                            FilesToCode.Remove(file);
                        }
                    }
                }
            }
        }
        private void InitializeFiles()
        {
            FilesToCode.Clear();
            foreach (var file in Directory.GetFiles(_vmBase.SharedFSWPath))
            {
                FilesToCode.Add(Path.GetFullPath(file));
            }
        }

        private void OnFileCreated(object sender, FileSystemEventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                App.Current.Dispatcher.Invoke(() => FilesToCode.Add(Path.GetFileName(e.FullPath)));

                byte[] keyBytes = Encoding.ASCII.GetBytes(CodeKey);

                try
                {
                    string name = Path.GetFileName(e.FullPath);
                    FSWKodiranje(keyBytes, e.FullPath, name);
                    App.Current.Dispatcher.Invoke(() =>
                    {
                        CodedFiles.Add(name);
                    });



                }
                catch (Exception exce)
                {
                    if (exce is UnauthorizedAccessException)
                    {
                        FilesToCode.Add(e.FullPath);
                        codeSlowFiles(keyBytes);
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
    }
}
