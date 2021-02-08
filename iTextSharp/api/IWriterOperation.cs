using System;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace iTextSharp.text.api {

    /**
     * @author itextpdf.com
     *
     */
    public interface IWriterOperation {
        /**
         * Receive a writer and the document to do certain operations on them.
         * @param writer the PdfWriter
         * @param doc the document
         * @throws DocumentException
         */
        void Write(PdfWriter writer, Document doc);
    }
}