using System;

namespace iTextSharp.text.io {

    /**
     * @since 5.3.5
     */
    public class GetBufferedRandomAccessSource : IRandomAccessSource {
        /**
         * The source
         */
        private readonly IRandomAccessSource source;
        
        private readonly byte[] getBuffer;
        private long getBufferStart = -1;
        private long getBufferEnd = -1;
        
        /**
         * Constructs a new OffsetRandomAccessSource
         * @param source the source
         */
        public GetBufferedRandomAccessSource(IRandomAccessSource source) {
            this.source = source;
            
            this.getBuffer = new byte[(int)Math.Min(Math.Max(source.Length/4, 1), (long)4096)];
            this.getBufferStart = -1;
            this.getBufferEnd = -1;

        }

        /**
         * {@inheritDoc}
         */
        public virtual int Get(long position) {
            if (position < getBufferStart || position > getBufferEnd){
                int count = source.Get(position, getBuffer, 0, getBuffer.Length);
                if (count == -1)
                    return -1;
                getBufferStart = position;
                getBufferEnd = position + count - 1;
            }
            int bufPos = (int)(position-getBufferStart);
            return 0xff & getBuffer[bufPos];
        }

        /**
         * {@inheritDoc}
         */
        public virtual int Get(long position, byte[] bytes, int off, int len) {
            return source.Get(position, bytes, off, len);
        }

        /**
         * {@inheritDoc}
         */
        public virtual long Length {
            get {
                return source.Length;
            }
        }

        /**
         * {@inheritDoc}
         */
        public virtual void Close() {
            source.Close();
            getBufferStart = -1;
            getBufferEnd = -1;
        }

        public void Dispose() {
            Close();
        }
    }
}