using System;

using CipherBox.Cryptography;
using CipherBox.Cryptography.Symmetric;
using CipherBox.Cryptography.Padding;


namespace CipherBox.Cryptography 
{
    // Creates an AES Cipher with CBC and no padding.
    // Used by PDF encryption
    public class AESCipherCbcNoPad 
    {
        private IBlockCipher cbc;
        
        // Creates a new instance of AESCipher 
        public AESCipherCbcNoPad(bool forEncryption, byte[] key) 
        {
            IBlockCipher aes = new AesFastEngine();
            cbc = new CbcBlockCipher(aes);
            KeyParameter kp = new KeyParameter(key);
            cbc.Init(forEncryption, kp);
        }
        
        public byte[] ProcessBlock(byte[] inp, int inpOff, int inpLen) 
        {
            if ((inpLen % cbc.GetBlockSize()) != 0)
                throw new ArgumentException("Not multiple of block: " + inpLen);
            byte[] outp = new byte[inpLen];
            int baseOffset = 0;
            while (inpLen > 0) {
                cbc.ProcessBlock(inp, inpOff, outp, baseOffset);
                inpLen -= cbc.GetBlockSize();
                baseOffset += cbc.GetBlockSize();
                inpOff += cbc.GetBlockSize();
            }
            return outp;
        }        
    }


    // Creates an AES Cipher with CBC and padding PKCS5/7.
    // Used by PDF encryption
    public class AESCipherCbcPkcs7
    {
        private PaddedBufferedBlockCipher bp;

        // Creates a new instance of AESCipher
        public AESCipherCbcPkcs7(bool forEncryption, byte[] key, byte[] iv)
        {
            IBlockCipher aes = new AesFastEngine();
            IBlockCipher cbc = new CbcBlockCipher(aes);
            bp = new PaddedBufferedBlockCipher(cbc);
            KeyParameter kp = new KeyParameter(key);
            ParametersWithIV piv = new ParametersWithIV(kp, iv);
            bp.Init(forEncryption, piv);
        }

        public byte[] Update(byte[] inp, int inpOff, int inpLen)
        {
            int neededLen = bp.GetUpdateOutputSize(inpLen);
            byte[] outp = null;
            if (neededLen > 0)
                outp = new byte[neededLen];
            else
                neededLen = 0;
            bp.ProcessBytes(inp, inpOff, inpLen, outp, 0);
            return outp;
        }

        public byte[] DoFinal()
        {
            int neededLen = bp.GetOutputSize(0);
            byte[] outp = new byte[neededLen];
            int n = 0;
            try
            {
                n = bp.DoFinal(outp, 0);
            }
            catch
            {
                return outp;
            }
            if (n != outp.Length)
            {
                byte[] outp2 = new byte[n];
                System.Array.Copy(outp, 0, outp2, 0, n);
                return outp2;
            }
            else
            {
                return outp;
            }
        }

    }
}
