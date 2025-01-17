using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
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

            ListenCommand = new RelayCommand(async (param)=>await Osluskuj(param));
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


        private async Task Osluskuj(object sender)
        {
            if(ReceiveChecked)
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                try
                {
                    serverSocket.Bind(new IPEndPoint(IPAddress.Any, Int32.Parse(ReceivePort)));
                    serverSocket.Listen(5);
                    InfoTextRec = "Server je spreman i osluskuje konekcije!";
                    OnProprtyChanged(nameof(InfoTextRec));



                    while (true)
                    {
                        Socket clientSocket = await serverSocket.AcceptAsync();
                        Task.Run(() => HandleClientAsyncAdvance(clientSocket));
                    }
                }
                catch (Exception ex)
                {
                    InfoTextRec = $"Error: {ex.Message}";
                    OnProprtyChanged(nameof(InfoTextRec));


                }
                finally
                {
                    serverSocket.Close();
                }
            }
            else
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
            
        }


       

        private async Task HandleClientAsyncAdvance(Socket clientSocket)
        {
            try
            {
                using (NetworkStream networkStream = new NetworkStream(clientSocket))
                using (BinaryReader reader = new BinaryReader(networkStream))
                using (BinaryWriter writer = new BinaryWriter(networkStream))
                {
                    string fileName = reader.ReadString();
                    long fileSize = reader.ReadInt64();

                    InfoTextRec = $"Preuzimanje fajla: {fileName} ({fileSize} bytes)";
                    OnProprtyChanged(nameof(InfoTextRec));


                    string savePath = Path.Combine(Directory.GetCurrentDirectory(), "Received_" + fileName);
                    using (FileStream fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                    {
                        byte[] buffer = new byte[4096];
                        long totalBytesReceived = 0;

                        while (totalBytesReceived < fileSize)
                        {
                            int bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length);
                            if (bytesRead == 0) break;

                            await fileStream.WriteAsync(buffer, 0, bytesRead);
                            totalBytesReceived += bytesRead;
                        }
                    }

                    InfoTextRec = $"Fajl {fileName} uspešno preuzet.";
                    OnProprtyChanged(nameof(InfoTextRec));

                    writer.Write("Fajl je uspešno preuzet.");
                }
            }
            catch (Exception ex)
            {
                InfoTextRec = $"Error handling client: {ex.Message}";
                OnProprtyChanged(nameof(InfoTextRec));


            }
            finally
            {
                clientSocket.Close();
            }
        }
    }
}
