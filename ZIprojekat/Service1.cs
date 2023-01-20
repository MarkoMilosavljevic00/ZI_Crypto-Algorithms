using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ZIprojekat
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession)]
    public class Service1 : IService1
    {
        RC6 rc6;
        CTR ctr;
        Bifid bifid;
        byte[] rc6BmpHash;
        public Service1()
        {
            rc6 = new RC6();
            bifid = new Bifid();
            ctr = new CTR(rc6);
        }

        public string GetData(string value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        public bool EncryptBitmap(string inputPath, string outputPath, string alghorithm, bool hash, string key, string nonce)
        {
            byte[] all_data = File.ReadAllBytes(inputPath);
            byte[] header = new byte[54];
            Array.Copy(all_data, header, 54);
            byte[] pixelData = all_data.Skip(54).ToArray();
            byte[] encData;

            if (hash)
            {
                TigerHash tigerHash = new TigerHash();
                rc6BmpHash = tigerHash.Process(Encoding.Default.GetString(pixelData));
            }

            switch (alghorithm)
            {
                case "RC6":
                    rc6.ExpandKey(Encoding.Default.GetBytes(key));
                    encData = rc6.Encrypt(pixelData);
                    break;
                case "RC6_CTR":
                    ctr.SetNonce(nonce);
                    encData = ctr.EncryptRC6(pixelData, key);
                    break;
                default:
                    return false;
            }

            byte[] encryptedData = new byte[header.Length + encData.Length];
            Array.Copy(header, encryptedData, header.Length);
            Array.Copy(encData, 0, encryptedData, header.Length, encData.Length);
            using (MemoryStream stream = new MemoryStream(encryptedData))
            {
                Bitmap image = new Bitmap(stream);
                image.Save(outputPath + ".bmp");
            }

            return true;
        }

        public bool DecryptBitmap(string inputPath, string outputPath, string alghorithm, bool hash, string key, string nonce)
        {
            byte[] all_data = File.ReadAllBytes(inputPath);
            byte[] header = new byte[54];
            byte[] pixelData = all_data.Skip(54).ToArray();
            byte[] decData;

            Array.Copy(all_data, header, 54);

            switch (alghorithm)
            {
                case "RC6":
                    rc6.ExpandKey(Encoding.Default.GetBytes(key));
                    decData = rc6.Decrypt(pixelData);
                    break;
                case "RC6_CTR":
                    ctr.SetNonce(nonce);
                    decData = ctr.DecryptRC6(pixelData, key);
                    break;
                default:
                    return false;
            }

            if (hash)
            {
                if (rc6BmpHash != null)
                {
                    TigerHash tigerHash = new TigerHash();
                    byte[] checkHash = tigerHash.Process(Encoding.Default.GetString(decData));
                    if (!checkHash.SequenceEqual(rc6BmpHash))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            byte[] decryptedData = new byte[header.Length + decData.Length];
            Array.Copy(header, decryptedData, header.Length);
            Array.Copy(decData, 0, decryptedData, header.Length, decData.Length);

            using (MemoryStream stream = new MemoryStream(decryptedData))
            {
                Bitmap image = new Bitmap(stream);
                image.Save(outputPath);
            }
            return true;
        }


        public string EncryptRC6(string source, string key)
        {
            //RC6 rc = new RC6(Encoding.UTF8.GetBytes(key));
            rc6.ExpandKey(Encoding.UTF8.GetBytes(key));
            byte[] byteText = Encoding.Default.GetBytes(source);
            string res = Encoding.Default.GetString(rc6.Encrypt(byteText));
            return res;
        }

        public string EncryptRC6_CTRmode(string source, string key, string nonce)
        {
            ctr.SetNonce(nonce);
            byte[] byteText = Encoding.Default.GetBytes(source);
            string res = Encoding.Default.GetString(ctr.EncryptRC6(byteText, key));
            return res;

        }

        public string DecryptRC6(string source, string key)
        {
            //RC6 rc = new RC6(Encoding.UTF8.GetBytes(key));
            rc6.ExpandKey(Encoding.UTF8.GetBytes(key));
            string res = Encoding.Default.GetString(rc6.Decrypt(Encoding.Default.GetBytes(source)));
            return res;
        }

        public string DecryptRC6_CTRmode(string source, string key, string nonce)
        {
            ctr.SetNonce(nonce);
            byte[] byteText = Encoding.Default.GetBytes(source);
            string res = Encoding.Default.GetString(ctr.DecryptRC6(byteText, key));
            return res;

        }

        public List<string> GenerateRandomKeyBifid()
        {
            List<string> key = bifid.GenerateKey();
            return key;
        }

       public void LoadKeyBifid(string key)
        {
            bifid.LoadKey(key);
        }

        public List<string> EncryptBifid(List<string> source)
        {
            List<string> res = bifid.EncryptLines(source);
            return res;
        }

        public List<string> DecryptBifid(List<string> source)
        {
            List<string> res = bifid.DecryptLines(source);
            return res;
        }

        public byte[] GenerateTigerHash(string source)
        {
            TigerHash th = new TigerHash();
            byte[] res = th.Process(source);
            return res;
        }

    }
}
