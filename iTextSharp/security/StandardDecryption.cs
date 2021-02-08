using System;

using CipherBox.Cryptography;

namespace iTextSharp.text.pdf
{
    public class StandardDecryption
    {
        protected RC4 arcfour;
        protected AESCipherCbcPkcs7 cipher;
        private byte[] key;
        private bool aes;
        private bool initiated;
        private byte[] iv = new byte[16];
        private int ivptr;

        /** Creates a new instance of StandardDecryption */
        public StandardDecryption(byte[] key, int off, int len, Revisions revision)
        {
            aes = (revision == Revisions.V4 || revision == Revisions.V5);
            if (aes)
            {
                this.key = new byte[len];
                System.Array.Copy(key, off, this.key, 0, len);
            }
            else
            {
                byte[] rc4key = new byte[len];
                Array.Copy(key, off, rc4key, 0, len);
                arcfour = new RC4(rc4key);
            }
        }

        public byte[] Update(byte[] b, int off, int len)
        {
            if (aes)
            {
                if (initiated)
                {
                    return cipher.Update(b, off, len);
                }
                else
                {
                    int left = Math.Min(iv.Length - ivptr, len);
                    System.Array.Copy(b, off, iv, ivptr, left);
                    off += left;
                    len -= left;
                    ivptr += left;
                    if (ivptr == iv.Length)
                    {
                        cipher = new AESCipherCbcPkcs7(false, key, iv);
                        initiated = true;
                        if (len > 0)
                            return cipher.Update(b, off, len);
                    }
                    return null;
                }
            }
            else
            {
                byte[] msg = new byte[len];
                Array.Copy(b, off, msg, 0, len);
                msg = arcfour.Encrypt(msg);
                return msg;
            }
        }

        public byte[] Finish()
        {
            if (aes)
                return cipher.DoFinal();
            else
                return null;
        }
    }
}
