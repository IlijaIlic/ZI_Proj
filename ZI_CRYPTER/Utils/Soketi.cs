using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;

namespace ZI_CRYPTER.Utils
{
    public static class Soketi
    {


        public static async Task AdvanceKlijent(string ipAdresa, string port, string putanja, string InfoText)
        {
            try
            {
                using (Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
                {
                    await clientSocket.ConnectAsync(ipAdresa, Int32.Parse(port));

                    using (NetworkStream networkStream = new NetworkStream(clientSocket))
                    using (BinaryReader reader = new BinaryReader(networkStream))
                    using (BinaryWriter writer = new BinaryWriter(networkStream))
                    {
                        string filePath = putanja;
                        string fileName = Path.GetFileName(filePath);
                        long fileSize = new FileInfo(filePath).Length;

                        // Слање метаподатака (име фајла и величина)
                        writer.Write(fileName);
                        writer.Write(fileSize);

                        // Слање фајла у блоковима
                        using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                        {
                            byte[] buffer = new byte[4096];
                            int bytesRead;

                            while ((bytesRead = await fileStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                            {
                                await networkStream.WriteAsync(buffer, 0, bytesRead);
                            }
                        }

                        // Примање потврде са сервера
                        string response = reader.ReadString();
                        InfoText = $"Server response: {response}";
                        


                    }
                }
            }
            catch (Exception ex)
            {
                InfoText = $"Error: {ex.Message}";

            }
        }
        public static async Task AdvanceServer(string port, Socket serverSocket, string InfoText)
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                serverSocket.Bind(new IPEndPoint(IPAddress.Any, Int32.Parse(port)));
                serverSocket.Listen(5);
                InfoText = "Server je spreman i osluskuje konekcije!";


                while (true)
                {
                    Socket clientSocket = await serverSocket.AcceptAsync();
                    Task.Run(() => HandleClientAsyncAdvance(clientSocket, InfoText));
                }
            }
            catch (Exception ex)
            {
                InfoText = $"Error: {ex.Message}";


            }
            finally
            {
                serverSocket.Close();
            }
        }

        private static async Task HandleClientAsyncAdvance(Socket clientSocket, string InfoText)
        {
            try
            {
                using (NetworkStream networkStream = new NetworkStream(clientSocket))
                using (BinaryReader reader = new BinaryReader(networkStream))
                using (BinaryWriter writer = new BinaryWriter(networkStream))
                {
                    string fileName = reader.ReadString();
                    long fileSize = reader.ReadInt64();

                    InfoText = $"Preuzimanje fajla: {fileName} ({fileSize} bytes)";


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

                    InfoText = $"Fajl {fileName} uspešno preuzet.";

                    writer.Write("Fajl je uspešno preuzet.");
                }
            }
            catch (Exception ex)
            {
                InfoText = $"Error handling client: {ex.Message}";


            }
            finally
            {
                clientSocket.Close();
            }
        }


       
    }
}
