using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZI_CRYPTER.Model;

namespace ZI_CRYPTER.ViewModel
{
    class DekodirajVM : Utils.ViewModelBase
    {
        private readonly PageModel _pageModel;
        public DekodirajVM()
        {
            _pageModel = new PageModel();
        }
    }
}
