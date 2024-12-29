using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZI_CRYPTER.Model;

namespace ZI_CRYPTER.ViewModel
{
    class SettingsVM : Utils.ViewModelBase
    {
        private readonly PageModel _pageModel;

        public SettingsVM()
        {
            _pageModel = new PageModel();
        }
    }
}
