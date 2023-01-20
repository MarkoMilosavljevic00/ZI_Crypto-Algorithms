using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        byte[] rc6Hash;
        byte[] bifidHash;

        ServiceReference1.IService1 client = new ServiceReference1.Service1Client();
        public Form1()
        {
            InitializeComponent();
        }


        #region RC6
        private void rc6EncBtn_Click(object sender, EventArgs e)
        {
            if (rc6KeyInput.Text.Length < 4)
            {
                MessageBox.Show("Kljuc mora biti veci od 4 karaktera (128bits)", "Greska!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (rc6CtrMode.Checked == true)
            {
                if (rc6NonceTxt.Text.Length != 4)
                {
                    MessageBox.Show("Nonce treba sadrzati 4 karaktera/broja", "Greska!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    GenerateRC6TigerHash(rc6Input.Text);
                    rc6Output.Text = client.EncryptRC6_CTRmode(rc6Input.Text, rc6KeyInput.Text, rc6NonceTxt.Text);
                }
            }
            else
            {
                GenerateRC6TigerHash(rc6Input.Text);
                rc6Output.Text = client.EncryptRC6(rc6Input.Text, rc6KeyInput.Text);
            }

        }
       

        private void rc6DecBtn_Click(object sender, EventArgs e)
        {
            if (rc6KeyInput.Text.Length < 4)
            {
                MessageBox.Show("Kljuc mora biti veci od 4 karaktera (128bits)", "Greska!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (rc6CtrMode.Checked == true)
            {
                if (rc6NonceTxt.Text.Length != 4)
                {
                    MessageBox.Show("Nonce treba sadrzati 4 karaktera/broja", "Greska!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                else
                {
                    rc6Output.Text = client.DecryptRC6_CTRmode(rc6Input.Text, rc6KeyInput.Text, rc6NonceTxt.Text);
                    ValidateRC6TigerHash(rc6Output.Text);
                }
            }
            else
            {
                rc6Output.Text = client.DecryptRC6(rc6Input.Text, rc6KeyInput.Text);
                ValidateRC6TigerHash(rc6Output.Text);
            }
        }

        private void rc6InputFileBtn_Click(object sender, EventArgs e)
        {
            string filter = "Text files (*.txt)|*.txt|Binary files (*.bin)|*.bin|Bitmap files (*.bmp)|*.bmp";
            ChooseInputFile(rc6PathKeyInput, rc6PathInput, rc6PathOutput, rc6EncFileBtn, rc6DecFileBtn, rc6BmpRadio, filter);
        }

        

        private void rc6OutputFileBtn_Click(object sender, EventArgs e)
        {
            ChooseOutputFile(rc6PathOutput, rc6OutputFolderBrowser, rc6EncFileBtn, rc6DecFileBtn);
        }

        private void rc6KeyFileBtn_Click(object sender, EventArgs e)
        {
            ChooseKeyFile(rc6PathKeyInput, rc6PathInput, rc6PathOutput, rc6EncFileBtn, rc6DecFileBtn);
        }

        private void rc6EncFileBtn_Click(object sender, EventArgs e)
        {
            string input = GetInputFromFile(rc6PathInput);
            string key = GetKeyFromFile(rc6PathKeyInput);
            string cipherText;
            string filename = rc6NameOutput.Text;
            string alghorithm = "RC6";
            string nonce = "";

            if(string.IsNullOrEmpty(filename))
            {
                MessageBox.Show("Unesite ime izlaznog fajla!", "Upozorenje!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (key.Length < 4)
            {
                MessageBox.Show("Kljuc mora biti veci od 4 karaktera (128bits)", "Greska!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (rc6CtrMode.Checked == true)
            {
                if (rc6NonceTxt.Text.Length != 4)
                {
                    MessageBox.Show("Nonce treba sadrzati 4 karaktera/broja", "Greska!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (rc6PathInput.Text.EndsWith(".bmp"))
                {
                    alghorithm = "RC6_CTR";
                    nonce = rc6NonceTxt.Text;
                }
            }
            if (rc6PathInput.Text.EndsWith(".bmp"))
            {
                if (client.EncryptBitmap(rc6PathInput.Text, rc6PathOutput.Text + "\\" + filename, alghorithm, rc6TigerHashChk.Checked, key))
                {
                    if (rc6TigerHashChk.Checked)
                        rc6TigerHashChk.Enabled = false;
                    MessageBox.Show("Uspesno ste enkriptovali bitmapu", "Uspesno enkriptovanje!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }
                else
                {
                    MessageBox.Show("Nije izabran nijedan validan algoritam za enkripciju bitmape!", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }
            else if(alghorithm == "RC6_CTR")
            {
                GenerateRC6TigerHash(input);
                cipherText = client.EncryptRC6_CTRmode(input, key, nonce);
            }
            else
            {
                GenerateRC6TigerHash(input);
                cipherText = client.EncryptRC6(input, key);
            }
            WriteOutputToFile(cipherText, rc6PathOutput.Text, filename, rc6TxtRadio, rc6BinRadio);
        }

        private void rc6DecFileBtn_Click(object sender, EventArgs e)
        {
            string input = GetInputFromFile(rc6PathInput);
            string key = GetKeyFromFile(rc6PathKeyInput);
            string filename = rc6NameOutput.Text;
            string plainText;

            if (key.Length < 4)
            {
                MessageBox.Show("Kljuc mora biti veci od 4 karaktera (128bits)", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (rc6PathInput.Text.EndsWith(".bmp"))
            {
                if (rc6TigerHashChk.Checked)
                    rc6TigerHashChk.Enabled = true;

                if (client.DecryptBitmap(rc6PathInput.Text, rc6PathOutput.Text + "\\" + filename + ".bmp", "RC6", rc6TigerHashChk.Checked , key))
                {
                    MessageBox.Show("Uspesno ste dekriptovali bitmapu", "Uspesno enkriptovanje!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if(rc6TigerHashChk.Checked)
                        MessageBox.Show("Fajl je uspesno validiran, hash vrednosti se poklapaju!", "Uspesna validacija", MessageBoxButtons.OK, MessageBoxIcon.Information);


                    return;
                }
                else if (rc6TigerHashChk.Checked)
                {
                    MessageBox.Show("Nepoznata vrednost hash funkcije originalnog fajla ili se hash vrednosti ne poklapaju!", "Neuspesna validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                else
                {
                    MessageBox.Show("Nije izabran nijedan validan algoritam za dekriptovanje bitmape!", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                plainText = client.DecryptRC6(input, key);
                ValidateRC6TigerHash(plainText);
            }

            WriteOutputToFile(plainText, rc6PathOutput.Text, filename, rc6TxtRadio, rc6BinRadio);
        }
        private void rc6CtrMode_CheckedChanged(object sender, EventArgs e)
        {
            if (rc6CtrMode.Checked == true)
                rc6NonceTxt.Enabled = true;
            else
                rc6NonceTxt.Enabled = false;
        }
        #endregion

        #region Bifid

        private void bifidGenKeyBtn_Click(object sender, EventArgs e)
        {
            bifidKeyTxt.Text = "";
            string[] key = client.GenerateRandomKeyBifid();
            foreach(string row in key)
            {
                bifidKeyTxt.AppendText(row);
                if (key.Last() != row)
                    bifidKeyTxt.AppendText(Environment.NewLine);
            }
            bifidEncBtn.Enabled = true;
            bifidDecBtn.Enabled = true;

        }

        private void bifidSaveKeyBtn_Click(object sender, EventArgs e)
        {
            // Pravimo novi SaveFileDialog
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            // Postavljamo filter za tipove fajlova koji će se pojaviti u dijalogu
            saveFileDialog1.Filter = "Text Files|*.txt|All Files|*.*";
            saveFileDialog1.Title = "Save key";

            // Ako korisnik izabere lokaciju i klikne na dugme "Sačuvaj",
            // otvorimo StreamWriter i upišemo tekst u fajl
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(saveFileDialog1.FileName))
                {
                    sw.Write(bifidKeyTxt.Text);
                }
            }
        }

        private void bifidEncBtn_Click(object sender, EventArgs e)
        {
            string input = bifidInputTxt.Text;
            string[] plaintextLines = input.Split('\n') ;
            GenerateBifidTigerHash(plaintextLines);
            string[] ciphertextLines = client.EncryptBifid(plaintextLines);
            bifidOutputTxt.Text = "";
            foreach (string row in ciphertextLines)
            {
                bifidOutputTxt.AppendText(row);
                if(ciphertextLines.Last() != row)
                    bifidOutputTxt.AppendText(Environment.NewLine);
            }
        }

        private void bifidDecBtn_Click(object sender, EventArgs e)
        {
            string input = bifidInputTxt.Text;
            string[] plaintextLines = input.Split('\n');
            string[] ciphertextLines = client.DecryptBifid(plaintextLines);
            ValidateBifidTigerHash(ciphertextLines);
            bifidOutputTxt.Text = "";
            foreach (string row in ciphertextLines)
            {
                bifidOutputTxt.AppendText(row);
                if (ciphertextLines.Last() != row)
                    bifidOutputTxt.AppendText(Environment.NewLine);
            }
        }

        private void bifidKeyFileBtn_Click(object sender, EventArgs e)
        {
            ChooseKeyFile(bifidKeyInputFile, bifidInputPathFile, bifidOutputPathFile, bifidEncFileBtn, bifidDecFileBtn);
        }

        private void bifidInputFileBtn_Click(object sender, EventArgs e)
        {
            string filter = "Text files (*.txt)|*.txt|Binary files (*.bin)|*.bin";
            ChooseInputFile(bifidKeyInputFile, bifidInputPathFile, bifidOutputPathFile, bifidEncFileBtn, bifidDecFileBtn, bifidBmpRadio, filter);
        }

        private void bifidOutputFileBtn_Click(object sender, EventArgs e)
        {
            ChooseOutputFile(bifidOutputPathFile, bifidOutputFolderBrowser, bifidEncFileBtn, bifidDecFileBtn);
        }

        private void bifidEncFileBtn_Click(object sender, EventArgs e)
        {
            string[] input = GetInputFromFile(bifidInputPathFile).Split('\n');
            string key = GetKeyFromFile(bifidKeyInputFile);
            if (!ValidateBifidKey(key))
                return;
            client.LoadKeyBifid(key);

            string[] ciphertext;
            string filename = bifidOutputName.Text;
            if (string.IsNullOrEmpty(filename))
            {
                MessageBox.Show("Unesite ime izlaznog fajla!", "Upozorenje!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (bifidInputPathFile.Text.EndsWith(".bmp"))
            {
                MessageBox.Show("Enkripcija bitmape nije podrzana Bifid algoritmom. Molimo vas probajte da iskoristite neki drugi algoritam.", "Greska!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //if (bifidInputPathFile.Text.EndsWith(".bmp"))
            //{
            //    if (client.EncryptBitmap(bifidInputPathFile.Text, bifidOutputPathFile.Text + "\\" + filename, "Bifid", bifidTigerHashChk.Checked, key))
            //    {
            //        if (bifidTigerHashChk.Checked)
            //            bifidTigerHashChk.Enabled = false;
            //        MessageBox.Show("Uspesno ste enkriptovali bitmapu", "Uspesno enkriptovanje!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //    else
            //    {
            //        MessageBox.Show("Nije izabran nijedan validan algoritam za enkripciju bitmape!", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return;
            //    }
            //}
            //else
            //{
            //    GenerateBifidTigerHash(input);
            //    ciphertext = client.EncryptBifid(input);
            //}

            GenerateBifidTigerHash(input);
            ciphertext = client.EncryptBifid(input);
            string oneLine = "";
            foreach (string line in ciphertext)
            {
                oneLine += line;
            }
            WriteOutputToFile(oneLine, bifidOutputPathFile.Text, filename, bifidTxtRadio, bifidBinRadio);
        }

        private void bifidDecFileBtn_Click(object sender, EventArgs e)
        {
            string[] input = GetInputFromFile(bifidInputPathFile).Split('\n');
            string key = GetKeyFromFile(bifidKeyInputFile);
            if (!ValidateBifidKey(key))
                return;
            client.LoadKeyBifid(key);
            string[] plaintext;
            string filename = bifidOutputName.Text;
            if (string.IsNullOrEmpty(filename))
            {
                MessageBox.Show("Unesite ime izlaznog fajla!", "Upozorenje!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (bifidInputPathFile.Text.EndsWith(".bmp"))
            {
                MessageBox.Show("Enkripcija bitmape nije podrzana Bifid algoritmom. Molimo vas probajte da iskoristite neki drugi algoritam.", "Greska!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            //if (bifidInputPathFile.Text.EndsWith(".bmp"))
            //{
            //    if (bifidTigerHashChk.Checked)
            //        bifidTigerHashChk.Enabled = true;
            //    if (client.DecryptBitmap(bifidInputPathFile.Text, bifidOutputPathFile.Text + "\\" + filename + ".bmp", "Bifid", bifidTigerHashChk.Checked, key))
            //    {
            //        MessageBox.Show("Uspesno ste dekriptovali bitmapu", "Uspesno enkriptovanje!", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        if (bifidTigerHashChk.Checked)
            //            MessageBox.Show("Fajl je uspesno validiran, hash vrednosti se poklapaju!", "Uspesna validacija", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        return;
            //    }
            //    else if (bifidTigerHashChk.Checked)
            //    {
            //        MessageBox.Show("Nepoznata vrednost hash funkcije originalnog fajla ili se hash vrednosti ne poklapaju!", "Neuspesna validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //        return;
            //    }
            //    else
            //    {
            //        MessageBox.Show("Nije izabran nijedan validan algoritam za dekriptovanje bitmape!", "Greska", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        return;
            //    }
            //}
            //else
            //{
            //    plaintext = client.DecryptBifid(input);
            //    ValidateBifidTigerHash(plaintext);
            //}
            plaintext = client.DecryptBifid(input);
            ValidateBifidTigerHash(plaintext);
            string oneLine = "";
            foreach (string line in plaintext)
            {
                oneLine += line;
            }
            WriteOutputToFile(oneLine, bifidOutputPathFile.Text, filename, bifidTxtRadio, bifidBinRadio);
        }

        private void bifidTigerHashChk_CheckedChanged(object sender, EventArgs e)
        {
            if (bifidTigerHashChk.Checked == true)
            {
                MessageBox.Show("Moguce je dobijanje razlicitih hash vrednosti zbog same prirode algoritma (j->i)!", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }

        #endregion
        // ======================= HELPER FUNCTIONS ========================

        #region File manager
        private void ChooseKeyFile(TextBox keyPath, TextBox inputPath, TextBox outputPath, Button encFileBtn, Button decFileBtn)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Text files (*.txt)|*.txt|Binary files (*.bin)|*.bin";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string path = dialog.FileName;
                keyPath.Text = path;
                if (!string.IsNullOrEmpty(inputPath.Text) && !string.IsNullOrEmpty(outputPath.Text))
                {
                    encFileBtn.Enabled = true;
                    decFileBtn.Enabled = true;
                }
            }
        }
        private void ChooseInputFile(TextBox keyPath, TextBox inputPath, TextBox outputPath, Button encFileBtn, Button decFileBtn, RadioButton bmpRadio, string filter)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = filter;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string path = dialog.FileName;
                inputPath.Text = path;
                if (!string.IsNullOrEmpty(keyPath.Text) && !string.IsNullOrEmpty(outputPath.Text))
                {
                    encFileBtn.Enabled = true;
                    decFileBtn.Enabled = true;
                }
                if (inputPath.Text.EndsWith(".bmp"))
                {
                    bmpRadio.Enabled = true;
                    bmpRadio.Checked = true;
                }
                else
                {
                    bmpRadio.Enabled = false;
                    bmpRadio.Checked = false;
                }
            }
        }
        private void ChooseOutputFile(TextBox outputPath, FolderBrowserDialog fbd, Button encBtn, Button decBtn)
        {
            DialogResult result = fbd.ShowDialog();
            if (result == DialogResult.OK)
            {
                outputPath.Text = fbd.SelectedPath;
                if (Directory.Exists(outputPath.Text))
                {
                    encBtn.Enabled = true;
                    decBtn.Enabled = true;
                }
            }
        }
        private string GetKeyFromFile(TextBox keyPath)
        {
            string key = "";
            if (keyPath.Text.EndsWith(".txt"))
            {
                key = File.ReadAllText(keyPath.Text);
            }
            else if (keyPath.Text.EndsWith(".bin") || keyPath.Text.EndsWith(".bmp"))
            {
                byte[] keyByte = File.ReadAllBytes(keyPath.Text);
                key = Encoding.UTF8.GetString(keyByte);
            }

            return key;
        }

        private bool ValidateBifidKey(string key)
        {
            key = key.Replace("\r\n", "");
            if (key.Length != 25)
            {
                MessageBox.Show("Kljuc treba biti matrica 5x5 koja sadrzi sva mala slova", "Greska!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            foreach (char c in key)
            {
                if (!char.IsLower(c))
                {
                    MessageBox.Show("Kljuc treba biti matrica 5x5 koja sadrzi sva mala slova", "Greska!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            return true;
        }

        private string GetInputFromFile(TextBox inputPath)
        {
            string input = "";
            if (inputPath.Text.EndsWith(".txt"))
            {
                input = File.ReadAllText(inputPath.Text);

            }
            else if (inputPath.Text.EndsWith(".bin") || inputPath.Text.EndsWith(".bmp"))
            {
                byte[] inputByte = File.ReadAllBytes(inputPath.Text);
                input = Encoding.Default.GetString(inputByte);
            }

            return input;
        }

        private void WriteOutputToFile(string cipherText, string path, string filename, RadioButton txtRadio, RadioButton binRadio)
        {
            if (txtRadio.Checked)
            {
                File.WriteAllText(path + "\\" + filename + ".txt", cipherText);
            }
            else if (binRadio.Checked)
            {
                File.WriteAllBytes(path + "\\" + filename + ".bin", Encoding.Default.GetBytes(cipherText));
            }
            else
            {
                MessageBox.Show("Molimo oznacite koju ekstenziju zelite da ima vas izlazni fajl [txt - bin - bmp]!", "Upozorenje", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            }
        }
        private static string MergeHeaderWithData(byte[] header, string encryptedPixelsStr)
        {
            byte[] encryptedPixels = Encoding.Default.GetBytes(encryptedPixelsStr);
            byte[] bitmapData = new byte[header.Length + encryptedPixels.Length];
            Array.Copy(header, bitmapData, header.Length);
            Array.Copy(encryptedPixels, 0, bitmapData, header.Length, encryptedPixels.Length);
            return Encoding.Default.GetString(bitmapData);
        }

        private string GetBmpPixels(string input)
        {
            byte[] input_bytes = Encoding.Default.GetBytes(input);
            byte[] pixels = input_bytes.Skip(54).ToArray();
            return Encoding.Default.GetString(pixels);
        }

        private byte[] GetBmpHeader(string input)
        {
            byte[] input_bytes = Encoding.Default.GetBytes(input);
            byte[] header = new byte[54];
            Array.Copy(input_bytes, header, 54);
            return header;
        }
        #endregion

        #region Tiger Hasher
        private void ValidateRC6TigerHash(string plaintext)
        {
            if (rc6TigerHashChk.Checked)
            {
                rc6TigerHashChk.Enabled = true;
                if (rc6Hash != null)
                {
                    byte[] checkHash = client.GenerateTigerHash(plaintext);
                    if (checkHash.SequenceEqual(rc6Hash))
                    {
                        MessageBox.Show("Enrkipcija je uspesno validirana, hash vrednosti se poklapaju!", "Uspesna validacija", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Vrednosti hash funkcije originalnog fajla i dekriptovanog nisu iste!", "Neuspesna validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    }
                }
                else
                {
                    MessageBox.Show("Nepoznata vrednost hash funkcije originalnog fajla!", "Neuspesna validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void GenerateRC6TigerHash(string input)
        {
            if (rc6TigerHashChk.Checked)
            {
                rc6TigerHashChk.Enabled = false;
                rc6Hash = client.GenerateTigerHash(input);
            }
        }

        private void ValidateBifidTigerHash(string[] input)
        {
            if (bifidTigerHashChk.Checked)
            {
                bifidTigerHashChk.Enabled = true;
                if (bifidHash != null)
                {
                    byte[] checkHash;
                    string oneLine = "";
                    foreach (string line in input)
                    {
                        oneLine += line;
                    }
                    checkHash = client.GenerateTigerHash(oneLine);
                    if (checkHash.SequenceEqual(bifidHash))
                    {
                        MessageBox.Show("Enrkipcija je uspesno validirana, hash vrednosti se poklapaju!", "Uspesna validacija", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Vrednosti hash funkcije originalnog fajla i dekriptovanog nisu iste!", "Neuspesna validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    }
                }
                else
                {
                    MessageBox.Show("Nepoznata vrednost hash funkcije originalnog fajla!", "Neuspesna validacija", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void GenerateBifidTigerHash(string[] input)
        {
            if (bifidTigerHashChk.Checked)
            {
                bifidTigerHashChk.Enabled = false;
                string oneLine = "";
                string tmp;
                foreach(string line in input)
                {
                    tmp = line.Replace(" ","");
                    oneLine += tmp.Replace("\r", "");
                }
                bifidHash = client.GenerateTigerHash(oneLine);
            }
        }



        #endregion

    }
}
