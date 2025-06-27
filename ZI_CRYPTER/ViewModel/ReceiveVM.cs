using Cryptography;
using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
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


        public ICommand ChangeReceiveLocationCommand { get; set; }
        public ICommand ListenCommand { get; set; }

        public ReceiveVM(ViewModelBase vmb)
        {
            _pageModel = new PageModel();
            _vmBase = vmb;

            ListenCommand = new RelayCommand(async (param) => await Osluskuj(param));
            ChangeReceiveLocationCommand = new RelayCommand(ChangeReceiveLocation);
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

        public string ReceiveOutput
        {
            get => _vmBase.SharedReceiveOutput;
            set
            {
                _vmBase.SharedReceiveOutput = value;
                OnProprtyChanged(nameof(SharedReceiveOutput));
            }
        }

        // OTVORI FOLDER KAD PRIMI FAJL
        private async Task Osluskuj(object sender)
        {
            if (ReceiveChecked)
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
                    InfoTextRec = "Server je zaustavljen.";
                    OnProprtyChanged(nameof(InfoTextRec));
                }
                catch (Exception ex)
                {
                    InfoTextRec = $"Greška u prekidu slušanja: {ex.Message}";
                    OnProprtyChanged(nameof(InfoTextRec));
                }
            }

        }

        private void ChangeReceiveLocation(object parameter)
        {
            var folderDialog = new OpenFolderDialog
            {

            };

            if (folderDialog.ShowDialog() == true)
            {
                var folderName = folderDialog.FolderName;
                ReceiveOutput = folderName;
                OnProprtyChanged(nameof(ReceiveOutput));

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
                    int hashLength = reader.ReadInt32();
                    byte[] hash = reader.ReadBytes(hashLength);

                    InfoTextRec = $"Preuzimanje fajla: {fileName} ({fileSize} bajta)";
                    OnProprtyChanged(nameof(InfoTextRec));


                    string savePath = Path.Combine(ReceiveOutput, "Primljeno - kodirano - " + fileName);
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

                    //InfoTextRec = $"Fajl {fileName} uspešno preuzet.";
                    InfoTextRec = savePath;
                    OnProprtyChanged(nameof(InfoTextRec));
                    writer.Write("Fajl je uspešno preuzet.");

                    byte[] noviHash = BLAKE.ComputeHash(Path.Combine(ReceiveOutput, "Primljeno - kodirano - " + fileName));

                    if (!noviHash.SequenceEqual(hash))
                    {
                        App.Current.Dispatcher.Invoke(() =>
                        {
                            WindowInfoAlert wia = new WindowInfoAlert("Blake hash error");
                            wia.ShowDialog();
                        });
                        return;
                    }

                    byte[] keyByte = Encoding.ASCII.GetBytes(ReceiveKey);
                    if (ReceiveAlg == "XTEA" && ReceiveKey != "")
                    {
                        XTEA.DecryptFile(Path.Combine(ReceiveOutput, "Primljeno - kodirano - " + fileName), Path.Combine(ReceiveOutput, "Primljeno - dekodirano - " + fileName), keyByte);
                    }
                    else if (ReceiveAlg == "A5/1" && ReceiveKey != "")
                    {
                        A51Faster.useA51((Path.Combine(ReceiveOutput, "Primljeno - kodirano - " + fileName)), Path.Combine(ReceiveOutput, "Primljeno - dekodirano - " + fileName), keyByte);
                    }
                    else if (ReceiveAlg == "XTEA + OFB" && ReceiveKey != "")
                    {
                        XTEA.OFB((Path.Combine(ReceiveOutput, "Primljeno - kodirano - " + fileName)), Path.Combine(ReceiveOutput, "Primljeno - dekodirano - " + fileName), keyByte, Encoding.ASCII.GetBytes("asdfasdf"));
                    }

                }
            }
            catch (Exception ex)
            {
                if (ex is UnauthorizedAccessException)
                {
                    WindowInfoAlert wia = new WindowInfoAlert("Za zeljeni output direktorijum su potrebne privilegije administratora!");
                    wia.ShowDialog();
                }
                else
                {
                    InfoTextRec = $"Greska: {ex.Message}";
                    OnProprtyChanged(nameof(InfoTextRec));
                }
            }
            finally
            {
                clientSocket.Close();
            }
        }
    }
}
