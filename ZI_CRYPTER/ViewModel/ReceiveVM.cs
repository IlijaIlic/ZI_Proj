using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
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
        private Socket serverSocket;
        

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

        public string ReceiveKey
        {
            get => _vmBase.SharedReceiveKey;
            set
            {
                _vmBase.SharedReceiveKey = value;
                OnProprtyChanged(nameof(SharedReceiveKey));
            }
        }

        public string ReceiveAlg
        {
            get => _vmBase.SharedReceiveAlg;
            set
            {
                _vmBase.SharedReceiveAlg = value;
                OnProprtyChanged(nameof(SharedReceiveAlg));
            }
        }

        public ObservableCollection<string> FileToReceive
        {
            get => _vmBase.SharedFileToReceive;
            set
            {
                _vmBase.SharedFileToReceive = value;
                OnProprtyChanged(nameof(SharedFileToReceive));
            }
        }

        public string InfoTextRec
        {
            get => _vmBase.SharedInfoTextRec;
            set
            {
                _vmBase.SharedInfoTextRec = value;
                OnProprtyChanged(nameof(SharedInfoTextRec));
            }
        }

        public bool ReceiveChecked
        {
            get => _vmBase.SharedReceiveChecked;
            set
            {
                _vmBase.SharedReceiveChecked = value;
                OnProprtyChanged(nameof(SharedReceiveChecked));
            }
        }

        private void Listen(object obj)
        {
            WindowInfoAlert wia = new WindowInfoAlert("Osluskivanje ukljuceno");
        }

        #region EventHandlersServer

        private async void Osluskuj(object sender, EventArgs e)
        {
            await Soketi.AdvanceServer(ReceivePort, serverSocket, InfoTextRec);
        }


        private void PrekiniOsluskivanje(object sender, EventArgs e)
        {
            try
            {
                serverSocket?.Close();
                // UPOZORI KORISNIKA
                // UpdateStatus(sslServer, "Server je zaustavljen.");
            }
            catch (Exception ex)
            {
                // UPOZORI KORISNIKA
                // UpdateStatus(sslServer, $"Greška u prekidu slušanja: {ex.Message}");
            }
        }

        #endregion
    }
}
