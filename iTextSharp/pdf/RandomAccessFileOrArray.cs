using System;
using System.IO;
using System.Text;
using iTextSharp.text.exceptions;
using iTextSharp.text.io;


namespace iTextSharp.text.pdf 
{
    /** Intended to be layered on top of a low level RandomAccessSource object.  Provides
    * functionality useful during parsing:
    * <ul>
    * 	<li>tracks current position in the file</li>
    * 	<li>allows single byte pushback</li>
    * 	<li>allows reading of multi-byte data structures (int, long, String) for both Big and Little Endian representations</li>
    * 	<li>allows creation of independent 'views' of the underlying data source</li>
    * </ul> 
    * 
    * @author Paulo Soares, Kevin Day
    */

    public class RandomAccessFileOrArray
    {

        /**
         * The source that backs this object
         */
        private readonly IRandomAccessSource byteSource;

        /**
         * The physical location in the underlying byte source.
         */
        private long byteSourcePosition;

        /**
         * the pushed  back byte, if any
         */
        private byte back;
        /**
         * Whether there is a pushed back byte
         */
        private bool isBack = false;

        /**
         * @deprecated use {@link RandomAccessFileOrArray#RandomAccessFileOrArray(RandomAccessSource)} instead
         * @param filename
         * @throws IOException
         */

        public RandomAccessFileOrArray(String filename)
            : this(new RandomAccessSourceFactory()
                       .SetForceRead(false)
                       .CreateBestSource(filename))
        {
        }

        /**
         * Creates an independent view of the specified source.  Closing the new object will not close the source.
         * Closing the source will have adverse effect on the behavior of the new view.
         * @deprecated use {@link RandomAccessFileOrArray#createView()} instead
         * @param source the source for the new independent view
         */

        public RandomAccessFileOrArray(RandomAccessFileOrArray source)
            : this(new IndependentRandomAccessSource(source.byteSource))
        {
        }

        /**
         * Creates an independent view of this object (with it's own file pointer and pushback queue).  Closing the new object will not close this object.
         * Closing this object will have adverse effect on the view.
         * @return the new view
         */

        public RandomAccessFileOrArray CreateView()
        {
            return new RandomAccessFileOrArray(new IndependentRandomAccessSource(byteSource));
        }

        public IRandomAccessSource CreateSourceView()
        {
            return new IndependentRandomAccessSource(byteSource);
        }

        /**
         * Creates a RandomAccessFileOrArray that wraps the specified byte source.  The byte source will be closed when
         * this RandomAccessFileOrArray is closed.
         * @param byteSource the byte source to wrap
         */

        public RandomAccessFileOrArray(IRandomAccessSource byteSource)
        {
            this.byteSource = byteSource;
        }

        /**
         * Constructs a new RandomAccessFileOrArrayObject
         * @param filename the file to open (can be a file system file or one of the following url strings: file://, http://, https://, jar:, wsjar:, vfszip:
         * @param forceRead if true, the entire file will be read into memory
         * @param plainRandomAccess if true, a regular RandomAccessFile is used to access the file contents.  If false, a memory mapped file will be used, unless the file cannot be mapped into memory, in which case regular RandomAccessFile will be used
         * @throws IOException if there is a failure opening or reading the file
         * @deprecated use {@link RandomAccessSourceFactory#createBestSource(String)} and {@link RandomAccessFileOrArray#RandomAccessFileOrArray(RandomAccessSource)} instead
         */

        public RandomAccessFileOrArray(String filename, bool forceRead)
            : this(new RandomAccessSourceFactory()
                       .SetForceRead(forceRead)
                       .CreateBestSource(filename))
        {
        }

        /**
         * @param url
         * @throws IOException
         * @deprecated use {@link RandomAccessSourceFactory#createSource(URL)} and {@link RandomAccessFileOrArray#RandomAccessFileOrArray(RandomAccessSource)} instead
         */

        public RandomAccessFileOrArray(Uri url)
            : this(new RandomAccessSourceFactory().CreateSource(url))
        {
        }

        /**
         * @param is
         * @throws IOException
         * @deprecated use {@link RandomAccessSourceFactory#createSource(InputStream)} and {@link RandomAccessFileOrArray#RandomAccessFileOrArray(RandomAccessSource)} instead
         */
        public RandomAccessFileOrArray(Stream inp)
            : this(new RandomAccessSourceFactory().CreateSource(inp))
        {
        }


        /**
         * @param arrayIn
         * @throws IOException
         * @deprecated use {@link RandomAccessSourceFactory#createSource(byte[])} and {@link RandomAccessFileOrArray#RandomAccessFileOrArray(RandomAccessSource)} instead
         */
        public RandomAccessFileOrArray(byte[] arrayIn)
            : this(new RandomAccessSourceFactory().CreateSource(arrayIn))
        {
        }

        //TODO: I'm only putting this in here for backwards compatability with PdfReader(RAFOA, byte[]).  Once we get rid of the
        //PdfReader constructor, we can get rid of this method as well
        protected internal IRandomAccessSource GetByteSource()
        {
            return byteSource;
        }

        /**
         * Pushes a byte back.  The next get() will return this byte instead of the value from the underlying data source
         * @param b the byte to push
         */

        public void PushBack(byte b)
        {
            back = b;
            isBack = true;
        }

        /**
         * Reads a single byte
         * @return the byte, or -1 if EOF is reached
         * @throws IOException
         */

        public int Read()
        {
            if (isBack)
            {
                isBack = false;
                return back & 0xff;
            }
            return byteSource.Get(byteSourcePosition++);
        }

        public int Read(byte[] b, int off, int len)
        {
            if (len == 0)
                return 0;
            int count = 0;
            if (isBack && len > 0)
            {
                isBack = false;
                b[off++] = back;
                --len;
                count++;
            }
            if (len > 0)
            {
                int byteSourceCount = byteSource.Get(byteSourcePosition, b, off, len);
                if (byteSourceCount > 0)
                {
                    count += byteSourceCount;
                    byteSourcePosition += byteSourceCount;
                }
            }
            if (count == 0)
                return -1;
            return count;
        }

        public int Read(byte[] b)
        {
            return Read(b, 0, b.Length);
        }

        public void ReadFully(byte[] b)
        {
            ReadFully(b, 0, b.Length);
        }

        public void ReadFully(byte[] b, int off, int len)
        {
            if (len == 0)
                return;
            int n = 0;
            do
            {
                int count = Read(b, off + n, len - n);
                if (count <= 0)
                    throw new EndOfStreamException();
                n += count;
            } while (n < len);
        }

        public long Skip(long n)
        {
            return SkipBytes(n);
        }

        public long SkipBytes(long n)
        {
            if (n <= 0)
            {
                return 0;
            }
            int adj = 0;
            if (isBack)
            {
                isBack = false;
                if (n == 1)
                {
                    return 1;
                }
                else
                {
                    --n;
                    adj = 1;
                }
            }
            long pos;
            long len;
            long newpos;

            pos = FilePointer;
            len = Length;
            newpos = pos + n;
            if (newpos > len)
            {
                newpos = len;
            }
            Seek(newpos);

            /* return the actual number of bytes skipped */
            return newpos - pos + adj;
        }

        public void ReOpen()
        {
            Seek(0);
        }

        public void Close()
        {
            isBack = false;
            byteSource.Close();
        }

        public long Length
        {
            get { return byteSource.Length; }
        }

        public void Seek(long pos)
        {
            byteSourcePosition = pos;
            isBack = false;
        }

        public void Seek(int pos)
        {
            Seek((long) pos);
        }

        public long FilePointer
        {
            get { return byteSourcePosition - (isBack ? 1 : 0); }
        }

        public bool ReadBoolean()
        {
            int ch = this.Read();
            if (ch < 0)
                throw new EndOfStreamException();
            return (ch != 0);
        }

        public byte ReadByte()
        {
            int ch = this.Read();
            if (ch < 0)
                throw new EndOfStreamException();
            return (byte) (ch);
        }

        public int ReadUnsignedByte()
        {
            int ch = this.Read();
            if (ch < 0)
                throw new EndOfStreamException();
            return ch;
        }

        public short ReadShort()
        {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
                throw new EndOfStreamException();
            return (short) ((ch1 << 8) + ch2);
        }

        /**
        * Reads a signed 16-bit number from this stream in little-endian order.
        * The method reads two
        * bytes from this stream, starting at the current stream pointer.
        * If the two bytes read, in order, are
        * <code>b1</code> and <code>b2</code>, where each of the two values is
        * between <code>0</code> and <code>255</code>, inclusive, then the
        * result is equal to:
        * <blockquote><pre>
        *     (short)((b2 &lt;&lt; 8) | b1)
        * </pre></blockquote>
        * <p>
        * This method blocks until the two bytes are read, the end of the
        * stream is detected, or an exception is thrown.
        *
        * @return     the next two bytes of this stream, interpreted as a signed
        *             16-bit number.
        * @exception  EOFException  if this stream reaches the end before reading
        *               two bytes.
        * @exception  IOException   if an I/O error occurs.
        */

        public short ReadShortLE()
        {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
                throw new EndOfStreamException();
            return (short) ((ch2 << 8) + (ch1 << 0));
        }

        public int ReadUnsignedShort()
        {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
                throw new EndOfStreamException();
            return (ch1 << 8) + ch2;
        }

        /**
        * Reads an unsigned 16-bit number from this stream in little-endian order.
        * This method reads
        * two bytes from the stream, starting at the current stream pointer.
        * If the bytes read, in order, are
        * <code>b1</code> and <code>b2</code>, where
        * <code>0&nbsp;&lt;=&nbsp;b1, b2&nbsp;&lt;=&nbsp;255</code>,
        * then the result is equal to:
        * <blockquote><pre>
        *     (b2 &lt;&lt; 8) | b1
        * </pre></blockquote>
        * <p>
        * This method blocks until the two bytes are read, the end of the
        * stream is detected, or an exception is thrown.
        *
        * @return     the next two bytes of this stream, interpreted as an
        *             unsigned 16-bit integer.
        * @exception  EOFException  if this stream reaches the end before reading
        *               two bytes.
        * @exception  IOException   if an I/O error occurs.
        */

        public int ReadUnsignedShortLE()
        {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
                throw new EndOfStreamException();
            return (ch2 << 8) + (ch1 << 0);
        }

        public char ReadChar()
        {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
                throw new EndOfStreamException();
            return (char) ((ch1 << 8) + ch2);
        }

        /**
        * Reads a Unicode character from this stream in little-endian order.
        * This method reads two
        * bytes from the stream, starting at the current stream pointer.
        * If the bytes read, in order, are
        * <code>b1</code> and <code>b2</code>, where
        * <code>0&nbsp;&lt;=&nbsp;b1,&nbsp;b2&nbsp;&lt;=&nbsp;255</code>,
        * then the result is equal to:
        * <blockquote><pre>
        *     (char)((b2 &lt;&lt; 8) | b1)
        * </pre></blockquote>
        * <p>
        * This method blocks until the two bytes are read, the end of the
        * stream is detected, or an exception is thrown.
        *
        * @return     the next two bytes of this stream as a Unicode character.
        * @exception  EOFException  if this stream reaches the end before reading
        *               two bytes.
        * @exception  IOException   if an I/O error occurs.
        */

        public char ReadCharLE()
        {
            int ch1 = this.Read();
            int ch2 = this.Read();
            if ((ch1 | ch2) < 0)
                throw new EndOfStreamException();
            return (char) ((ch2 << 8) + (ch1 << 0));
        }

        public int ReadInt()
        {
            int ch1 = this.Read();
            int ch2 = this.Read();
            int ch3 = this.Read();
            int ch4 = this.Read();
            if ((ch1 | ch2 | ch3 | ch4) < 0)
                throw new EndOfStreamException();
            return ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + ch4);
        }

        /**
        * Reads a signed 32-bit integer from this stream in little-endian order.
        * This method reads 4
        * bytes from the stream, starting at the current stream pointer.
        * If the bytes read, in order, are <code>b1</code>,
        * <code>b2</code>, <code>b3</code>, and <code>b4</code>, where
        * <code>0&nbsp;&lt;=&nbsp;b1, b2, b3, b4&nbsp;&lt;=&nbsp;255</code>,
        * then the result is equal to:
        * <blockquote><pre>
        *     (b4 &lt;&lt; 24) | (b3 &lt;&lt; 16) + (b2 &lt;&lt; 8) + b1
        * </pre></blockquote>
        * <p>
        * This method blocks until the four bytes are read, the end of the
        * stream is detected, or an exception is thrown.
        *
        * @return     the next four bytes of this stream, interpreted as an
        *             <code>int</code>.
        * @exception  EOFException  if this stream reaches the end before reading
        *               four bytes.
        * @exception  IOException   if an I/O error occurs.
        */

        public int ReadIntLE()
        {
            int ch1 = this.Read();
            int ch2 = this.Read();
            int ch3 = this.Read();
            int ch4 = this.Read();
            if ((ch1 | ch2 | ch3 | ch4) < 0)
                throw new EndOfStreamException();
            return ((ch4 << 24) + (ch3 << 16) + (ch2 << 8) + (ch1 << 0));
        }

        /**
        * Reads an unsigned 32-bit integer from this stream. This method reads 4
        * bytes from the stream, starting at the current stream pointer.
        * If the bytes read, in order, are <code>b1</code>,
        * <code>b2</code>, <code>b3</code>, and <code>b4</code>, where
        * <code>0&nbsp;&lt;=&nbsp;b1, b2, b3, b4&nbsp;&lt;=&nbsp;255</code>,
        * then the result is equal to:
        * <blockquote><pre>
        *     (b1 &lt;&lt; 24) | (b2 &lt;&lt; 16) + (b3 &lt;&lt; 8) + b4
        * </pre></blockquote>
        * <p>
        * This method blocks until the four bytes are read, the end of the
        * stream is detected, or an exception is thrown.
        *
        * @return     the next four bytes of this stream, interpreted as a
        *             <code>long</code>.
        * @exception  EOFException  if this stream reaches the end before reading
        *               four bytes.
        * @exception  IOException   if an I/O error occurs.
        */

        public long ReadUnsignedInt()
        {
            long ch1 = this.Read();
            long ch2 = this.Read();
            long ch3 = this.Read();
            long ch4 = this.Read();
            if ((ch1 | ch2 | ch3 | ch4) < 0)
                throw new EndOfStreamException();
            return ((ch1 << 24) + (ch2 << 16) + (ch3 << 8) + (ch4 << 0));
        }

        public long ReadUnsignedIntLE()
        {
            long ch1 = this.Read();
            long ch2 = this.Read();
            long ch3 = this.Read();
            long ch4 = this.Read();
            if ((ch1 | ch2 | ch3 | ch4) < 0)
                throw new EndOfStreamException();
            return ((ch4 << 24) + (ch3 << 16) + (ch2 << 8) + (ch1 << 0));
        }

        public long ReadLong()
        {
            return ((long) (ReadInt()) << 32) + (ReadInt() & 0xFFFFFFFFL);
        }

        public long ReadLongLE()
        {
            int i1 = ReadIntLE();
            int i2 = ReadIntLE();
            return ((long) i2 << 32) + (i1 & 0xFFFFFFFFL);
        }

        public float ReadFloat()
        {
            int[] a = {ReadInt()};
            float[] b = {0};
            Buffer.BlockCopy(a, 0, b, 0, 4);
            return b[0];
        }

        public float ReadFloatLE()
        {
            int[] a = {ReadIntLE()};
            float[] b = {0};
            Buffer.BlockCopy(a, 0, b, 0, 4);
            return b[0];
        }

        public double ReadDouble()
        {
            long[] a = {ReadLong()};
            double[] b = {0};
            Buffer.BlockCopy(a, 0, b, 0, 8);
            return b[0];
        }

        public double ReadDoubleLE()
        {
            long[] a = {ReadLongLE()};
            double[] b = {0};
            Buffer.BlockCopy(a, 0, b, 0, 8);
            return b[0];
        }

        public String ReadLine()
        {
            StringBuilder input = new StringBuilder();
            int c = -1;
            bool eol = false;

            while (!eol)
            {
                switch (c = Read())
                {
                    case -1:
                    case '\n':
                        eol = true;
                        break;
                    case '\r':
                        eol = true;
                        long cur = FilePointer;
                        if ((Read()) != '\n')
                        {
                            Seek(cur);
                        }
                        break;
                    default:
                        input.Append((char) c);
                        break;
                }
            }

            if ((c == -1) && (input.Length == 0))
                return null;
            return input.ToString();
        }


        public String ReadString(int length, String encoding)
        {
            byte[] buf = new byte[length];
            ReadFully(buf);
            Encoding enc = Encoding.GetEncoding(encoding);
            return enc.GetString(buf);
        }
    }
}
