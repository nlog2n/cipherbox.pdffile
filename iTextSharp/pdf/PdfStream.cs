using System;
using System.IO;

using CipherBox.Pdf.Utility.Zlib;

namespace iTextSharp.text.pdf 
{
    /**
     * <CODE>PdfStream</CODE> is the Pdf stream object.
     * <P>
     * A stream, like a string, is a sequence of characters. However, an application can
     * read a small portion of a stream at a time, while a string must be read in its entirety.
     * For this reason, objects with potentially large amounts of data, such as images and
     * page descriptions, are represented as streams.<BR>
     * A stream consists of a dictionary that describes a sequence of characters, followed by
     * the keyword <B>stream</B>, followed by zero or more lines of characters, followed by
     * the keyword <B>endstream</B>.<BR>
     * All streams must be <CODE>PdfIndirectObject</CODE>s. The stream dictionary must be a direct
     * object. The keyword <B>stream</B> that follows the stream dictionary should be followed by
     * a carriage return and linefeed or just a linefeed.<BR>
     * Remark: In this version only the FLATEDECODE-filter is supported.<BR>
     * This object is described in the 'Portable Document Format Reference Manual version 1.3'
     * section 4.8 (page 41-53).<BR>
     * </P>
     *
     * @see        PdfObject
     * @see        PdfDictionary
     */

    public class PdfStream : PdfDictionary {
    
        // membervariables
    
        /**
        * A possible compression level.
        * @since   2.1.3
        */
        public const int DEFAULT_COMPRESSION = -1;
        /**
        * A possible compression level.
        * @since   2.1.3
        */
        public const int NO_COMPRESSION = 0;
        /**
        * A possible compression level.
        * @since   2.1.3
        */
        public const int BEST_SPEED = 1;
        /**
        * A possible compression level.
        * @since   2.1.3
        */
        public const int BEST_COMPRESSION = 9;        
        
        /** is the stream compressed? */
        protected bool compressed = false;

        /**
        * The level of compression.
        * @since   2.1.3
        */
        protected int compressionLevel = NO_COMPRESSION;
    
        protected MemoryStream streamBytes = null;
    
        protected Stream inputStream;
        protected PdfIndirectReference iref;
        protected int inputStreamLength = -1;
        protected PdfWriter writer;
        protected int rawLength;
        
        internal static byte[] STARTSTREAM = DocWriter.GetISOBytes("stream\n");
        internal static byte[] ENDSTREAM = DocWriter.GetISOBytes("\nendstream");
        internal static int SIZESTREAM = STARTSTREAM.Length + ENDSTREAM.Length;

        // constructors
    
        /**
         * Constructs a <CODE>PdfStream</CODE>-object.
         *
         * @param        bytes            content of the new <CODE>PdfObject</CODE> as an array of <CODE>byte</CODE>.
         */
 
        public PdfStream(byte[] bytes) : base() {
            type = STREAM;
            this.bytes = bytes;
            rawLength = bytes.Length;
            Put(PdfName.LENGTH, new PdfNumber(bytes.Length));
        }
  
        /**
         * Creates an efficient stream. No temporary array is ever created. The <CODE>InputStream</CODE>
         * is totally consumed but is not closed. The general usage is:
         * <pre>
         * InputStream in = ...;
         * PdfStream stream = new PdfStream(in, writer);
         * stream.FlateCompress();
         * writer.AddToBody(stream);
         * stream.WriteLength();
         * in.Close();
         * </pre>
         * @param inputStream the data to write to this stream
         * @param writer the <CODE>PdfWriter</CODE> for this stream
         */    
        public PdfStream(Stream inputStream, PdfWriter writer) {
            type = STREAM;
            this.inputStream = inputStream;
            this.writer = writer;
            iref = writer.PdfIndirectReference;
            Put(PdfName.LENGTH, iref);
        }

        /**
         * Constructs a <CODE>PdfStream</CODE>-object.
         */
    
        protected PdfStream() : base() {
            type = STREAM;
        }
    
        // methods overriding some methods of PdfObject
    
        /**
         * Writes the stream length to the <CODE>PdfWriter</CODE>.
         * <p>
         * This method must be called and can only be called if the contructor {@link #PdfStream(InputStream,PdfWriter)}
         * is used to create the stream.
         * </p>
         * @throws IOException on error
         * @see #PdfStream(InputStream,PdfWriter)
         */
        public void WriteLength() {
            if (inputStream == null)
                throw new PdfException("writelength.can.only.be.called.in.a.contructed.pdfstream.inputstream.pdfwriter");
            if (inputStreamLength == -1)
                throw new PdfException("writelength.can.only.be.called.after.output.of.the.stream.body");
            writer.AddToBody(new PdfNumber(inputStreamLength), iref, false);
        }
    
        public int RawLength {
            get {
                return rawLength;
            }
        }

        // methods
    
        /**
        * Compresses the stream.
        */
        public void FlateCompress() {
            FlateCompress(DEFAULT_COMPRESSION);
        }
        
        /**
        * Compresses the stream.
        * @param compressionLevel the compression level (0 = best speed, 9 = best compression, -1 is default)
        * @since   2.1.3
        */
        public void FlateCompress(int compressionLevel) {
            if (!Document.Compress)
                return;
            // check if the flateCompress-method has already been used
            if (compressed) {
                return;
            }
            this.compressionLevel = compressionLevel;
            if (inputStream != null) {
                compressed = true;
                return;
            }
            // check if a filter already exists
            PdfObject filter = PdfReader.GetPdfObject(Get(PdfName.FILTER));
            if (filter != null) {
                if (filter.IsName()) {
                    if (PdfName.FLATEDECODE.Equals(filter))
                        return;
                }
                else if (filter.IsArray()) {
                    if (((PdfArray) filter).Contains(PdfName.FLATEDECODE))
                        return;
                }
                else {
                    throw new PdfException("stream.could.not.be.compressed.filter.is.not.a.name.or.array");
                }
            }
            // compress
            MemoryStream stream = new MemoryStream();
            ZDeflaterOutputStream zip = new ZDeflaterOutputStream(stream, compressionLevel);
            if (streamBytes != null)
                streamBytes.WriteTo(zip);
            else
                zip.Write(bytes, 0, bytes.Length);
            //zip.Close();
            zip.Finish();
            // update the object
            streamBytes = stream;
            bytes = null;
            Put(PdfName.LENGTH, new PdfNumber(streamBytes.Length));
            if (filter == null) {
                Put(PdfName.FILTER, PdfName.FLATEDECODE);
            }
            else {
                PdfArray filters = new PdfArray(filter);
                filters.Add(PdfName.FLATEDECODE);
                Put(PdfName.FILTER, filters);
            }
            compressed = true;
        }

        protected virtual void SuperToPdf(PdfWriter writer, Stream os) {
            base.ToPdf(writer, os);
        }
    
        public override void ToPdf(PdfWriter writer, Stream os) {
            if (inputStream != null && compressed)
                Put(PdfName.FILTER, PdfName.FLATEDECODE);
            PdfEncryption crypto = null;
            if (writer != null)
                crypto = writer.Encryption;
            if (crypto != null) {
                PdfObject filter = Get(PdfName.FILTER);
                if (filter != null) {
                    if (PdfName.CRYPT.Equals(filter))
                        crypto = null;
                    else if (filter.IsArray()) {
                        PdfArray a = ((PdfArray)filter);
                        if (a.Size > 0 && PdfName.CRYPT.Equals(a[0]))
                            crypto = null;
                    }
                }
            }
            PdfObject nn = Get(PdfName.LENGTH);
            if (crypto != null && nn != null && nn.IsNumber()) {
                int sz = ((PdfNumber)nn).IntValue;
                Put(PdfName.LENGTH, new PdfNumber(crypto.CalculateStreamSize(sz)));
                SuperToPdf(writer, os);
                Put(PdfName.LENGTH, nn);
            }
            else
                SuperToPdf(writer, os);
            os.Write(STARTSTREAM, 0, STARTSTREAM.Length);
            if (inputStream != null) {
                rawLength = 0;
                ZDeflaterOutputStream def = null;
                OutputStreamCounter osc = new OutputStreamCounter(os);
                OutputStreamEncryption ose = null;
                Stream fout = osc;
                if (crypto != null && !crypto.IsEmbeddedFilesOnly())
                    fout = ose = crypto.GetEncryptionStream(fout);
                if (compressed)    
                    fout = def = new ZDeflaterOutputStream(fout, compressionLevel);
                
                byte[] buf = new byte[4192];
                while (true) {
                    int n = inputStream.Read(buf, 0, buf.Length);
                    if (n <= 0)
                        break;
                    fout.Write(buf, 0, n);
                    rawLength += n;
                }
                if (def != null)
                    def.Finish();
                if (ose != null)
                    ose.Finish();
                inputStreamLength = (int)osc.Counter;
            }
            else {
                if (crypto != null && !crypto.IsEmbeddedFilesOnly()) {
                    byte[] b;
                    if (streamBytes != null) {
                        b = crypto.EncryptByteArray(streamBytes.ToArray());
                    }
                    else {
                        b = crypto.EncryptByteArray(bytes);
                    }
                    os.Write(b, 0, b.Length);
                }
                else {
                    if (streamBytes != null)
                        streamBytes.WriteTo(os);
                    else
                        os.Write(bytes, 0, bytes.Length);
                }
            }
            os.Write(ENDSTREAM, 0, ENDSTREAM.Length);
        }
    
        /**
        * Writes the data content to an <CODE>Stream</CODE>.
        * @param os the destination to write to
        * @throws IOException on error
        */    
        public void WriteContent(Stream os) {
            if (streamBytes != null)
                streamBytes.WriteTo(os);
            else if (bytes != null)
                os.Write(bytes, 0, bytes.Length);
        }

        /**
        * @see com.lowagie.text.pdf.PdfObject#toString()
        */
        public override String ToString() {
            if (Get(PdfName.TYPE) == null) return "Stream";
            return "Stream of type: " + Get(PdfName.TYPE);
        }
    }
}
