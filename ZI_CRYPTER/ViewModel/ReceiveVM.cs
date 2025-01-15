using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using ZI_CRYPTER.Model;
using ZI_CRYPTER.Utils;

namespace ZI_CRYPTER.ViewModel
{
    public class ReceiveVM : Utils.ViewModelBase
    {
        private readonly PageModel _pageModel;
        public readonly ViewModelBase _vmBase;

        

        public ICommand ListenCommand { get; set; }

        public ReceiveVM(ViewModelBase vmb)
        {
            _pageModel = new PageModel();
            _vmBase = vmb;

            ListenCommand = new RelayCommand(Listen);
        }
        
        public string ReceivePort
        {
            get => _vmBase.SharedReceivePort;
            set
            {
                _vmBase.SharedReceivePort = value;
                OnProprtyChanged(nameof(SharedReceivePort));
            }
        }

        private void Listen(object obj)
        {
            WindowInfoAlert wia = new WindowInfoAlert("Osluskivanje ukljuceno");
        }
    }
}
