using System;
using System.Security.Cryptography;

namespace CipherBox.Cryptography  
{
    public sealed class IVGenerator 
    {
        // Gets a 16 byte random initialization vector.
        public static byte[] GetIV() 
        {
            return GetIV(16);
        }

        // Gets a random initialization vector.
        public static byte[] GetIV(int size) 
        {
            return RandomIV(size);
        }

        // random salt
        public static byte[] RandomIV(int size)
        {
            byte[] iv = new byte[size];
            RandomNumberGenerator.Create().GetBytes(iv);
            return iv;
        }
    }
}
