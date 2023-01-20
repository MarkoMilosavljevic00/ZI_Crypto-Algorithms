using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ZIprojekat
{
    public class Bifid
    {

        private char[,] keySquare = new char[5, 5];
        private int period = 5;
        private int iIndexI;
        private int iIndexJ;


        public Bifid()
        {
        }

        public void stepOneEncrypt(string plaintext, out string[] values)
        {
            values = new string[2]; //values[0] - row, values[1] - col
            bool foundLetter = false;

            foreach (var item in plaintext)
            {
                if (item == ' ')
                {
                    values[0] += " ";
                    values[1] += " ";
                    continue;
                }

                if (!Char.IsLetter(item)) //ignorisanje bilo kog drugog znaka ili broja
                    continue;

                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 5; j++)
                    {
                        if (item == 'j')
                        {
                            values[0] += (iIndexI + 1).ToString();
                            values[1] += (iIndexJ + 1).ToString();

                            foundLetter = true;
                            break;
                        }
                        else if (keySquare[i, j] == item)
                        {
                            values[0] += (i + 1).ToString();
                            values[1] += (j + 1).ToString();

                            foundLetter = true;
                            break;
                        }
                    }
                    if (foundLetter)
                    {
                        foundLetter = false;
                        break;
                    }
                }
            }
        }

        public void stepTwoEncrypt(string[] values)
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

        public void stepThreeEncrypt(string[] values, out string newValue)
        {
            newValue = "";
            string[] rows = values[0].Split(' ');
            string[] cols = values[1].Split(' ');

            for (int i = 0; i < rows.Length; i++)
                newValue += rows[i] + cols[i] + " ";
        }

        public void stepFourEncrypt(string newValue, out string encryptedPlaintext)
        {
            encryptedPlaintext = "";

            for (int i = 0; i < newValue.Length - 2; i += 2)
            {
                if (newValue[i] == ' ')
                {
                    i++;
                }
                encryptedPlaintext += keySquare[Int32.Parse(newValue[i].ToString()) - 1, Int32.Parse(newValue[i + 1].ToString()) - 1];
            }
        }

        public void stepOneDecrypt(string encryptedPlaintext, out string newValue)
        {
            newValue = "";
            int length = encryptedPlaintext.Length;
            int temp = length;
            int k = 0;
            while ((length - period) >= 0)
            {
                newValue += encryptedPlaintext.Substring(period * k, period);
                newValue += " ";
                k++;
                length -= period;
            }
            newValue += encryptedPlaintext.Substring(temp - length);
        }

        public void stepTwoDecrypt(string newValue, out string value)
        {
            value = "";
            bool foundLetter = false;

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
                        if (newValue[k] == keySquare[i, j])
                        {
                            value += (i + 1).ToString() + (j + 1).ToString();
                            foundLetter = true;
                            break;
                        }
                    }
                    if (foundLetter)
                    {
                        foundLetter = false;
                        break;
                    }
                }
            }
        }

        public void stepThreeDecrypt(string value, out string[] values)
        {
            values = new string[2];
            int length = value.Length;
            int temp = length;
            int k = 0;

            while ((length - period * 2) >= 0)
            {
                values[0] += value.Substring(period * k, period);
                values[1] += value.Substring(period * (k + 1), period);
                k += 2;
                length -= period * 2;
            }
            values[0] += value.Substring(temp - length, length / 2);
            values[1] += value.Substring(temp - length / 2, length / 2);
        }

        public void stepFourDecrypt(string[] values, out string plaintext)
        {
            plaintext = "";

            for (int i = 0; i < values[0].Length; i++)
            {
                int row = Int32.Parse(values[0][i].ToString()) - 1;
                int col = Int32.Parse(values[1][i].ToString()) - 1;

                plaintext += keySquare[row, col];
            }
        }

        public byte[] Encrypt(byte[] plaintext)
        {
            string[] rcValues;
            string rcTogether;
            string temp;

            this.stepOneEncrypt(Encoding.Unicode.GetString(plaintext).ToLower(), out rcValues);
            this.stepTwoEncrypt(rcValues);
            this.stepThreeEncrypt(rcValues, out rcTogether);
            this.stepFourEncrypt(rcTogether, out temp);

            byte[] ciphertext = Encoding.Unicode.GetBytes(temp);
            return ciphertext;
        }

        public byte[] Decrypt(byte[] ciphertext)
        {
            string temp2;
            string temp3;
            string[] values;
            string plaintext_str;

            this.stepOneDecrypt(Encoding.Unicode.GetString(ciphertext), out temp2);
            this.stepTwoDecrypt(temp2, out temp3);
            this.stepThreeDecrypt(temp3, out values);
            this.stepFourDecrypt(values, out plaintext_str);


            byte[] plaintext = Encoding.Unicode.GetBytes(plaintext_str);
            return plaintext;
        }

        public List<string> EncryptLines(List<string> plaintextLines)
        {
            string[] rcValues;
            string rcTogether;
            List<string> encryptedLines = new List<string>();
            string oneLine = "";

            foreach (var item in plaintextLines)
            {
                string temp;
                oneLine += item.Replace(" ", "") + "\n";
                this.stepOneEncrypt(item.ToLower(), out rcValues);
                this.stepTwoEncrypt(rcValues);
                this.stepThreeEncrypt(rcValues, out rcTogether);
                this.stepFourEncrypt(rcTogether, out temp);
                encryptedLines.Add(temp);
            }

            return encryptedLines;
        }

        public List<string> DecryptLines(List<string> plaintextLines/*, out bool sameHashes*/)
        {
            string temp2;
            string temp3;
            string[] values;
            string plaintext;
            List<string> decryptedLines = new List<string>();
            string oneLine = "";

            foreach (string line in plaintextLines)
            {
                this.stepOneDecrypt(line, out temp2);
                this.stepTwoDecrypt(temp2, out temp3);
                this.stepThreeDecrypt(temp3, out values);
                this.stepFourDecrypt(values, out plaintext);
                oneLine += plaintext + "\n";
                decryptedLines.Add(plaintext);
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
                        iIndexI = i;
                        iIndexJ = j;
                    }
                    if (letter == 'j' || randomLetters.Contains(letter))
                    {
                        j--;
                        continue;
                    }

                    randomLetters.Add(letter);
                    keySquare[i, j] = letter;
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
                        iIndexI = i;
                        iIndexJ = j;
                    }
                    keySquare[i, j++] = c;
                }
                i++;
                j = 0;
            }
        }
    }
}
