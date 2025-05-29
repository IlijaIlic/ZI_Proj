using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZI_CRYPTER.Model;
using ZI_CRYPTER.Utils;

namespace ZI_CRYPTER.ViewModel
{
    class SettingsVM : Utils.ViewModelBase
    {
        private readonly PageModel _pageModel;
        public readonly ViewModelBase _vmBase;

        public ICommand ChangeTargetLocationCommand { get; set; }
        public ICommand ChangeXLocationCommand { get; set; }


        public SettingsVM(ViewModelBase vmb)
        {
            _pageModel = new PageModel();
            _vmBase = vmb;
            ChangeTargetLocationCommand = new RelayCommand(ChangeTargetLocation);
            ChangeXLocationCommand = new RelayCommand(ChangeXLocation);

        }

        public string XPath
        {
            get => _vmBase.SharedXPath;
            set
            {
                _vmBase.SharedXPath = value;
                OnProprtyChanged(nameof(XPath));
            }
        }
        public string FSWPath
        {
            get => _vmBase.SharedFSWPath;
            set
            {
                _vmBase.SharedFSWPath = value;
                OnProprtyChanged(nameof(FSWPath));
            }
        }

        public string CodeAlg
        {
            get => _vmBase.SharedCodeAlg;
            set
            {
                _vmBase.SharedCodeAlg = value;
                OnProprtyChanged(nameof(ViewModelBase));
            }
        }

        public string CodeKey
        {
            get => _vmBase.SharedCodeKey;
            set
            {
                _vmBase.SharedCodeKey = value;
                OnProprtyChanged(nameof(ViewModelBase));
            }
        }

        private void ChangeTargetLocation(object parameter)
        {
            var folderDialog = new OpenFolderDialog
            {

            };

            if (folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
                _vmBase.SharedFSWPath = folderName;
                OnProprtyChanged(nameof(FSWPath));

            }

        }

        private void ChangeXLocation(object parameter)
        {
            var folderDialog = new OpenFolderDialog
            {

            };

            if (folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
                _vmBase.SharedXPath = folderName;
                OnProprtyChanged(nameof(XPath));

            }
        }
    }
}
