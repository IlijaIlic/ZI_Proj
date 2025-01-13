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
        public ICommand ChooseOutputLocationCommand { get; set; }

        public SettingsVM(ViewModelBase vmb)
        {
            _pageModel = new PageModel();
            _vmBase = vmb;

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
    }
}
