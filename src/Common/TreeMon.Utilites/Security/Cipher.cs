// Copyright (c) 2017 TreeMon.org.
//Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace TreeMon.Utilites.Security
{
    public class Cipher 
    {
        protected Cipher() { }

        public const int SALT_BYTE_SIZE = 24;
        public static string Crypt(string Password, string Data, bool bEncrypt)
        {
            if (string.IsNullOrEmpty(Data))
                return Data;

            try
            {
                //backlog generate random salt and store in db
                //const string input = "DynamicSaltString";//stored in db table
                //byte[] array = Encoding.ASCII.GetBytes(input);

                byte[] u8_Salt = new byte[] { 0x26, 0x19, 0x81, 0x4E, 0xA0, 0x6D, 0x95, 0x34, 0x26, 0x75, 0x64, 0x05, 0xF6 };
                PasswordDeriveBytes i_Pass = new PasswordDeriveBytes(Password, u8_Salt);
                Rijndael i_Alg = Rijndael.Create();
                i_Alg.Key = i_Pass.GetBytes(32);
                i_Alg.IV = i_Pass.GetBytes(16);
                ICryptoTransform i_Trans = (bEncrypt) ? i_Alg.CreateEncryptor() : i_Alg.CreateDecryptor();
                MemoryStream i_Mem = new MemoryStream();
                CryptoStream i_Crypt = new CryptoStream(i_Mem, i_Trans, CryptoStreamMode.Write);
                byte[] u8_Data;
                if (bEncrypt)
                    u8_Data = Encoding.Unicode.GetBytes(Data);
                else
                    u8_Data = Convert.FromBase64String(Data);

                i_Crypt.Write(u8_Data, 0, u8_Data.Length);
                i_Crypt.Close();

                if (bEncrypt)
                    return Convert.ToBase64String(i_Mem.ToArray());
                else
                    return Encoding.Unicode.GetString(i_Mem.ToArray());
            }
            catch 
            {
                return Data;
            }
        }

        public static async Task<string> CryptAsync(string Password, string Data, bool bEncrypt)
        {
            if (string.IsNullOrEmpty(Data))
                return Data;

            try
            {
                //backlog generate random salt and store in db
                //const string input = "DynamicSaltString";//stored in db table
                //byte[] array = Encoding.ASCII.GetBytes(input);

                byte[] u8_Salt = new byte[] { 0x26, 0x19, 0x81, 0x4E, 0xA0, 0x6D, 0x95, 0x34, 0x26, 0x75, 0x64, 0x05, 0xF6 };
                PasswordDeriveBytes i_Pass = new PasswordDeriveBytes(Password, u8_Salt);
                Rijndael i_Alg = Rijndael.Create();
                i_Alg.Key = i_Pass.GetBytes(32);
                i_Alg.IV = i_Pass.GetBytes(16);
                ICryptoTransform i_Trans = (bEncrypt) ? i_Alg.CreateEncryptor() : i_Alg.CreateDecryptor();
                MemoryStream i_Mem = new MemoryStream();
                CryptoStream i_Crypt = new CryptoStream(i_Mem, i_Trans, CryptoStreamMode.Write);
                byte[] u8_Data;
                if (bEncrypt)
                    u8_Data = Encoding.Unicode.GetBytes(Data);
                else
                    u8_Data = Convert.FromBase64String(Data);

                await i_Crypt.WriteAsync(u8_Data, 0, u8_Data.Length);
                i_Crypt.Close();

                if (bEncrypt)
                    return Convert.ToBase64String(i_Mem.ToArray());
                else
                    return Encoding.Unicode.GetString(i_Mem.ToArray());
            }
            catch
            {
                return Data;
            }
        }


        public static string GenerateSalt()
        {
            RNGCryptoServiceProvider csprng = new RNGCryptoServiceProvider();
            byte[] salt = new byte[SALT_BYTE_SIZE];
            csprng.GetBytes(salt);
            return Encoding.UTF8.GetString(salt);
        }

        public static string RandomString(int length)
        {
            string[] vowels = { "a", "e", "i", "o", "u", "1", "3", "5", "7", "9" };
            string[] cons = { "b", "c", "d", "g", "h", "j", "k", "l", "m", "n", "p", "r", "s", "t", "u", "v", "w", "tr", "cr", "br", "fr", "th", "dr", "ch", "ph", "wr", "st", "sp", "sw", "pr", "sl", "cl", "2", "4", "6", "8", "0" };

            string password = "";
            int num_vowels = vowels.Length;
            int num_cons = cons.Length;

            for (int i = 0; i < length; i++)
            {
                Random r = new Random(Guid.NewGuid().GetHashCode());

                password += cons[r.Next(0, num_cons - 1)] + vowels[r.Next(0, num_vowels - 1)];
            }

            return password.Substring(0, length);
        }

        //Generates a 64 bit Key.
        public static string GenerateKey()
        {
            // Create an instance of Symetric Algorithm. Key and IV is generated automatically.
            DESCryptoServiceProvider desCrypto = (DESCryptoServiceProvider)DESCryptoServiceProvider.Create();

            // Use the Automatically generated key for Encryption. 
            return ASCIIEncoding.ASCII.GetString(desCrypto.Key);
        }

        public static  async Task<bool> EncryptFileAsync(string inputFile, string outputFilename,string sKey)
        {
            if (!File.Exists(inputFile))
                return false;

            try
            {
                string inputContent = await new FileInfo(inputFile).OpenText().ReadToEndAsync();

                if (string.IsNullOrWhiteSpace(inputContent))
                    return false;

                inputContent= await Cipher.CryptAsync(sKey, inputContent, true);

                 await new FileInfo(outputFilename).CreateText().WriteAsync(inputContent);
            }
            catch 
            {
                return false;
            }
            return true;
        }

        public static async Task<bool> DecryptFileAsync(string inputFile,string outputFile, string sKey)
        {
            if (!File.Exists(inputFile))
                return false;

            string inputContent = "";
            try
            {
                using (FileStream fileStream = new FileStream(inputFile, FileMode.Open, FileAccess.Read))
                using (StreamReader reader = new StreamReader(fileStream))
                {
                    inputContent = await reader.ReadToEndAsync();
                    inputContent = await Cipher.CryptAsync(sKey, inputContent, false);
                }

                using (StreamWriter fsDecrypted = new StreamWriter(outputFile)) //Print the contents of the decrypted file.
                {
                    await fsDecrypted.WriteAsync(inputContent);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
