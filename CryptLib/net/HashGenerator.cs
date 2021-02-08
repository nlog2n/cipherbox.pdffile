using System;
using System.Security.Cryptography;

namespace CipherBox.Cryptography.Net
{
    // Names of hash algorithms, used by GetHashAlgorithm() below in .Net
    public class HashAlgorithms
    {
        public const string SHA1 = "SHA-1";
        public const string SHA256 = "SHA256";
        public const string SHA384 = "SHA384";
        public const string SHA512 = "SHA512";

        public const string MD5 = "MD5";
        public const string MD4 = "MD4";
        public const string MD2 = "MD2";
        public const string RIPEMD128 = "RIPEMD-128";
        public const string RIPEMD160 = "RIPEMD-160";
        public const string WHIRLPOOL = "WHIRLPOOL";
    }



    // A generic hash wrapper for .Net HashAlgorithm
    // used by:  Office Agile encryption
    public static class HashGenerator
    {
        public static HashAlgorithm GetHashAlgorithm(string algo)
        {
            // Obsolete!
            //return HashAlgorithm.Create(algo);

            // We switch over the possible names because, unfortunately, HashAlgorithm.Create
            // chooses to initialize the SHA***Managed() implementations for SHA 256, 284, and
            // 512. These implementations are not FIPS-compliant, therefore we explicitly
            // instantiate them.
            HashAlgorithm hashAlg;
            switch (algo)
            {
                case "SHA256": hashAlg = new SHA256CryptoServiceProvider();
                    break;
                case "SHA384": hashAlg = new SHA384CryptoServiceProvider();
                    break;
                case "SHA512": hashAlg = new SHA512CryptoServiceProvider();
                    break;
                default: hashAlg = HashAlgorithm.Create(algo);
                    break;
            }

            // TODO: check more??
            //if (hashAlg.HashSize != (agile.pkeHashSize << 3))
            //    return null; //Unexpected hash size

            // Initialize method is usually used to reuse the hashAlg object by reset internal state.
            // Since in our case each time the hash algorithm may be different, we simply create a new one.
            // hashAlg.Initialize();  

            return hashAlg;
        }

    }
}