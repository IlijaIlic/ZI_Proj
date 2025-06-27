using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZI_CRYPTER.Utils
{
    public static class A51
    {
        // koristimo uint a ne bool zbog povecanja performansi zbog bitwise operacija
        // zbir bitova svih registra je 64 sto bi moglo da stane u tip double


        public static uint[] R1 = new uint[19]; // bit za majority vote 8 // vrednost prvog bita xor bitova 13, 16, 17, 18
        public static uint[] R2 = new uint[22]; // bit za majority vote 10 // vrednost prvog bita xor bitova 20, 21
        public static uint[] R3 = new uint[23]; // bit za majority vote 10 // vrednost prvog bita xor bitova 7, 20, 21, 22

        public static int warmUpRounds = 100;

        public static void ClearRegisters()
        {
            for (int i = 0; i < 23; i++)
            {
                R3[i] = 0;
                if (i < 22)
                {
                    R2[i] = 0;
                    if (i < 19) R1[i] = 0;
                }
            }
        }

        public static void InitializeRegistersWithULongKey(ulong key)
        {
            ClearRegisters();

            uint keyBit;

            for (int i = 0; i < 64; i++)
            {
                keyBit = (uint)((key >> i) & 1);


                uint zeroBitR1 = ProduceFirstBitR1();
                uint zeroBitR2 = ProduceFirstBitR2();
                uint zeroBitR3 = ProduceFirstBitR3();

                ShiftR1(zeroBitR1);
                ShiftR2(zeroBitR2);
                ShiftR3(zeroBitR3);


                R1[0] ^= keyBit;
                R2[0] ^= keyBit;
                R3[0] ^= keyBit;
            }

            for (int i = 0; i < warmUpRounds; i++)
            {
                ClockForward();
            }
        }

        public static void InitializeRegistersWithKeyBytes(byte[] keyBytes)
        {
            ClearRegisters();

            foreach (byte b in keyBytes)
            {
                for (int i = 0; i < 8; i++)
                {
                    uint keyBit = (uint)((b >> i) & 1);

                    uint zeroBitR1 = ProduceFirstBitR1();
                    uint zeroBitR2 = ProduceFirstBitR2();
                    uint zeroBitR3 = ProduceFirstBitR3();

                    ShiftR1(zeroBitR1);
                    ShiftR2(zeroBitR2);
                    ShiftR3(zeroBitR3);

                    R1[0] ^= keyBit;
                    R2[0] ^= keyBit;
                    R3[0] ^= keyBit;
                }
            }

            for (int i = 0; i < warmUpRounds; i++)
            {
                ClockForward();
            }
        }


        public static void InitializeRegistersWithStringKey(string key)
        {
            for (int i = 0; i < 23; i++)
            {
                R3[i] = 0;
                if (i < 22)
                {
                    R2[i] = 0;
                    if (i < 19) R1[i] = 0;
                }
            }

            for (int i = 0; i < 64; i++)
            {

            }
        }

        public static void ShiftR1(uint nulti)
        {
            for (int i = 18; i > 0; i--)
            {
                R1[i] = R1[i - 1];
            }

            R1[0] = nulti;
        }

        public static void ShiftR2(uint nulti)
        {
            for (int i = 21; i > 0; i--)
            {
                R2[i] = R2[i - 1];
            }
            R2[0] = nulti;
        }

        public static void ShiftR3(uint nulti)
        {
            for (int i = 22; i > 0; i--)
            {
                R3[i] = R3[i - 1];
            }
            R3[0] = nulti;
        }

        public static uint ProduceFirstBitR1()
        {
            return R1[13] ^ R1[16] ^ R1[17] ^ R1[18]; ;
        }

        public static uint ProduceFirstBitR2()
        {
            return R2[20] ^ R2[21];
        }

        public static uint ProduceFirstBitR3()
        {
            return R3[7] ^ R3[20] ^ R3[21] ^ R3[22];
        }

        public static uint ProduceKeyBit()
        {
            return R1[18] ^ R2[21] ^ R3[22];
        }

        public static uint ClockForward()
        {
            uint majBit = (R1[8] + R2[10] + R3[10]) > 1 ? 1u : 0u;

            if (R1[8] == majBit)
            {
                uint bitR1 = ProduceFirstBitR1();
                ShiftR1(bitR1);
            }

            if (R2[10] == majBit)
            {
                uint bitR2 = ProduceFirstBitR2();
                ShiftR2(bitR2);
            }

            if (R3[10] == majBit)
            {
                uint bitR3 = ProduceFirstBitR3();
                ShiftR3(bitR3);
            }

            uint keyBit = ProduceKeyBit();

            return keyBit;
        }

        public static void useA51(string inputFilePath, string outputFilePath, byte[] key)
        {
            ValidateKeyA51(key);

            byte[] fileData = File.ReadAllBytes(inputFilePath);
            byte[] newData = new byte[fileData.Length];

            InitializeRegistersWithKeyBytes(key);

            for (int i = 0; i < fileData.Length; i++)
            {
                byte keystreamByte = 0;

                for (int bit = 0; bit < 8; bit++)
                {
                    uint keyBit = ClockForward(); 
                    keystreamByte |= (byte)(keyBit << bit);
                }

                newData[i] = (byte)(fileData[i] ^ keystreamByte);
            }

            File.WriteAllBytes(outputFilePath, newData);
        }


        public static void ValidateKeyA51(byte[] key)
        {
            // TODO
            if (key.Length > 8)
            {
                throw new Exception("Kljuc mora imati 64 bita.");
            }
            Console.WriteLine("TODO");
        }
    }
}
