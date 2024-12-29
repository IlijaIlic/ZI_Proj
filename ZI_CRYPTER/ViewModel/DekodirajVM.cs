using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public DekodirajVM(ViewModelBase vmb)
        {
            _pageModel = new PageModel();

            _vmBase = vmb;
            ChooseOutputLocationCommand = new RelayCommand(ChangeOutputLocation);
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
