using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ZIprojekat
{
    public class RC6
    {
        private const int numOfRounds = 20; 
        private const int w = 32; 
        private uint[] roundKey = new uint[2 * numOfRounds + 4];  
        private byte[] mainKey; 

        private const uint P = 0xB7E15163;
        private const uint Q = 0x9E3779B9;

        public RC6(byte[] key)
        {
            GenerateKey(key);
        }

        public RC6()
        {
        }
        public void GenerateKey(byte[] key)
        {
            mainKey = key;
            int c = 0;
            int i, j;
            c = key.Length / 4;
            uint[] L = new uint[c];
            for (i = 0; i < c; i++)
            {
                L[i] = BitConverter.ToUInt32(mainKey, i * 4); 
            }
            roundKey[0] = P;
            for (i = 1; i < 2 * numOfRounds + 4; i++)
                roundKey[i] = roundKey[i - 1] + Q;
            uint A, B; 
            A = B = 0;
            i = j = 0;
            int V = 3 * Math.Max(c, 2 * numOfRounds + 4); 
            for (int s = 1; s <= V; s++)
            {
                A = roundKey[i] = LeftShift((roundKey[i] + A + B), 3);
                B = L[j] = LeftShift((L[j] + A + B), (int)(A + B));
                i = (i + 1) % (2 * numOfRounds + 4);
                j = (j + 1) % c;
            }
        }

        public byte[] Encrypt(byte[] plaintext)
        {
            uint A, B, C, D;
            int i = plaintext.Length;
            while (i % 16 != 0)
                i++;

            byte[] text = new byte[i];
            plaintext.CopyTo(text, 0);
            byte[] ciphertext = new byte[i];

            for (i = 0; i < text.Length; i = i + 16)
            {
                A = BitConverter.ToUInt32(text, i);
                B = BitConverter.ToUInt32(text, i + 4);
                C = BitConverter.ToUInt32(text, i + 8);
                D = BitConverter.ToUInt32(text, i + 12);
                B = B + roundKey[0];
                D = D + roundKey[1];
                for (int j = 1; j <= numOfRounds; j++)
                {
                    uint t = LeftShift((B * (2 * B + 1)), (int)(Math.Log(w, 2)));
                    uint u = LeftShift((D * (2 * D + 1)), (int)(Math.Log(w, 2)));
                    A = (LeftShift((A ^ t), (int)u)) + roundKey[j * 2];
                    C = (LeftShift((C ^ u), (int)t)) + roundKey[j * 2 + 1];
                    uint temp = A;
                    A = B;
                    B = C;
                    C = D;
                    D = temp;
                }

                A = A + roundKey[2 * numOfRounds + 2];
                C = C + roundKey[2 * numOfRounds + 3];
                uint[] tmps = new uint[4] { A, B, C, D };
                byte[] block = ToArrayBytes(tmps, 4);
                block.CopyTo(ciphertext, i);
            }
            return ciphertext;
        }
        public byte[] Decrypt(byte[] ciphertext)
        {
            uint A, B, C, D;
            int i;
            byte[] plainText = new byte[ciphertext.Length];

            for (i = 0; i < ciphertext.Length; i = i + 16)
            {
                A = BitConverter.ToUInt32(ciphertext, i);
                B = BitConverter.ToUInt32(ciphertext, i + 4);
                C = BitConverter.ToUInt32(ciphertext, i + 8);
                D = BitConverter.ToUInt32(ciphertext, i + 12);
                C = C - roundKey[2 * numOfRounds + 3];
                A = A - roundKey[2 * numOfRounds + 2];
                for (int j = numOfRounds; j >= 1; j--)
                {
                    uint tmp = D;
                    D = C;
                    C = B;
                    B = A;
                    A = tmp;
                    uint u = LeftShift((D * (2 * D + 1)), (int)Math.Log(w, 2));
                    uint t = LeftShift((B * (2 * B + 1)), (int)Math.Log(w, 2));
                    C = RightShift((C - roundKey[2 * j + 1]), (int)t) ^ u;
                    A = RightShift((A - roundKey[2 * j]), (int)u) ^ t;
                }
                D = D - roundKey[1];
                B = B - roundKey[0];
                uint[] tmps = new uint[4] { A, B, C, D };
                byte[] block = ToArrayBytes(tmps, 4);
                block.CopyTo(plainText, i);
            }
            return plainText;
        }

        public uint RightShift(uint value, int shift)
        {
            return (value >> shift) | (value << (w - shift));
        }

        public uint LeftShift(uint value, int shift)
        {
            return (value << shift) | (value >> (w - shift));
        }

        public byte[] ToArrayBytes(uint[] uints, int Long)
        {
            byte[] array = new byte[Long * 4];
            for (int i = 0; i < Long; i++)
            {
                byte[] tmp = BitConverter.GetBytes(uints[i]);
                tmp.CopyTo(array, i * 4);
            }
            return array;
        }
    }
}
