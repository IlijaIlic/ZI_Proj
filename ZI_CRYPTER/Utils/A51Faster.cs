using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZI_CRYPTER.Utils
{
    public static class A51Faster
    {

        // Registers as bitfields (stored in lower bits of uints)
        public static uint R1; // 19-bit (bits 0-18)
        public static uint R2; // 22-bit (bits 0-21)
        public static uint R3; // 23-bit (bits 0-22)

        public static int warmUpRounds = 100;

        public static void ClearRegisters()
        {
            R1 = R2 = R3 = 0;
        }

        public static void InitializeRegistersWithKeyBytes(byte[] keyBytes)
        {
            ClearRegisters();

            foreach (byte b in keyBytes)
            {
                for (int i = 0; i < 8; i++)
                {
                    uint keyBit = (uint)((b >> i) & 1);

                    ClockR1(ProduceFirstBitR1());
                    ClockR2(ProduceFirstBitR2());
                    ClockR3(ProduceFirstBitR3());

                    R1 ^= keyBit;
                    R2 ^= keyBit;
                    R3 ^= keyBit;
                }
            }

            for (int i = 0; i < warmUpRounds; i++)
                ClockForward();
        }

        // Shifting via bitwise operations
        public static void ClockR1(uint newBit)
        {
            R1 = ((R1 << 1) | newBit) & 0x7FFFF; // 19 bits mask
        }

        public static void ClockR2(uint newBit)
        {
            R2 = ((R2 << 1) | newBit) & 0x3FFFFF; // 22 bits mask
        }

        public static void ClockR3(uint newBit)
        {
            R3 = ((R3 << 1) | newBit) & 0x7FFFFF; // 23 bits mask
        }

        // Tap positions for feedback
        public static uint ProduceFirstBitR1() =>
            ((R1 >> 13) ^ (R1 >> 16) ^ (R1 >> 17) ^ (R1 >> 18)) & 1;

        public static uint ProduceFirstBitR2() =>
            ((R2 >> 20) ^ (R2 >> 21)) & 1;

        public static uint ProduceFirstBitR3() =>
            ((R3 >> 7) ^ (R3 >> 20) ^ (R3 >> 21) ^ (R3 >> 22)) & 1;

        // Majority function bit
        public static uint ClockForward()
        {
            uint maj = Majority((R1 >> 8) & 1, (R2 >> 10) & 1, (R3 >> 10) & 1);

            if (((R1 >> 8) & 1) == maj)
                ClockR1(ProduceFirstBitR1());
            if (((R2 >> 10) & 1) == maj)
                ClockR2(ProduceFirstBitR2());
            if (((R3 >> 10) & 1) == maj)
                ClockR3(ProduceFirstBitR3());

            return ProduceKeyBit();
        }

        public static uint ProduceKeyBit() =>
            ((R1 >> 18) ^ (R2 >> 21) ^ (R3 >> 22)) & 1;

        public static uint Majority(uint a, uint b, uint c) =>
            (a + b + c) > 1 ? 1u : 0u;

        public static void useA51(string inputFilePath, string outputFilePath, byte[] key)
        {
            ValidateKeyA51(key);
            InitializeRegistersWithKeyBytes(key);


            using (FileStream fsInput = new FileStream(inputFilePath, FileMode.Open, FileAccess.Read))
            using (FileStream fsOutput = new FileStream(outputFilePath, FileMode.Create, FileAccess.Write))
            {
                int byteRead;
                while ((byteRead = fsInput.ReadByte()) != -1)
                {
                    byte keystreamByte = 0;

                    for (int bit = 0; bit < 8; bit++)
                        keystreamByte |= (byte)(ClockForward() << bit);

                    byte encryptedByte = (byte)(byteRead ^ keystreamByte);
                    fsOutput.WriteByte(encryptedByte);
                }
            }
        }

        public static void ValidateKeyA51(byte[] key)
        {
            if (key.Length > 8)
                throw new Exception("Kljuc mora imati 64 bita.");
        }
    }
}
