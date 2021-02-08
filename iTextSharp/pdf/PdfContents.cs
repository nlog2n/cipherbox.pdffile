using System;
using System.IO;

using CipherBox.Pdf.Utility.Zlib;

using iTextSharp.text;

namespace iTextSharp.text.pdf 
{
    /**
     * <CODE>PdfContents</CODE> is a <CODE>PdfStream</CODE> containing the contents (text + graphics) of a <CODE>PdfPage</CODE>.
     */

    public class PdfContents : PdfStream {
    
        internal static byte[] SAVESTATE = DocWriter.GetISOBytes("q\n");
        internal static byte[] RESTORESTATE = DocWriter.GetISOBytes("Q\n");
        internal static byte[] ROTATE90 = DocWriter.GetISOBytes("0 1 -1 0 ");
        internal static byte[] ROTATE180 = DocWriter.GetISOBytes("-1 0 0 -1 ");
        internal static byte[] ROTATE270 = DocWriter.GetISOBytes("0 -1 1 0 ");
        internal static byte[] ROTATEFINAL = DocWriter.GetISOBytes(" cm\n");
        // constructor
    
        /**
         * Constructs a <CODE>PdfContents</CODE>-object, containing text and general graphics.
         *
         * @param under the direct content that is under all others
         * @param content the graphics in a page
         * @param text the text in a page
         * @param secondContent the direct content that is over all others
         * @throws BadPdfFormatException on error
         */
    
        internal PdfContents(PdfContentByte under, PdfContentByte content, PdfContentByte text, PdfContentByte secondContent, Rectangle page) : base() {
            Stream ostr = null;
            streamBytes = new MemoryStream();
            if (Document.Compress) {
                compressed = true;
                int compresLevel;
                if (text != null)
                    compresLevel = text.PdfWriter.CompressionLevel;
                else
                    compresLevel = content.PdfWriter.CompressionLevel;
                ostr = new ZDeflaterOutputStream(streamBytes, compresLevel);
            }
            else
                ostr = streamBytes;
            int rotation = page.Rotation;
            byte[] tmp;
            switch (rotation) {
                case 90:
                    ostr.Write(ROTATE90, 0, ROTATE90.Length);
                    tmp = DocWriter.GetISOBytes(ByteBuffer.FormatDouble(page.Top));
                    ostr.Write(tmp, 0, tmp.Length);
                    ostr.WriteByte((byte)' ');
                    ostr.WriteByte((byte)'0');
                    ostr.Write(ROTATEFINAL, 0, ROTATEFINAL.Length);
                    break;
                case 180:
                    ostr.Write(ROTATE180, 0, ROTATE180.Length);
                    tmp = DocWriter.GetISOBytes(ByteBuffer.FormatDouble(page.Right));
                    ostr.Write(tmp, 0, tmp.Length);
                    ostr.WriteByte((byte)' ');
                    tmp = DocWriter.GetISOBytes(ByteBuffer.FormatDouble(page.Top));
                    ostr.Write(tmp, 0, tmp.Length);
                    ostr.Write(ROTATEFINAL, 0, ROTATEFINAL.Length);
                    break;
                case 270:
                    ostr.Write(ROTATE270, 0, ROTATE270.Length);
                    ostr.WriteByte((byte)'0');
                    ostr.WriteByte((byte)' ');
                    tmp = DocWriter.GetISOBytes(ByteBuffer.FormatDouble(page.Right));
                    ostr.Write(tmp, 0, tmp.Length);
                    ostr.Write(ROTATEFINAL, 0, ROTATEFINAL.Length);
                    break;
            }
            if (under.Size > 0) {
                ostr.Write(SAVESTATE, 0, SAVESTATE.Length);
                under.InternalBuffer.WriteTo(ostr);
                ostr.Write(RESTORESTATE, 0, RESTORESTATE.Length);
            }
            if (content.Size > 0) {
                ostr.Write(SAVESTATE, 0, SAVESTATE.Length);
                content.InternalBuffer.WriteTo(ostr);
                ostr.Write(RESTORESTATE, 0, RESTORESTATE.Length);
            }
            if (text != null) {
                ostr.Write(SAVESTATE, 0, SAVESTATE.Length);
                text.InternalBuffer.WriteTo(ostr);
                ostr.Write(RESTORESTATE, 0, RESTORESTATE.Length);
            }
            if (secondContent.Size > 0) {
                secondContent.InternalBuffer.WriteTo(ostr);
            }

            if (ostr is ZDeflaterOutputStream)
                ((ZDeflaterOutputStream)ostr).Finish();
            Put(PdfName.LENGTH, new PdfNumber(streamBytes.Length));
            if (compressed)
                Put(PdfName.FILTER, PdfName.FLATEDECODE);
        }
    }
}
