using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using ZI_CRYPTER.Model;
using ZI_CRYPTER.Utils;

namespace ZI_CRYPTER.ViewModel
{
    class PosaljiVM : Utils.ViewModelBase
    {
        private readonly PageModel _pageModel;
        public readonly ViewModelBase _vmBase;


        public PosaljiVM(ViewModelBase vmb)
        {
            _pageModel = new PageModel();
            _vmBase = vmb;
        }

        public string SendIP
        {
            get => _vmBase.SharedSendIP;
            set
            {
                _vmBase.SharedSendIP = value;
                OnProprtyChanged(nameof(SharedSendIP));
            }
        }

        public string SendPort
        {
            get => _vmBase.SharedSendPort;
            set
            {
                _vmBase.SharedSendPort = value;
                OnProprtyChanged(nameof(SharedSendPort));
            }
        }
    }
}
