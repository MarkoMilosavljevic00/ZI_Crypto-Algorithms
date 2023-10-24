using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZIprojekat
{
    public class Bifid
    {

        private char[,] key = new char[5, 5];
        private int indexI;
        private int indexJ;
        private int period = 5;


        public Bifid()
        {
        }

        public string[] Encrypt1(string plaintext)
        {
            string[] val = new string[2];
            bool found = false;

            foreach (var item in plaintext)
            {
                if (item == ' ')
                {
                    val[0] += " ";
                    val[1] += " ";
                    continue;
                }

                if (!Char.IsLetter(item))
                    continue;

                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (item == 'j')
                        {
                            val[0] += (indexI + 1).ToString();
                            val[1] += (indexJ + 1).ToString();

                            found = true;
                            break;
                        }
                        else if (key[i, j] == item)
                        {
                            val[0] += (i + 1).ToString();
                            val[1] += (j + 1).ToString();

                            found = true;
                            break;
                        }
                    }
                    if (found)
                    {
                        found = false;
                        break;
                    }
                }
            }
            return val;
        }

        public void Encrypt2(string[] values)
        {
            string[] trimmedValues = new string[2];
            trimmedValues[0] = values[0].Replace(" ", "");
            trimmedValues[1] = values[1].Replace(" ", "");

            int length = trimmedValues[0].Length;
            int temp = length;
            values[0] = "";
            values[1] = "";
            int k = 0;
            while ((length - period) >= 0)
            {
                values[0] += trimmedValues[0].Substring(period * k, period);
                values[0] += " ";
                values[1] += trimmedValues[1].Substring(period * k, period);
                values[1] += " ";
                k++;
                length -= period;
            }
            values[0] += trimmedValues[0].Substring(temp - length);
            values[1] += trimmedValues[1].Substring(temp - length);
        }

        public string Encrypt3(string[] values)
        {
            string together = "";
            string[] r = values[0].Split(' ');
            string[] c = values[1].Split(' ');

            for (int i = 0; i < r.Length; i++)
                together += r[i] + c[i] + " ";
            return together;
        }

        public string Encrypt4(string newValue)
        {
            string ciphertext = "";

            for (int i = 0; i < newValue.Length - 2; i += 2)
            {
                if (newValue[i] == ' ')
                {
                    i++;
                }
                ciphertext += key[Int32.Parse(newValue[i].ToString()) - 1, Int32.Parse(newValue[i + 1].ToString()) - 1];
            }
            return ciphertext;
        }

        public string Decrypt1(string ciphertext)
        {
            string value = "";
            int l = ciphertext.Length;
            int tmp = l;
            int k = 0;
            while ((l - period) >= 0)
            {
                value += ciphertext.Substring(period * k, period);
                value += " ";
                k++;
                l -= period;
            }
            value += ciphertext.Substring(tmp - l);
            return value;
        }

        public string Decrypt2(string newValue)
        {
            string value = "";
            bool found = false;
            for (int k = 0; k < newValue.Length; k++)
            {
                if (newValue[k] == ' ')
                {
                    continue;
                }
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (newValue[k] == key[i, j])
                        {
                            value += (i + 1).ToString() + (j + 1).ToString();
                            found = true;
                            break;
                        }
                    }
                    if (found)
                    {
                        found = false;
                        break;
                    }
                }
            }
            return value;
        }

        public string[] Decrypt3(string value)
        {
            string[] values = new string[2];
            int l = value.Length;
            int tmp = l;
            int k = 0;

            while ((l - period * 2) >= 0)
            {
                values[0] += value.Substring(period * k, period);
                values[1] += value.Substring(period * (k + 1), period);
                k += 2;
                l -= period * 2;
            }
            values[0] += value.Substring(tmp - l, l / 2);
            values[1] += value.Substring(tmp - l / 2, l / 2);
            return values;
        }

        public string Decrypt4(string[] values)
        {
           string plaintext = "";
            for (int i = 0; i < values[0].Length; i++)
            {
                int r = Int32.Parse(values[0][i].ToString()) - 1;
                int c = Int32.Parse(values[1][i].ToString()) - 1;

                plaintext += key[r, c];
            }
            return plaintext;
        }

        public byte[] Encrypt(byte[] plaintext)
        {
            string[] values = Encrypt1(Encoding.Unicode.GetString(plaintext).ToLower());
            this.Encrypt2(values);
            string valuesTogether = Encrypt3(values);
            string ciphertext_str = Encrypt4(valuesTogether);

            byte[] ciphertext = Encoding.Unicode.GetBytes(ciphertext_str);
            return ciphertext;
        }

        public byte[] Decrypt(byte[] ciphertext)
        {
            string temp2 = Decrypt1(Encoding.Unicode.GetString(ciphertext));
            string temp3 = Decrypt2(temp2);
            string[] values = Decrypt3(temp3);
            string plaintext_str = Decrypt4(values);

            byte[] plaintext = Encoding.Unicode.GetBytes(plaintext_str);
            return plaintext;
        }

        public List<string> EncryptLines(List<string> plaintextLines)
        {
            List<string> encryptedLines = new List<string>();

            foreach (var plaintext in plaintextLines)
            {
                string[] values = Encrypt1(plaintext.ToLower());
                this.Encrypt2(values);
                string valuesTogether = Encrypt3(values);
                string ciphertext_str = Encrypt4(valuesTogether);
                encryptedLines.Add(ciphertext_str);
            }
            return encryptedLines;
        }

        public List<string> DecryptLines(List<string> ciphertextLines)
        {
            List<string> decryptedLines = new List<string>();
            foreach (string ciphertext in ciphertextLines)
            {
                string temp2 = Decrypt1(ciphertext);
                string temp3 = Decrypt2(temp2);
                string[] values = Decrypt3(temp3);
                string plaintext_str = Decrypt4(values);
                decryptedLines.Add(plaintext_str);
            }
            return decryptedLines;
        }

        public List<string> GenerateKey()
        {
            Random rand = new Random();
            List<string> strings = new List<string>();
            List<char> randomLetters = new List<char>();
            for (int i = 0; i < 5; i++)
            {
                string tmp = "";
                for (int j = 0; j < 5; j++)
                {
                    int charNumb = rand.Next(97, 123);
                    char letter = Convert.ToChar(charNumb);
                    if (letter == 'i' && !randomLetters.Contains(letter))
                    {
                        indexI = i;
                        indexJ = j;
                    }
                    if (letter == 'j' || randomLetters.Contains(letter))
                    {
                        j--;
                        continue;
                    }

                    randomLetters.Add(letter);
                    key[i, j] = letter;
                    tmp += letter;
                }
                strings.Add(tmp);
            }
            return strings;
        }
        public void LoadKey(string key)
        {
            int i = 0, j = 0;
            string[] keyLines = key.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string line in keyLines)
            {
                foreach (char c in line)
                {
                    if (c == 'i')
                    {
                        indexI = i;
                        indexJ = j;
                    }
                    this.key[i, j++] = c;
                }
                i++;
                j = 0;
            }
        }
    }
}
