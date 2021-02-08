using System;

namespace CipherBox.Cryptography
{
    // interface that a message digest conforms to.
    public interface IDigest
    {
        // return the algorithm name
        string AlgorithmName { get; }

        // return the size, in bytes, of the digest produced by this message digest.
		int GetDigestSize();

        // return the size, in bytes, of the internal buffer used by this digest.
		int GetByteLength();

        // update the message digest with a single byte.
        // Input: the input byte to be entered.
        void Update(byte input);

        // update the message digest with a block of bytes.
        // Input: input - the byte array containing the data.
        //        inOff - the offset into the byte array where the data starts.
        //        len   - the length of the data.
        void BlockUpdate(byte[] input, int inOff, int length);

         // Close the digest, producing the final digest value. The doFinal call leaves the digest reset.
         // Input:  output - the array the digest is to be copied into.
         //         outOff - the offset into the out array the digest is to start at.
        int DoFinal(byte[] output, int outOff);

        // reset the digest back to it's initial state.
        void Reset();
    }
}
