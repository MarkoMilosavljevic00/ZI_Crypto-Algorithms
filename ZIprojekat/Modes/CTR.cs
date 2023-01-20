using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZIprojekat
{
    class CTR 
    {
        private readonly RC6 rc6;
        private ulong counter;
        private readonly byte[] nonceAndCounter;

        public CTR(RC6 rc6)
        {
            this.rc6 = rc6;
            nonceAndCounter = new byte[16];
        }

        public void SetNonce(string nonce)
        {
            //rc6 = new RC6();
            if (nonce.Length > 4) nonce = nonce.Substring(0, 4);
            counter = 0;
            //nonceAndCounter = new byte[16];
            Array.Copy(Encoding.Unicode.GetBytes(nonce), nonceAndCounter, 8);
        }

        public byte[] EncryptRC6(byte[] data, string key)
        {
            rc6.GenerateKey(Encoding.UTF8.GetBytes(key));
            byte[] encrypted = new byte[data.Length];
            for (int i = 0; i < data.Length; i += 16)
            {
                Array.Copy(BitConverter.GetBytes(counter), 0, nonceAndCounter, 8, 8);
                counter++;

                byte[] encryptedNonceAndCounter = rc6.Encrypt(nonceAndCounter);

                for (int j = 0; j < 16 && (i + j) < data.Length; j++)
                    encrypted[i + j] = (byte)(data[i + j] ^ encryptedNonceAndCounter[j]);
            }

            counter = 0;
            return encrypted;
        }

        public byte[] DecryptRC6(byte[] data, string key)
        {
            return EncryptRC6(data, key);
        }
    }
}
