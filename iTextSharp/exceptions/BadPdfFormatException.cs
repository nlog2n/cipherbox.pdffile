using System;

namespace iTextSharp.text.pdf
{
    /**
     * Signals that a bad PDF format has been used to construct a <CODE>PdfObject</CODE>.
     *
     * @see        PdfException
     * @see        PdfBoolean
     * @see        PdfNumber
     * @see        PdfString
     * @see        PdfName
     * @see        PdfDictionary
     * @see        PdfFont
     */

    public class BadPdfFormatException : Exception
    {
        public BadPdfFormatException() : base() {}
        public BadPdfFormatException(string message) : base(message) {}
    }
}
