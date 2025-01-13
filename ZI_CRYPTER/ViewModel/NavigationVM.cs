using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZI_CRYPTER.Utils;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace ZI_CRYPTER.ViewModel
{
    class NavigationVM : Utils.ViewModelBase
    {
        private object _currentView;

        public ObservableCollection<ViewModelBase> Tabs { get; }

        public object CurrentView
        {
            get { return _currentView; }
            set 
            { 
                _currentView = value; 
                OnProprtyChanged(); 
            }
        }

        public ICommand DekodirajCommand { get; set; }
        public ICommand KodirajCommand{ get; set; }
        public ICommand PosaljiCommand { get; set; }
        public ICommand PostavkeCommand { get; set; }

        private void Dekodiraj(object obj)
        {
            var dekodirajVm = new DekodirajVM(App.ViewModelBaseInstance);

            CurrentView = dekodirajVm;
        }

        private void Kodiraj(object obj)
        {
            var kodirajVm = new KodirajVM(App.ViewModelBaseInstance);
            
            CurrentView = kodirajVm;
        }

        private void Posalji(object obj) => CurrentView = new PosaljiVM();
        private void Postavke(object obj){

            var postavkeVm = new SettingsVM(App.ViewModelBaseInstance);

            CurrentView = postavkeVm;
        }

        public NavigationVM()
        {

            Tabs = new ObservableCollection<ViewModelBase>();

            DekodirajCommand = new RelayCommand(Dekodiraj);
            KodirajCommand = new RelayCommand(Kodiraj);
            PosaljiCommand = new RelayCommand(Posalji);
            PostavkeCommand = new RelayCommand(Postavke);

            CurrentView = new KodirajVM(App.ViewModelBaseInstance);

        }
    }
}
