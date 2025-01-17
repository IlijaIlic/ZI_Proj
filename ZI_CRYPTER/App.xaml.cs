﻿using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;
using ZI_CRYPTER.Utils;

namespace ZI_CRYPTER
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        public static ViewModelBase ViewModelBaseInstance { get; private set; } = new ViewModelBase();

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ViewModelBaseInstance.SharedCodeAlg = "undef";
            ViewModelBaseInstance.SharedCodeKey = "";
            ViewModelBaseInstance.SharedFSWChecked = false;
            ViewModelBaseInstance.SharedWatcher = new FileSystemWatcher();
            ViewModelBaseInstance.SharedFilesToCode = new ObservableCollection<string>();

            ViewModelBaseInstance.SharedDecodeOutputPutanja = "C:\\Program Files\\";
            ViewModelBaseInstance.SharedDecodeAlg = "undef";
            ViewModelBaseInstance.SharedDecodeKey = "";
            ViewModelBaseInstance.SharedFileToDecode = new ObservableCollection<string>();

            ViewModelBaseInstance.SharedSendIP1 = "";
            ViewModelBaseInstance.SharedSendIP2 = "";
            ViewModelBaseInstance.SharedSendIP3 = "";
            ViewModelBaseInstance.SharedSendIP4 = "";
            ViewModelBaseInstance.SharedSendPort = "";
            ViewModelBaseInstance.SharedInfoText = "";
            ViewModelBaseInstance.SharedFileToSend = new ObservableCollection<string>();

            ViewModelBaseInstance.SharedReceivePort = "";
            ViewModelBaseInstance.SharedInfoTextRec = "";
        }
    }

}
