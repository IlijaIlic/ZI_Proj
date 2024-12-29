using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ZI_CRYPTER.Utils
{
    public class ViewModelBase : INotifyPropertyChanged
    {

        private string _decodeOutputPutanja;

        public string SharedDecodeOutputPutanja
        {
            get => _decodeOutputPutanja;
            set
            {
                _decodeOutputPutanja = value;
                OnProprtyChanged(nameof(SharedDecodeOutputPutanja));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnProprtyChanged([CallerMemberName] string propName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
