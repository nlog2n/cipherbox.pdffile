using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;


// USE AesCryptoServiceProvider, DO NOT USE RijndaelManaged class!
// RijndaelManaged class and AesCryptoServiceProvider class have subtle differences on implementation.
//
// RijndaelManaged class is an implementation of Rijndael algorithm in .net framework, 
// which was not validated under NIST Cryptographic Module Validation Program (CMVP). 
// It does not support CFB or OFB mode. block size fixed to 128bits.
// AesManaged class is just a wrapper of RijndaelManaged class.
//
// AesCryptoServiceProvider class calls Windows Crypto API, which uses RSAENH.DLL, 
// and has been validated by NIST in CMVP. 


namespace CipherBox.Cryptography.Net
{
    // AES cipher with ECB mode, no padding
    // used by: Office standard encryption
    public static class AESCipherEcbNoPad
    {
        public static byte[] Decrypt(byte[] data, byte[] key)
        {
            //  Create uninitialized AES encryption object.
            AesCryptoServiceProvider cipher = new AesCryptoServiceProvider();
            {
                // MS-OFFCRYPTO v1.0 2.3.4.7 pp 39. required that the encryption mode is Electronic Codebook (ECB) 
                cipher.Mode = CipherMode.ECB;  // default is CBC
                cipher.Padding = PaddingMode.None; // default is PKCS7
                cipher.KeySize = key.Length * 8;  // key size in bits
                // cipher.IV = null; 
                // cipher.Key = key;
            }

            return SymmetricCipher.Decrypt(cipher, key, null, data);
        }

        public static byte[] Encrypt(byte[] data, byte[] key)
        {
            //  Create uninitialized AES encryption object.
            AesCryptoServiceProvider cipher = new AesCryptoServiceProvider();
            {
                // MS-OFFCRYPTO v1.0 2.3.4.7 pp 39. required that the encryption mode is Electronic Codebook (ECB) 
                cipher.Mode = CipherMode.ECB;
                cipher.Padding = PaddingMode.None;
                cipher.KeySize = key.Length * 8;  // key size in bits
                // cipher.Key = key;
                // cipher.IV = null;
            }

            return SymmetricCipher.Encrypt(cipher, key, null, data);
        }
    }
}