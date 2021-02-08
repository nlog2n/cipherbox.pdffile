using System;

using iTextSharp.text;

namespace iTextSharp.text.pdf 
{
    /**
     * Signals that an unspecified problem while constructing a PDF document.
     *
     * @see        BadPdfFormatException
     */

    public class PdfException : DocumentException 
    {    
        public PdfException() : base() {}

        public PdfException(string message) : base(message) {}
    }
}
