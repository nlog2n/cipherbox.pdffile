using System;

namespace iTextSharp.text.io {

    /**
     * A RandomAccessSource that is based on an underlying byte array 
     * @since 5.3.5
     */
    internal class ArrayRandomAccessSource : IRandomAccessSource {
        private byte[] array;
        
        public ArrayRandomAccessSource(byte[] array) {
            if (array == null) 
                throw new ArgumentNullException();
            this.array = array;
        }

        public virtual int Get(long offset) {
            if (offset >= array.Length) return -1;
            return 0xff & array[(int)offset];
        }

        public virtual int Get(long offset, byte[] bytes, int off, int len) {
            if (array == null) throw new InvalidOperationException("Already closed");
            
            if (offset >= array.Length)
                return -1;
            
            if (offset + len > array.Length)
                len = (int)(array.Length - offset);
            
            System.Array.Copy(array, (int)offset, bytes, off, len);
            
            return len;

        }

        public virtual long Length {
            get {
                return array.Length;
            }
        }

        public virtual void Close() {
            array = null;
        }

        public void Dispose() {
            Close();
        }
    }
}