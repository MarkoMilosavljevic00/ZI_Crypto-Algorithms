using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ZIprojekat
{
    public class Knapsack
    {
        private long[] P;
        private long[] J;
        private long n;
        private long m;
        private long m_inverse;

        public Knapsack()
        {
            P = new long[16];
            J = new long[16];
        }

        public string Encrypt(string plaintext)
        {
            string ciphertext = "";
            byte[] data = Encoding.Unicode.GetBytes(plaintext.ToCharArray());
            BitArray B;
            for (int c = 0; c < data.Length - 1; c += 2)
            {
                long C = 0;
                int len;
                if (data[c + 1] == (byte)0)
                    len = 8;
                else
                    len = 16;
                B = new BitArray(new byte[] { data[c], data[c + 1] });
                for (int i = len - 1; i >= 0; i--)
                {
                    int b = B[i] ? 1 : 0;
                    if (b == 0)
                        continue;
                    C += J[i] * b;
                }
                ciphertext += C.ToString() + " ";
            }
            return ciphertext;
        }

        public string Decrypt(string ciphertext)
        {
            long TC;
            string plaintext = "";
            string[] chars = ciphertext.Split(' ');
            foreach (string C in chars)
            {
                if (C == "")
                    continue;
                TC = 0;
                TC = (Int32.Parse(C) * m_inverse) % n;
                if (TC < 0)
                    TC += n;

                List<bool> factors = new List<bool>();
                factors = Factoring(TC);
                BitArray bits = new BitArray(Enumerable.Reverse(factors).ToArray());
                byte[] b = new byte[2];
                bits.CopyTo(b, 0);
                char c = BitConverter.ToChar(b, 0);
                plaintext += c;
            }
            return plaintext;
        }

        public List<string> GenerateKeys()
        {
            List<string> output = new List<string>();
            string privateKey = "";
            string publicKey = "";
            int tmp = 0;
            Random r = new Random();
            for (int i = 0; i < 16; i++)
            {
                tmp += r.Next(tmp + 1, tmp + 3);
                P[i] = tmp;
                privateKey += P[i] + " ";
            }

            n = r.Next(tmp, tmp + 3); 

            tmp = r.Next(1, (int)n / 2);
            while (NZD(n, tmp) != 1)
                tmp = r.Next(1, (int)n / 2);
            m = tmp;

            for (int i = 0; i < 16; i++)
            {
                J[i] = (P[i] * m) % n;
                if (J[i] < 0)
                    J[i] += n;
                publicKey += J[i] + " ";
            }
            m_inverse = this.Inversing(m, n);

            output.Add(privateKey);
            output.Add(publicKey);
            output.Add(n.ToString());
            output.Add(m.ToString());
            output.Add(m_inverse.ToString());

            return output;
        }

        public void LoadKey(string keyInOneLine)
        {
            string[] keyLines = keyInOneLine.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            string[] privateKey = keyLines[0].Split(' ');
            int i = 0;
            foreach(var el in privateKey)
            {
                if (el == "")
                    continue;
                Int64.TryParse(el, out P[i]);
                i++;
            }
            
            string[] publicKey = keyLines[1].Split(' ');
            i = 0;
            foreach(var el in publicKey)
            {
                if (el == "")
                    continue;
                Int64.TryParse(el, out J[i]);
                i++;
            }

            Int64.TryParse(keyLines[2], out n);
            Int64.TryParse(keyLines[3], out m);
            Int64.TryParse(keyLines[4], out m_inverse);
        }

        private long NZD(long a, long b)
        {
            long temp;

            while (b != 0)
            {
                temp = a;
                a = b;
                b = temp % b;
            }

            return a;
        }

        private long Inversing(long m, long n)
        {
            long n0 = n;
            long y = 0;
            long x = 1;

            if (n == 1)
                return 0;

            while (m > 1)
            {
                long q = m / n;
                long t = n;
                n = m % n;
                m = t;
                t = y;

                y = x - q * y;
                x = t;
            }

            if (x < 0)
                x += n0;

            return x;
        }

        private List<bool> Factoring(long number)
        {
            List<bool> factors = new List<bool>();

            for (int i = 15; i >= 0; i--)
            {
                if (number - P[i] >= 0)
                {
                    factors.Add(true);
                    number -= P[i];
                }
                else
                    factors.Add(false);
            }

            return factors;
        }
    }
}