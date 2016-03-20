using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace UtilsLib
{
    public static class Cryptor
    {
        public static readonly byte[] DEBUG_key = { 0x2e, 0x22, 0x9c, 0x20, 0x4e, 0xaf, 0xbc, 0xf8, 0x59, 0x97, 0x1a, 0x7a, 0xbd, 0x3e, 0xb2, 0x35, 0xef, 0xc1, 0x16, 0x88, 0x6 };

        /// <summary>
        /// Crypt data and convert to base64
        /// </summary>
        public static byte[] Crypt(byte[] plainData, byte[] key)
        {
            if (plainData == null)
                return null;
            
            try
            {
                string encryptedText;
                using (var encryptor = Aes.Create())
                {
                    if (encryptor == null)
                        return null;

                    var pdb = new Rfc2898DeriveBytes(key.ToString(),
                        new byte[] {0x3f, 0x78, 0x2a, 0x48, 0x42, 0x9a, 0x9e, 0x68, 0xdb, 0x39, 0x3e, 0xd2, 0xdd});
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(plainData, 0, plainData.Length);
                            cs.Close();
                        }
                        encryptedText = Convert.ToBase64String(ms.ToArray());
                    }
                }
                return encryptedText.GetBytes();
            }
            catch (Exception e)
            {
                Console.WriteLine("[Cryptor] Exception: " + e.Message + "\n" + e.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// Decrypt AES-encrypted base64 data to byte array
        /// </summary>
        public static byte[] Decrypt(byte[] cipherText, byte[] key)
        {
            if (cipherText == null)
                return null;
            
            try
            {
                var cipherBytes = Convert.FromBase64String(cipherText.GetString());
                string plaindata;
                using (var encryptor = Aes.Create())
                {
                    if (encryptor == null)
                        return null;

                    var pdb = new Rfc2898DeriveBytes(key.ToString(), new byte[] { 0x3f, 0x78, 0x2a, 0x48, 0x42, 0x9a, 0x9e, 0x68, 0xdb, 0x39, 0x3e, 0xd2, 0xdd });
                    encryptor.Key = pdb.GetBytes(32);
                    encryptor.IV = pdb.GetBytes(16);
                    using (var ms = new MemoryStream())
                    {
                        using (var cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        plaindata = Encoding.Unicode.GetString(ms.ToArray());
                    }
                }
                return plaindata.GetBytes();
            }
            catch (FormatException e)
            {
                // bad input
                return null;
            }
            catch (Exception e)
            {
                Console.WriteLine("[Cryptor] Exception: " + e.Message + "\n" + e.StackTrace);
                return null;
            }
        }
    }
}