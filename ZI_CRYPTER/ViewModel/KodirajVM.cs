using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZI_CRYPTER.Model;

namespace ZI_CRYPTER.ViewModel
{
    public class KodirajVM : Utils.ViewModelBase
    {
        private readonly PageModel _pageModel;

        public KodirajVM()
        {
            _pageModel = new PageModel();
        }
    }
}
