using System;

namespace iTextSharp.text.exceptions 
{
    /**
     * Typed exception used when opening an existing PDF document.
     * Gets thrown when the document isn't a valid PDF document according to iText,
     * but it's different from the InvalidPdfException in the sense that it may
     * be an iText limitation (most of the times it isn't but you might have
     * bumped into something that has been added to the PDF specs, but that isn't
     * supported in iText yet).
     * @since 2.1.5
     */
    public class UnsupportedPdfException : InvalidPdfException {

        /**
         * Creates an instance of an UnsupportedPdfException.
         * @param	message	the reason why the document isn't a PDF document according to iText.
         */
        public UnsupportedPdfException(String message) : base(message) {
        }
    }
}
