using System;
using System.Text;
using System.IO;

namespace Cryptography
{
    public class BLAKE
    {
        // Initialization Vector (IV) for BLAKE-256 (from fractional parts of sqrt of primes)
        private static readonly uint[] IV = new uint[]
        {
            0x6A09E667, 0xBB67AE85, 0x3C6EF372, 0xA54FF53A,
            0x510E527F, 0x9B05688C, 0x1F83D9AB, 0x5BE0CD19
        };

        // Constants from the fractional part of pi
        private static readonly uint[] Constants = new uint[]
        {
            0x243F6A88, 0x85A308D3, 0x13198A2E, 0x03707344,
            0xA4093822, 0x299F31D0, 0x082EFA98, 0xEC4E6C89,
            0x452821E6, 0x38D01377, 0xBE5466CF, 0x34E90C6C,
            0xC0AC29B7, 0xC97C50DD, 0x3F84D5B5, 0xB5470917
        };

        // Sigma permutation table
        private static readonly byte[,] Sigma = new byte[10, 16]
        {
            {  0, 1, 2, 3, 4, 5, 6, 7, 8, 9,10,11,12,13,14,15 },
            { 14,10, 4, 8, 9,15,13, 6, 1,12, 0, 2,11, 7, 5, 3 },
            { 11, 8,12, 0, 5, 2,15,13,10,14, 3, 6, 7, 1, 9, 4 },
            {  7, 9, 3, 1,13,12,11,14, 2, 6, 5,10, 4, 0,15, 8 },
            {  9, 0, 5, 7, 2, 4,10,15,14, 1,11,12, 6, 8, 3,13 },
            {  2,12, 6,10, 0,11, 8, 3, 4,13, 7, 5,15,14, 1, 9 },
            { 12, 5, 1,15,14,13, 4,10, 0, 7, 6, 3, 9, 2, 8,11 },
            { 13,11, 7,14,12, 1, 3, 9, 5, 0,15, 4, 8, 6, 2,10 },
            {  6,15,14, 9,11, 3, 0, 8,12, 2,13, 7, 1, 4,10, 5 },
            { 10, 2, 8, 4, 7, 6, 1, 5,15,11, 9,14, 3,12,13, 0 }
        };

        // Rotate right
        private static uint RotR(uint x, int n)
        {
            return (x >> n) | (x << (32 - n));
        }

        // Compression function for 512-bit block
        private static void Compress(ref uint[] h, byte[] block, ulong messageBitLength, uint[] salt = null)
        {
            uint[] m = new uint[16];
            for (int i = 0; i < 16; i++)
                m[i] = BitConverter.ToUInt32(block, i * 4);

            uint[] v = new uint[16];
            Array.Copy(h, v, 8);
            Array.Copy(IV, 0, v, 8, 8);

            // XOR salt into v[8..11] if supplied
            if (salt != null)
            {
                v[8] ^= salt[0];
                v[9] ^= salt[1];
                v[10] ^= salt[2];
                v[11] ^= salt[3];
            }

            // XOR message bit length into v[12], v[13]
            v[12] ^= (uint)(messageBitLength & 0xFFFFFFFF);
            v[13] ^= (uint)(messageBitLength >> 32);

            for (int round = 0; round < 14; round++)
            {
                for (int i = 0; i < 8; i++)
                {
                    int j = Sigma[round % 10, 2 * i];
                    int k = Sigma[round % 10, 2 * i + 1];
                    G(ref v, i, m[j], m[k]);
                }
            }

            for (int i = 0; i < 8; i++)
                h[i] ^= v[i] ^ v[i + 8];
        }

        // G function: the core mixing operation
        private static void G(ref uint[] v, int i, uint m_j, uint m_k)
        {
            int a, b, c, d;
            if (i < 4)
            {
                a = i;
                b = i + 4;
                c = i + 8;
                d = i + 12;
            }
            else
            {
                int diag = i - 4;
                a = diag;
                b = diag + 4;
                c = diag + 8;
                d = diag + 12;
            }

            v[a] = v[a] + v[b] + (m_j ^ Constants[(i * 2) % 16]);
            v[d] = RotR(v[d] ^ v[a], 16);
            v[c] = v[c] + v[d];
            v[b] = RotR(v[b] ^ v[c], 12);
            v[a] = v[a] + v[b] + (m_k ^ Constants[(i * 2 + 1) % 16]);
            v[d] = RotR(v[d] ^ v[a], 8);
            v[c] = v[c] + v[d];
            v[b] = RotR(v[b] ^ v[c], 7);
        }

        // Hash function entry point
        public static byte[] ComputeHash(string inputFilePath, uint[] salt = null)
        {
            byte[] input = File.ReadAllBytes(inputFilePath);
            uint[] h = new uint[8];
            Array.Copy(IV, h, 8);

            ulong messageBitLength = (ulong)input.Length * 8;
            int blockCount = (input.Length + 1 + 8 + 63) / 64;

            byte[] padded = new byte[blockCount * 64];
            Array.Copy(input, padded, input.Length);

            // Append padding: 0x80, then zero bytes, then length
            padded[input.Length] = 0x80;

            byte[] lengthBytes = BitConverter.GetBytes(messageBitLength);
            Array.Copy(lengthBytes, 0, padded, padded.Length - 8, 8);

            // Process blocks
            for (int i = 0; i < blockCount; i++)
            {
                byte[] block = new byte[64];
                Array.Copy(padded, i * 64, block, 0, 64);
                Compress(ref h, block, messageBitLength, salt);
            }

            // Final digest
            byte[] digest = new byte[32];
            for (int i = 0; i < 8; i++)
                Array.Copy(BitConverter.GetBytes(h[i]), 0, digest, i * 4, 4);

            return digest;
        }

        // Helper to get hex string
        //public static string HashToHexString(string message)
        //{
        //    byte[] hash = ComputeHash(Encoding.UTF8.GetBytes(message));
        //    StringBuilder sb = new StringBuilder();
        //    foreach (byte b in hash)
        //        sb.Append(b.ToString("x2"));
        //    return sb.ToString();
        //}
    }
}
