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
            byte[] encryptedData = Encrypt(fileData, key);

            File.WriteAllBytes(outputFilePath, encryptedData);
        }

        public static void DecryptFile(string inputFilePath, string outputFilePath, byte[] key)
        {
            ValidateKey(key);

            byte[] fileData = File.ReadAllBytes(inputFilePath);
            byte[] decryptedData = Decrypt(fileData, key);

            File.WriteAllBytes(outputFilePath, decryptedData);
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
            byte[] data = File.ReadAllBytes(inputFilePath);
            if (iv.Length != 8) throw new ArgumentException("IV must be 8 bytes.");

            byte[] encryptedData = new byte[data.Length];
            byte[] feedback = new byte[8];
            Array.Copy(iv, feedback, 8);

            int numBlocks = (data.Length + 7) / 8;

            for (int i = 0; i < numBlocks; i++)
            {
                uint v0 = BitConverter.ToUInt32(feedback, 0);
                uint v1 = BitConverter.ToUInt32(feedback, 4);
                EncryptBlock(ref v0, ref v1, key);

                Array.Copy(BitConverter.GetBytes(v0), 0, feedback, 0, 4);
                Array.Copy(BitConverter.GetBytes(v1), 0, feedback, 4, 4);

                int blockSize = Math.Min(8, data.Length - (i * 8));
                for (int j = 0; j < blockSize; j++)
                {
                    encryptedData[i * 8 + j] = (byte)(data[i * 8 + j] ^ feedback[j]);
                }
            }

            File.WriteAllBytes(outputFilePath, encryptedData);
        }
    }
}