using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ZI_CRYPTER.Utils
{
    class RelayCommand : ICommand
    {
        private readonly Action<object> _excecute;
        private readonly Func<object, bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _excecute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object param) => _canExecute == null || _canExecute(param);
        public void Execute(object param) => _excecute(param);
    }
}
