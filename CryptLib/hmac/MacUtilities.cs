using System;
using System.Collections;

using CipherBox.Cryptography.Symmetric;

namespace CipherBox.Cryptography.Hmac
{
    public class HmacAlgorithms
    {
        public const string HMACSHA1 = "HMACSHA1";
        public const string HMACSHA224 = "HMACSHA224";
        public const string HMACSHA256 = "HMACSHA256";
        public const string HMACSHA384 = "HMACSHA384";
        public const string HMACSHA512 = "HMACSHA512";

        public const string HMACMD5 = "HMACMD5";
        public const string HMACMD4 = "HMACMD4";
        public const string HMACMD2 = "HMACMD2";
        public const string HMACRIPEMD128 = "HMACRIPEMD128";
        public const string HMACRIPEMD160 = "HMACRIPEMD160";
        public const string HMACTIGER     = "HMACTIGER";
        public const string HMACWHIRLPOOL = "HMACWHIRLPOOL";
    }

	/// <remarks>
	///  Utility class for creating HMac object from their names
	/// </remarks>
	public static class MacUtilities
	{
		public static IMac GetMac(string algo)
		{
			if (algo.StartsWith("HMAC"))
			{
				string digestName;
				if (algo.StartsWith("HMAC-") || algo.StartsWith("HMAC/"))
				{
					digestName = algo.Substring(5);
				}
				else
				{
					digestName = algo.Substring(4);
				}

				return new HMac(DigestAlgorithms.GetDigestAlgorithm(digestName));
			}

			Console.WriteLine("HMAC: " + algo + " not recognized.");
            return null;
		}

        public static void SetKey(IMac hmac, byte[] key)
        {
            KeyParameter kp = new KeyParameter(key);
            hmac.Init(kp);
        }

        public static byte[] ComputeHash(IMac hmac, byte[] message)
        {
            hmac.BlockUpdate(message, 0, message.Length);
            byte[] output = new byte[hmac.GetMacSize()];
            hmac.DoFinal(output, 0);
            return output;
        }

	}
}
