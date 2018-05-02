using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryptionDecryptionDemo
{
    public static class Encryption
    {
        private static readonly byte[] SALT = new byte[] { 0x26, 0xdc, 0xff, 0x00, 0xad, 0xed, 0x7a, 0xee, 0xc5, 0xfe, 0x07, 0xaf, 0x4d, 0x08, 0x22, 0x3c };

        public static Stream GenerateStreamFromString(string s)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream, new UTF8Encoding(false));
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        public static void EncryptFile(string content, string outputFile, string password)
        {
            EncryptFile(GenerateStreamFromString(content), outputFile, password);
        }

        public static void EncryptFile(Stream stream, string outputFile, string password)
        {
            using (FileStream fsCrypt = new FileStream(outputFile, FileMode.Create))
            {
                using (RijndaelManaged rmCrypto = new RijndaelManaged())
                {
                    using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, SALT))
                    {
                        rmCrypto.Key = pdb.GetBytes(32);
                        rmCrypto.IV = pdb.GetBytes(16);
                    }

                    using (CryptoStream cs = new CryptoStream(fsCrypt, rmCrypto.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        int data;
                        while ((data = stream.ReadByte()) != -1)
                            cs.WriteByte((byte)data);
                    }
                }
            }
        }

        public static string DecryptFile(string inputFile, string password)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                DecryptFile(inputFile, ms, password);
                return new UTF8Encoding(false).GetString(ms.ToArray());
            }
        }

        public static void DecryptFile(string inputFile, Stream outputStream, string password)
        {
            try
            {
                using (FileStream fsCrypt = new FileStream(inputFile, FileMode.Open))
                {
                    using (RijndaelManaged rmCrypto = new RijndaelManaged())
                    {
                        using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, SALT))
                        {
                            rmCrypto.Key = pdb.GetBytes(32);
                            rmCrypto.IV = pdb.GetBytes(16);
                        }

                        using (CryptoStream cs = new CryptoStream(fsCrypt, rmCrypto.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            int data;
                            while ((data = cs.ReadByte()) != -1)
                                outputStream.WriteByte((byte)data);
                        }
                    }
                }
            }
            catch (CryptographicException)
            {

            }
        }
    }
}
