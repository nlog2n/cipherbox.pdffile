using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

using CipherBox.Cryptography.Utility;

/*
Main difference between SHA1, SHA1CryptoServiceProvider, SHA1Managed and SHA1Cng are:
1) SHA1: this is abstract class. All other implementation of SHA1 (SHA1CryptoServiceProvider, SHA1Managed and SHA1Cng) implements this abstract class. 
   To create concreate SHA1 class, use SHA1.Create(). By default SHA1.Create() returns SHA1CryptoServiceProvider, which is configurable.
   To configure default SHA1 implementation: http://msdn.microsoft.com/en-us/library/693aff9y.aspx
2) SHA1CryptoServiceProvider: this is wrapper for unmanaged CryptoAPI(CAPI). This is Federal Information Processing Standard (FIPS) certified.
3) SHA1Managed: this is complete implementation of SHA1 using managed code. This is fully managed but not FIPS certified and may be slower.
4) SHA1Cng: this is wrapper for unmanaged Cryptography Next Generation (CNG). These are newer implementation of cryptographic algorithms by 
   Microsoft with Windows 2008/Windows Vista or newer. This is also FIPS certified. 
*/

namespace CipherBox.Cryptography.Net
{
    // used by:  Office2007 and CapiRC4 encryption
    // more generic one like c# "HashAlgorithm" can be found in Office Agile
    public class SHAOne
    {
        SHA1 _sha1;

        public SHAOne()
        {
            _sha1 = SHA1.Create();
        }

        // SHA1(buf)
        public byte[] ComputeHash(byte[] data)
        {
            return _sha1.ComputeHash(data);
        }

        public byte[] ComputeHash(byte[] data, int offset, int count)
        {
            return _sha1.ComputeHash(data, offset, count);
        }

        // SHA1(buf || block)
        public byte[] ComputeHash(byte[] hashBuf, byte[] block)
        {
            return ComputeHash(ByteArrayUtils.Concat(hashBuf, block)); 
        }

        public void Clear()
        {
            _sha1.Clear();
        }
    }
}