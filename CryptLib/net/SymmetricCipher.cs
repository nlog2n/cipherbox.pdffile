using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace CipherBox.Cryptography.Net
{
    // Names of cipher algorithms, used by GetCipherAlgorithm() in .Net
    public class CipherAlgorithms
    {
        public const string AES = "AES";
        public const string RC2 = "RC2";
        public const string RC4 = "RC4"; // is not supported by .Net. use own implementation
        public const string DES = "DES";
        public const string DESX = "DESX";
        public const string TripleDES = "3DES";
        public const string TripleDES112 = "3DES_112";
    }

    // A generic cipher wrapper for .Net SymmetricAlgorithm, including AES
    // used by:  Office 2010 Agile encryption, and internal AESCipherEcbNoPad
    public static class SymmetricCipher
    {
        // Get System.Security.Cryptography.SymmetricAlgorithm by name
        // Note: you need to set properties before using, such as Mode, KeySize(bits), Padding
        public static SymmetricAlgorithm GetCipherAlgorithm(string algo)
        {
            //SymmetricAlgorithm cipher = SymmetricAlgorithm.Create(algo);  // Note: Some names like "AES" only supported by .Net 4.0

            SymmetricAlgorithm cipher = null;
            switch (algo)
            {
                case CipherAlgorithms.RC2:
                    cipher = new RC2CryptoServiceProvider();
                    break;

                case CipherAlgorithms.RC4:
                    Console.WriteLine("RC4 is not a supported algorithm");
                    return null;

                case CipherAlgorithms.DES:
                    cipher = new DESCryptoServiceProvider();
                    break;

                case CipherAlgorithms.DESX:
                    Console.WriteLine("DESX is not a supported algorithm");
                    return null;

                case CipherAlgorithms.TripleDES:
                    cipher = new TripleDESCryptoServiceProvider();
                    break;

                case CipherAlgorithms.TripleDES112:
                    cipher = new TripleDESCryptoServiceProvider();
                    break;

                case CipherAlgorithms.AES:
                default:
                    cipher = new AesCryptoServiceProvider();
                    break;
            }

            if (cipher == null)
            {
                Console.WriteLine(string.Format("An symmetric key algorithm of {0} could not be created.", algo));
                return null;
            }

            return cipher;
        }

        // decrypt using symmetric key algorithm
        public static byte[] Decrypt(SymmetricAlgorithm cipher, byte[] keyBytes, byte[] ivBytes, byte[] cipherText)
        {
            //  Generate decryptor from the existing key bytes and initialization 
            //  vector. Key size will be defined based on the number of the key bytes.
            ICryptoTransform decryptor = cipher.CreateDecryptor(keyBytes, ivBytes);

            //  Define memory stream which will be used to hold encrypted data.
            using (MemoryStream memoryStream = new MemoryStream(cipherText, 0, cipherText.Length))
            {
                //  Define memory stream which will be used to hold encrypted data.
                using (CryptoStream cryptoStream  = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    //  Since at this point we don't know what the size of decrypted data will be, 
                    //  we allocate the buffer long enough to hold ciphertext;
                    //  Note: plaintext is never longer than ciphertext.
                    byte[] decryptedBytes = new byte[cipherText.Length];
                    int decryptedByteCount = cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);

                    // adjust length
                    byte[] result = decryptedBytes;
                    if (decryptedByteCount < decryptedBytes.Length)
                    {
                        result = new byte[decryptedByteCount];
                        Array.Copy(decryptedBytes, result, result.Length);
                    }
                    return result;
                }
            }
        }


        // encrypt using symmetric key algorithm
        public static byte[] Encrypt(SymmetricAlgorithm cipher, byte[] keyBytes, byte[] ivBytes, byte[] plainText)
        {
            //  Generate encryptor from the existing key bytes and initialization 
            //  vector. Key size will be defined based on the number of the key bytes.
            ICryptoTransform encryptor = cipher.CreateEncryptor(keyBytes, ivBytes);

            //  Define memory stream which will be used to hold encrypted data.
            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                //  Define memory stream which will be used to hold encrypted data.
                using (CryptoStream cryptoStream  = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    //  Start encrypting.
                    cryptoStream.Write(plainText, 0, plainText.Length);

                    //  Finish encrypting.
                    cryptoStream.FlushFinalBlock();
                }

                //  Convert our encrypted data from a memory stream into a byte array.
                return memoryStream.ToArray();
            }
        }
    }
}