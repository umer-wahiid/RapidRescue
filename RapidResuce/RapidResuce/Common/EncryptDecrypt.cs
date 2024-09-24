using System.Security.Cryptography;
using System.Text;

namespace RapidResuce.Common
{
    public class EncryptDecrypt
    {
        static byte[] key1 = ASCIIEncoding.ASCII.GetBytes("everykey");
        public static string Encrypt(string originalString)
        {
            byte[] bytes = key1;
            try
            {
                if (String.IsNullOrEmpty(originalString))
                {
                    throw new ArgumentNullException("The string which needs to be encrypted can not be null.");
                }



                DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateEncryptor(bytes, bytes), CryptoStreamMode.Write);



                StreamWriter writer = new StreamWriter(cryptoStream);
                writer.Write(originalString);
                writer.Flush();
                cryptoStream.FlushFinalBlock();
                writer.Flush();



                return Convert.ToBase64String(memoryStream.GetBuffer(), 0, (int)memoryStream.Length);
            }
            catch (Exception)
            { throw; }
        }
        public static string Decrypt(string cryptedString)
        {
            byte[] bytes = key1;
            try
            {
                cryptedString = cryptedString.Replace(" ", "+");

                if (String.IsNullOrEmpty(cryptedString))
                    throw new ArgumentNullException("The string which needs to be decrypted can not be null.");

#pragma warning disable SYSLIB0021 // Type or member is obsolete
                using DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
#pragma warning restore SYSLIB0021 // Type or member is obsolete
                MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(cryptedString));
                CryptoStream cryptoStream = new CryptoStream(memoryStream, cryptoProvider.CreateDecryptor(bytes, bytes), CryptoStreamMode.Read);
                StreamReader reader = new StreamReader(cryptoStream);



                return reader.ReadToEnd();
            }
            catch (Exception)
            { throw; }
        }
    }
}
