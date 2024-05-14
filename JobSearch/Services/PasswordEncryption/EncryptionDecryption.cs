using System;
using System.Security.Cryptography;
using System.Text;
namespace Services.PasswordEncryption
{
    public class AESEncryptionUtility
    {
        public string Encrypt(string plaintext, string encryptionKey, out string ivKey)
        {
            using Aes aesObject = Aes.Create();

            aesObject.Key = Convert.FromBase64String(encryptionKey); 
            aesObject.BlockSize = 128;
            aesObject.Padding = PaddingMode.Zeros;
            aesObject.GenerateIV();

            ivKey = Convert.ToBase64String(aesObject.IV);

            ICryptoTransform encryptor = aesObject.CreateEncryptor();

            byte[] encryptedData;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter sw = new StreamWriter(cs))
                    {
                        sw.Write(plaintext);
                    }
                    encryptedData = ms.ToArray();
                }
            }

            return Convert.ToBase64String(encryptedData);
        }
        
        //public bool AuthenticateUser(string EncryptedKey, string StoredPassword)
        //{


        //}


    }
}


