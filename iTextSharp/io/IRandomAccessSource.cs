using System;

namespace iTextSharp.text.io 
{

    /**
     * Represents an abstract source that bytes can be read from.  This class forms the foundation for all byte input in iText. 
     * Implementations do not keep track of a current 'position', but rather provide absolute get methods.  Tracking position
     * should be handled in classes that use RandomAccessSource internally (via composition).
     * @since 5.3.5
     */
    public interface IRandomAccessSource : IDisposable 
    {
        /**
         * Gets a byte at the specified position
         * @param position
         * @return the byte, or -1 if EOF is reached
         */
        int Get(long position);
        
        /**
         * Gets an array at the specified position.  If the number of bytes requested cannot be read, the bytes that can be
         * read will be placed in bytes and the number actually read will be returned.
         * @param position the position in the RandomAccessSource to read from
         * @param bytes output buffer
         * @param off offset into the output buffer where results will be placed
         * @param len the number of bytes to read
         * @return the number of bytes actually read, or -1 if the file is at EOF
         */
        int Get(long position, byte[] bytes, int off, int len);
        
        /**
         * @return the length of this source
         */
        long Length {get;}
        
        /**
         * Closes this source.  The underlying data structure or source (if any) will also be closed
         * @throws IOException
         */
        void Close();
    }
}