using System;
//using System.Linq;  // requires .Net 3.5

namespace CipherBox.Cryptography.Utility
{
    public static class ByteArrayUtils
    {
        #region DotNet 2.0 compatible utils

        // implement some functions in dot Net 2.0, to replace Net 3.5 version
        public static byte[] Concat(byte[] buf1, byte[] buf2)
        {
            byte[] buffer = new byte[buf1.Length + buf2.Length];
            Array.Copy(buf1, 0, buffer, 0, buf1.Length);
            Array.Copy(buf2, 0, buffer, buf1.Length, buf2.Length);
            return buffer;

            //return buf1.Concat(buf2).ToArray();  // requires at least Net 3.5:  using System.Linq;  
        }

        public static byte[] Take(byte[] buf, int size)
        {
            byte[] buffer = new byte[size];
            Array.Copy(buf, buffer, size);
            return buffer;

            //return buf.Take(size).ToArray();  // requires at least Net 3.5:  using System.Linq;
        }

        public static byte[] Skip(byte[] buf, int size)
        {
            byte[] buffer = new byte[buf.Length - size];
            Array.Copy(buf, size, buffer, 0, buf.Length - size);
            return buffer;

            //return buf.Skip(size).ToArray();  // requires at least Net 3.5:  using System.Linq;
        }


        public static byte[] Repeat(byte padding, int size)
        {
            byte[] buffer = new byte[size];
            for (int i = 0; i < size; i++)
            {
                buffer[i] = padding;
            }
            return buffer;

            //return Enumerable.Repeat(padding, size).ToArray();  // requires at least Net 3.5:  using System.Linq;
        }

        // pad or truncate the byte array to given size
        public static byte[] Tailor(byte[] buf, int size, byte padding)
        {
            if (buf.Length < size)
            {
                return ByteArrayUtils.Concat(buf, ByteArrayUtils.Repeat(padding, size - buf.Length));
            }
            else if (buf.Length > size)
            {
                return ByteArrayUtils.Take(buf, size);
            }
            return buf;
        }

        #endregion


        // Round an int up to the next 'round' boundary
        public static int RoundUp(int value, int round)
        {
            if (round == 0)
                return round;
            return ((value + round - 1) / round) * round;
        }

        // Are two arrays equal?
        public static bool EqualBytes(byte[] arr1, byte[] arr2)
        {
            // REVIEW: use IStructuralEquatable in .Net 4.0?
            if (arr1.Length != arr2.Length)
                return false;

            for (int i = 0; i < arr1.Length; i++)
            {
                if (arr1[i] != arr2[i])
                    return false;
            }

            return true;
        }
    }
}

