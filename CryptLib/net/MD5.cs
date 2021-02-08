using System;
using System.Security.Cryptography;

namespace CipherBox.Cryptography.Net
{
    // simple wrapper around NET MD5
    public class MDFive
    {
        MD5 _md5;

        public MDFive()
        {
            _md5 = MD5.Create();
        }

        public byte[] ComputeHash(byte[] data)
        {
            return _md5.ComputeHash(data);
        }

        public byte[] ComputeHash(byte[] data, int offset, int count)
        {
            return _md5.ComputeHash(data, offset, count);
        }

        public void Clear()
        {
            _md5.Clear();
        }


    }
}