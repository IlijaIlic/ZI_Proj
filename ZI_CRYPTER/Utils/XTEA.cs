using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
namespace ZI_CRYPTER.Utils
{
    public static class XTEA
    {

        private const int NumRounds = 64;

        public static void EncryptFile(string inputFilePath, string outputFilePath, byte[] key)
        {
            ValidateKey(key);

            byte[] fileData = File.ReadAllBytes(inputFilePath);
            byte[] encryptedData = EncryptParallel(fileData, key);

            File.WriteAllBytes(outputFilePath, encryptedData);
        }

        public static void DecryptFile(string inputFilePath, string outputFilePath, byte[] key)
        {
            ValidateKey(key);

            byte[] fileData = File.ReadAllBytes(inputFilePath);
            byte[] decryptedData = DecryptParallel(fileData, key);

            File.WriteAllBytes(outputFilePath, decryptedData);
        }

        public static void EncryptFileParallelBuffered(string inputFilePath, string outputFilePath, byte[] key)
        {
            ValidateKey(key);

            const int bufferSize = 64 * 1024; 
            byte[] buffer = new byte[bufferSize];
            int bytesRead;

            using (FileStream fsInput = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            using (FileStream fsOutput = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            {
                while ((bytesRead = fsInput.Read(buffer, 0, bufferSize)) > 0)
                {
                    int blockSize = 8;
                    int totalBlocks = bytesRead / blockSize;

                   
                    Parallel.For(0, totalBlocks, i =>
                    {
                        int offset = i * blockSize;
                        uint v0 = BitConverter.ToUInt32(buffer, offset);
                        uint v1 = BitConverter.ToUInt32(buffer, offset + 4);

                        EncryptBlock(ref v0, ref v1, key);

                        Array.Copy(BitConverter.GetBytes(v0), 0, buffer, offset, 4);
                        Array.Copy(BitConverter.GetBytes(v1), 0, buffer, offset + 4, 4);
                    });

                    fsOutput.Write(buffer, 0, bytesRead);
                }
            }
        }

        public static void DecryptFileParallelBuffered(string inputFilePath, string outputFilePath, byte[] key)
        {
            ValidateKey(key);

            const int bufferSize = 64 * 1024; 
            byte[] buffer = new byte[bufferSize];
            int bytesRead;

            using (FileStream fsInput = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            using (FileStream fsOutput = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            {
                while ((bytesRead = fsInput.Read(buffer, 0, bufferSize)) > 0)
                {
                    int blockSize = 8;
                    int totalBlocks = bytesRead / blockSize;

                    Parallel.For(0, totalBlocks, i =>
                    {
                        int offset = i * blockSize;
                        uint v0 = BitConverter.ToUInt32(buffer, offset);
                        uint v1 = BitConverter.ToUInt32(buffer, offset + 4);

                        DecryptBlock(ref v0, ref v1, key);

                        Array.Copy(BitConverter.GetBytes(v0), 0, buffer, offset, 4);
                        Array.Copy(BitConverter.GetBytes(v1), 0, buffer, offset + 4, 4);
                    });

                    fsOutput.Write(buffer, 0, bytesRead);
                }
            }
        }


        private static byte[] Encrypt(byte[] data, byte[] key)
        {
            ValidateKey(key);

            int blockSize = 8;
            int paddedSize = ((data.Length + blockSize - 1) / blockSize) * blockSize;
            byte[] paddedData = new byte[paddedSize];
            Array.Copy(data, paddedData, data.Length);


            for (int i = 0; i < paddedData.Length; i += blockSize)
            {
                uint v0 = BitConverter.ToUInt32(paddedData, i);
                uint v1 = BitConverter.ToUInt32(paddedData, i + 4);
                EncryptBlock(ref v0, ref v1, key);
                Array.Copy(BitConverter.GetBytes(v0), 0, paddedData, i, 4);
                Array.Copy(BitConverter.GetBytes(v1), 0, paddedData, i + 4, 4);
            }

            return paddedData;
        }

        private static byte[] EncryptParallel(byte[] data, byte[] key)
        {
            ValidateKey(key);

            int blockSize = 8;
            int paddedSize = ((data.Length + blockSize - 1) / blockSize) * blockSize;
            byte[] paddedData = new byte[paddedSize];
            Array.Copy(data, paddedData, data.Length);

            int chunkSize = 64 * 1024; // 64KB
            int totalChunks = (paddedSize + chunkSize - 1) / chunkSize;

            Parallel.For(0, totalChunks, c =>
            {
                int chunkStart = c * chunkSize;
                int chunkEnd = Math.Min(chunkStart + chunkSize, paddedSize);

                for (int offset = chunkStart; offset < chunkEnd; offset += blockSize)
                {
                    uint v0 = BitConverter.ToUInt32(paddedData, offset);
                    uint v1 = BitConverter.ToUInt32(paddedData, offset + 4);

                    EncryptBlock(ref v0, ref v1, key);

                    Array.Copy(BitConverter.GetBytes(v0), 0, paddedData, offset, 4);
                    Array.Copy(BitConverter.GetBytes(v1), 0, paddedData, offset + 4, 4);
                }
            });

            return paddedData;
        }


        private static byte[] Decrypt(byte[] data, byte[] key)
        {
            ValidateKey(key);

            int blockSize = 8; // XTEA works on 64-bit blocks (8 bytes)
            byte[] output = new byte[data.Length];

            for (int i = 0; i < data.Length; i += blockSize)
            {
                uint v0 = BitConverter.ToUInt32(data, i);
                uint v1 = BitConverter.ToUInt32(data, i + 4);
                DecryptBlock(ref v0, ref v1, key);
                Array.Copy(BitConverter.GetBytes(v0), 0, output, i, 4);
                Array.Copy(BitConverter.GetBytes(v1), 0, output, i + 4, 4);
            }

            return output;
        }

        private static byte[] DecryptParallel(byte[] data, byte[] key)
        {
            ValidateKey(key);

            int blockSize = 8;
            byte[] output = new byte[data.Length];

            int chunkSize = 64 * 1024; // 64KB per chunk
            int totalChunks = (data.Length + chunkSize - 1) / chunkSize;

            Parallel.For(0, totalChunks, c =>
            {
                int chunkStart = c * chunkSize;
                int chunkEnd = Math.Min(chunkStart + chunkSize, data.Length);

                for (int offset = chunkStart; offset < chunkEnd; offset += blockSize)
                {
                    uint v0 = BitConverter.ToUInt32(data, offset);
                    uint v1 = BitConverter.ToUInt32(data, offset + 4);

                    DecryptBlock(ref v0, ref v1, key);

                    Array.Copy(BitConverter.GetBytes(v0), 0, output, offset, 4);
                    Array.Copy(BitConverter.GetBytes(v1), 0, output, offset + 4, 4);
                }
            });

            return output;
        }

        private static void EncryptBlock(ref uint v0, ref uint v1, byte[] key)
        {
            uint[] k = ConvertKeyToUInt32Array(key);
            uint sum = 0;
            uint delta = 0x9E3779B9;

            for (int i = 0; i < NumRounds; i++)
            {
                v0 += (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + k[sum & 3]);
                sum += delta;
                v1 += (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + k[(sum >> 11) & 3]);
            }
        }

        private static void DecryptBlock(ref uint v0, ref uint v1, byte[] key)
        {
            uint[] k = ConvertKeyToUInt32Array(key);
            uint delta = 0x9E3779B9;
            uint sum = delta * (uint)NumRounds;

            for (int i = 0; i < NumRounds; i++)
            {
                v1 -= (((v0 << 4) ^ (v0 >> 5)) + v0) ^ (sum + k[(sum >> 11) & 3]);
                sum -= delta;
                v0 -= (((v1 << 4) ^ (v1 >> 5)) + v1) ^ (sum + k[sum & 3]);
            }
        }

        private static uint[] ConvertKeyToUInt32Array(byte[] key)
        {
            uint[] k = new uint[4];
            for (int i = 0; i < 4; i++)
            {
                k[i] = BitConverter.ToUInt32(key, i * 4);
            }
            return k;
        }

        private static void ValidateKey(byte[] key)
        {
            if (key.Length != 16)
            {
                throw new ArgumentException("Kljuc mora imati 128 bita.");
            }
        }

        public static void OFB(string inputFilePath, string outputFilePath, byte[] key, byte[] iv)
        {
            ValidateKey(key);
            if (iv.Length != 8)
                throw new ArgumentException("IV must be 8 bytes.");

            const int bufferSize = 64 * 1024; 
            byte[] buffer = new byte[bufferSize];
            int bytesRead;

            byte[] feedback = new byte[8];
            Array.Copy(iv, feedback, 8);

            using (FileStream fsInput = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            using (FileStream fsOutput = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            {
                while ((bytesRead = fsInput.Read(buffer, 0, bufferSize)) > 0)
                {
                    int offset = 0;
                    while (offset < bytesRead)
                    {
                        uint v0 = BitConverter.ToUInt32(feedback, 0);
                        uint v1 = BitConverter.ToUInt32(feedback, 4);
                        EncryptBlock(ref v0, ref v1, key);

                        byte[] keystream = new byte[8];
                        Array.Copy(BitConverter.GetBytes(v0), 0, keystream, 0, 4);
                        Array.Copy(BitConverter.GetBytes(v1), 0, keystream, 4, 4);

                        int blockSize = Math.Min(8, bytesRead - offset);

                        for (int i = 0; i < blockSize; i++)
                            buffer[offset + i] ^= keystream[i];

                        Array.Copy(keystream, feedback, 8);

                        offset += blockSize;
                    }

                    fsOutput.Write(buffer, 0, bytesRead);
                }
            }
        }
    }
}