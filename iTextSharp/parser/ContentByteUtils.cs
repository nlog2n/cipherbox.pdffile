using System;
using System.IO;

using CipherBox.Pdf.Utility;

using iTextSharp.text.pdf;

namespace CipherBox.Pdf.Parser
{
    public class ContentByteUtils
    {
        private ContentByteUtils()
        {
        }

        /**
         * Gets the content bytes from a content object, which may be a reference
         * a stream or an array.
         * @param contentObject the object to read bytes from
         * @return the content bytes
         * @throws IOException
         */
        public static byte[] GetContentBytesFromContentObject(PdfObject contentObject)
        {
            byte[] result;
            switch (contentObject.Type)
            {
                case PdfObject.INDIRECT:
                    PRIndirectReference refi = (PRIndirectReference)contentObject;
                    PdfObject directObject = PdfReader.GetPdfObjectRelease(refi);
                    result = GetContentBytesFromContentObject(directObject);
                    break;
                case PdfObject.STREAM:
                    PRStream stream = (PRStream)PdfReader.GetPdfObjectRelease(contentObject);
                    result = PdfReader.GetStreamBytes(stream);
                    break;
                case PdfObject.ARRAY:
                    // Stitch together all content before calling ProcessContent(), because
                    // ProcessContent() resets state.
                    MemoryStream allBytes = new MemoryStream();
                    PdfArray contentArray = (PdfArray)contentObject;
                    ListIterator<PdfObject> iter = contentArray.GetListIterator();
                    while (iter.HasNext())
                    {
                        PdfObject element = iter.Next();
                        byte[] b;
                        allBytes.Write(b = GetContentBytesFromContentObject(element), 0, b.Length);
                        allBytes.WriteByte((byte)' ');
                    }
                    result = allBytes.ToArray();
                    break;
                default:
                    String msg = "Unable to handle Content of type " + contentObject.GetType();
                    throw new InvalidOperationException(msg);
            }
            return result;
        }

        /**
         * Gets the content bytes of a page from a reader
         * @param reader  the reader to get content bytes from
         * @param pageNum   the page number of page you want get the content stream from
         * @return  a byte array with the effective content stream of a page
         * @throws IOException
         * @since 5.0.1
         */
        public static byte[] GetContentBytesForPage(PdfReader reader, int pageNum)
        {
            PdfDictionary pageDictionary = reader.GetPageN(pageNum);
            PdfObject contentObject = pageDictionary.Get(PdfName.CONTENTS);
            if (contentObject == null)
                return new byte[0];

            byte[] contentBytes = ContentByteUtils.GetContentBytesFromContentObject(contentObject);
            return contentBytes;
        }
    }
}