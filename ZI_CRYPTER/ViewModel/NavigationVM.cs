﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZI_CRYPTER.Utils;
using System.Windows.Input;

namespace ZI_CRYPTER.ViewModel
{
    class NavigationVM : Utils.ViewModelBase
    {
        private object _currentView;

        public object CurrentView
        {
            get { return _currentView; }
            set { _currentView = value; OnProprtyChanged(); }
        }

        public ICommand DekodirajCommand { get; set; }
        public ICommand KodirajCommand{ get; set; }
        public ICommand PosaljiCommand { get; set; }
        public ICommand PostavkeCommand { get; set; }

        private void Dekodiraj(object obj) => CurrentView = new DekodirajVM();
        private void Kodiraj(object obj) => CurrentView = new KodirajVM();
        private void Posalji(object obj) => CurrentView = new PosaljiVM();
        private void Postavke(object obj) => CurrentView = new SettingsVM();

        public NavigationVM()
        {
            DekodirajCommand = new RelayCommand(Dekodiraj);
            KodirajCommand = new RelayCommand(Kodiraj);
            PosaljiCommand = new RelayCommand(Posalji);
            PostavkeCommand = new RelayCommand(Postavke);

            CurrentView = new KodirajVM();
        }
    }
}