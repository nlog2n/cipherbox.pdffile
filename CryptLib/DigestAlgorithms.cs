using System;
using System.Collections.Generic;
using System.IO;

using CipherBox.Cryptography;
using CipherBox.Cryptography.Digest;

namespace CipherBox.Cryptography  
{
    // message digest algorithms.
    public static class DigestAlgorithms 
    {
        // "MD5", "MD-5", "SHA1", "SHA-1", "SHA224", "SHA-224", "SHA256", "SHA-256", "SHA384", "SHA-384", "SHA512", "SHA-512"
        public static IDigest GetDigestAlgorithm(string algo)
        {
            try
            {
                switch (algo)
                {
                    case "MD5": 
                    case "MD-5":
                        return new MD5Digest();
                    case "MD2": return new MD2Digest();
                    case "MD4": return new MD4Digest();
                    case "SHA1": 
                    case "SHA-1":
                        return new Sha1Digest();
                    case "SHA224": 
                    case "SHA-224":
                        return new Sha224Digest();
                    case "SHA256": 
                    case "SHA-256":
                        return new Sha256Digest();
                    case "SHA384": 
                    case "SHA-384":
                        return new Sha384Digest();
                    case "SHA512": 
                    case "SHA-512":
                        return new Sha512Digest();
                    case "RIPEMD128":
                    case "RIPEMD-128": 
                        return new RipeMD128Digest();
                    case "RIPEMD160":
                    case "RIPEMD-160": 
                        return new RipeMD160Digest();
                    case "RIPEMD256":
                    case "RIPEMD-256": 
                        return new RipeMD256Digest();
                    case "RIPEMD320":
                    case "RIPEMD-320": 
                        return new RipeMD320Digest();
                    case "TIGER": return new TigerDigest();
                    case "WHIRLPOOL": return new WhirlpoolDigest();
                    default: return null;
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Digest " + algo + " not recognized.");
                return null;
            }
        }

        public static byte[] Digest(string algo, byte[] b, int offset, int len) 
        {
            return Digest(GetDigestAlgorithm(algo), b, offset, len);
        }

        public static byte[] Digest(string algo, byte[] b) 
        {
            return Digest(GetDigestAlgorithm(algo), b, 0, b.Length);
        }

        private static byte[] Digest(IDigest d, byte[] b, int offset, int len) 
        {
            d.BlockUpdate(b, offset, len);
            byte[] r = new byte[d.GetDigestSize()];
            d.DoFinal(r, 0);
            return r;
        }

    }
}