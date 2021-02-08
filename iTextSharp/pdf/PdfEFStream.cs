using System;
using System.IO;
using System.Collections;

using CipherBox.Pdf.Utility.Zlib;

namespace iTextSharp.text.pdf 
{
    /**
    * Extends PdfStream and should be used to create Streams for Embedded Files
    * (file attachments).
    * @since	2.1.3
    */

    public class PdfEFStream : PdfStream {

	    /**
	    * Creates a Stream object using an InputStream and a PdfWriter object
	    * @param	in	the InputStream that will be read to get the Stream object
	    * @param	writer	the writer to which the stream will be added
	    */
	    public PdfEFStream(Stream inp, PdfWriter writer) : base (inp, writer) {
	    }

	    /**
	    * Creates a Stream object using a byte array
	    * @param	fileStore	the bytes for the stream
	    */
	    public PdfEFStream(byte[] fileStore) : base(fileStore) {
	    }

        /**
        * @see com.lowagie.text.pdf.PdfDictionary#toPdf(com.lowagie.text.pdf.PdfWriter, java.io.OutputStream)
        */
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
                        PdfArray a = (PdfArray)filter;
                        if (!a.IsEmpty() && PdfName.CRYPT.Equals(a[0]))
                            crypto = null;
                    }
                }
            }
    	    if (crypto != null && crypto.IsEmbeddedFilesOnly()) {
    		    PdfArray filter = new PdfArray();
    		    PdfArray decodeparms = new PdfArray();
    		    PdfDictionary crypt = new PdfDictionary();
    		    crypt.Put(PdfName.NAME, PdfName.STDCF);
    		    filter.Add(PdfName.CRYPT);
    		    decodeparms.Add(crypt);
    		    if (compressed) {
    			    filter.Add(PdfName.FLATEDECODE);
    			    decodeparms.Add(new PdfNull());
    		    }
    		    Put(PdfName.FILTER, filter);
    		    Put(PdfName.DECODEPARMS, decodeparms);
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
                if (crypto != null)
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
                if (crypto == null) {
                    if (streamBytes != null)
                        streamBytes.WriteTo(os);
                    else
                        os.Write(bytes, 0, bytes.Length);
                }
                else {
                    byte[] b;
                    if (streamBytes != null) {
                        b = crypto.EncryptByteArray(streamBytes.ToArray());
                    }
                    else {
                        b = crypto.EncryptByteArray(bytes);
                    }
                    os.Write(b, 0, b.Length);
                }
            }
            os.Write(ENDSTREAM, 0, ENDSTREAM.Length);
        }
    }
}
