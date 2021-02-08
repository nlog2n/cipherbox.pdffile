// RC4 from http://sourceforge.net/projects/rc4charp/?source=recommended

/*

Managed RC4:
 
This project is to get around the issue that RC4 is NOT available in .NET and there is
special behavior with respect to MS C++ RC4 40-bit encryption.
The MSDN topic "Salt Value Functionality" explains this:
=== begin quote====
The Base Provider creates 40-bit symmetric keys created with eleven bytes of zero-value salt, 
eleven bytes of nonzero salt if CRYPTCREATESALT is specified, or no salt value. 
A 40-bit symmetric key with zero-value salt, however, is not equivalent to a 40-bit symmetric key without salt. 
For interoperability, keys must be created without salt. 
This problem results from a default condition that occurs only with keys of exactly 40 bits. 
All other key lengths do not have salt allocated by default.

Both the Base Providers and the Extended Provider can use the CRYPTNOSALT flag to specify 
that no salt value is allocated for a 40-bit symmetric key. 
The functions that accept this flag are 
  CryptGenKey, 
  CryptDeriveKey, and 
  CryptImportKey. 
By default, these functions provide backward compatibility for the 40-bit symmetric key 
case by continuing the use of the eleven-byte-long zero-value salt.
=== end quote =====

So to make it clear, Microsoft implementation of RC4 for 40-bit key is to set the last 88 bits all 0. 
The following two are exactly the same:
a) key size = 5 bytes
b) key size = 16 bytes AND the last 11 bytes are 0

This will impact CapiRC4Encryption only because Legacy uses 128-bit key.
  
*/


using System;

namespace CipherBox.Cryptography
{
    public class RC4
    {
        int x = 0;
        int y = 0;
        byte[] S = new byte[256];

        // init: Key-Scheduling Algorithm 
        public RC4(byte[] key)
        {
            int keyLength = key.Length;
            for (int i = 0; i < 256; i++)
            {
                S[i] = (byte)i;
            }

            int j = 0;
            for (int i = 0; i < 256; i++)
            {
                j = (j + S[i] + key[i % keyLength]) % 256;
                Swap(S, i, j); // S.Swap(i, j);
            }
        }

        // Pseudo-Random Generation Algorithm 
        private byte keyItem()
        {
            x = (x + 1) % 256;
            y = (y + S[x]) % 256;
            Swap(S, x, y); //  S.Swap(x, y);
            return S[(S[x] + S[y]) % 256];
        }     

        public byte[] Encrypt(byte[] data)
        {      
            byte[] cipher = new byte[data.Length];
            for (int m = 0; m < data.Length; m++)
            {               
                cipher[m] = (byte)(data[m] ^ keyItem());
            }
            return cipher;
        }

        public byte[] Decrypt(byte[] data)
        {
            return Encrypt(data);
        }

        // added by fanghui on 20130906, to use Net 2.0 only
        private static void Swap(byte[] array, int index1, int index2)
        {
            byte temp = array[index1];
            array[index1] = array[index2];
            array[index2] = temp;
        }
    }
}
