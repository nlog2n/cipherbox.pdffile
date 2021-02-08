using System;
using System.IO;

using CipherBox.Pdf.Utility.Zlib;

namespace iTextSharp.text.pdf
{
    public class PRStream : PdfStream
    {
        protected PdfReader reader;
        protected long offset;
        protected int length;

        //added by ujihara for decryption
        protected int objNum = 0;
        protected int objGen = 0;

        public PRStream(PRStream stream, PdfDictionary newDic)
        {
            reader = stream.reader;
            offset = stream.offset;
            length = stream.Length;
            compressed = stream.compressed;
            compressionLevel = stream.compressionLevel;
            streamBytes = stream.streamBytes;
            bytes = stream.bytes;
            objNum = stream.objNum;
            objGen = stream.objGen;
            if (newDic != null)
                Merge(newDic);
            else
                Merge(stream);
        }

        public PRStream(PRStream stream, PdfDictionary newDic, PdfReader reader)
            : this(stream, newDic)
        {
            this.reader = reader;
        }

        public PRStream(PdfReader reader, long offset)
        {
            this.reader = reader;
            this.offset = offset;
        }

        public PRStream(PdfReader reader, byte[] conts)
            : this(reader, conts, DEFAULT_COMPRESSION)
        {
        }

        /**
         * Creates a new PDF stream object that will replace a stream
         * in a existing PDF file.
         * @param   reader  the reader that holds the existing PDF
         * @param   conts   the new content
         * @param   compressionLevel    the compression level for the content
         * @since   2.1.3 (replacing the existing constructor without param compressionLevel)
         */
        public PRStream(PdfReader reader, byte[] conts, int compressionLevel)
        {
            this.reader = reader;
            this.offset = -1;
            if (Document.Compress)
            {
                MemoryStream stream = new MemoryStream();
                ZDeflaterOutputStream zip = new ZDeflaterOutputStream(stream, compressionLevel);
                zip.Write(conts, 0, conts.Length);
                zip.Close();
                bytes = stream.ToArray();
                Put(PdfName.FILTER, PdfName.FLATEDECODE);
            }
            else
                bytes = conts;
            Length = bytes.Length;
        }

        /**
         * Sets the data associated with the stream, either compressed or
         * uncompressed. Note that the data will never be compressed if
         * Document.compress is set to false.
         * 
         * @param data raw data, decrypted and uncompressed.
         * @param compress true if you want the stream to be compresssed.
         * @since   iText 2.1.1
         */
        public void SetData(byte[] data, bool compress)
        {
            SetData(data, compress, DEFAULT_COMPRESSION);
        }

        /**
         * Sets the data associated with the stream, either compressed or
         * uncompressed. Note that the data will never be compressed if
         * Document.compress is set to false.
         * 
         * @param data raw data, decrypted and uncompressed.
         * @param compress true if you want the stream to be compresssed.
         * @param compressionLevel  a value between -1 and 9 (ignored if compress == false)
         * @since   iText 2.1.3
         */
        public void SetData(byte[] data, bool compress, int compressionLevel)
        {
            Remove(PdfName.FILTER);
            this.offset = -1;
            if (Document.Compress && compress)
            {
                MemoryStream stream = new MemoryStream();
                ZDeflaterOutputStream zip = new ZDeflaterOutputStream(stream, compressionLevel);
                zip.Write(data, 0, data.Length);
                zip.Close();
                bytes = stream.ToArray();
                this.compressionLevel = compressionLevel;
                Put(PdfName.FILTER, PdfName.FLATEDECODE);
            }
            else
                bytes = data;
            Length = bytes.Length;
        }

        /**Sets the data associated with the stream
         * @param data raw data, decrypted and uncompressed.
         */
        public void SetData(byte[] data)
        {
            SetData(data, true);
        }

        public new int Length
        {
            set
            {
                length = value;
                Put(PdfName.LENGTH, new PdfNumber(length));
            }
            get
            {
                return length;
            }
        }

        public long Offset
        {
            get
            {
                return offset;
            }
        }

        public PdfReader Reader
        {
            get
            {
                return reader;
            }
        }

        public new byte[] GetBytes()
        {
            return bytes;
        }

        public int ObjNum
        {
            get
            {
                return objNum;
            }
            set
            {
                objNum = value;
            }
        }

        public int ObjGen
        {
            get
            {
                return objGen;
            }
            set
            {
                objGen = value;
            }
        }

        public override void ToPdf(PdfWriter writer, Stream os)
        {
            byte[] b = PdfReader.GetStreamBytesRaw(this);
            PdfEncryption crypto = null;
            if (writer != null)
                crypto = writer.Encryption;
            PdfObject objLen = Get(PdfName.LENGTH);
            int nn = b.Length;
            if (crypto != null)
                nn = crypto.CalculateStreamSize(nn);
            Put(PdfName.LENGTH, new PdfNumber(nn));
            SuperToPdf(writer, os);
            Put(PdfName.LENGTH, objLen);
            os.Write(STARTSTREAM, 0, STARTSTREAM.Length);
            if (length > 0)
            {
                if (crypto != null && !crypto.IsEmbeddedFilesOnly())
                    b = crypto.EncryptByteArray(b);
                os.Write(b, 0, b.Length);
            }
            os.Write(ENDSTREAM, 0, ENDSTREAM.Length);
        }
    }
}
