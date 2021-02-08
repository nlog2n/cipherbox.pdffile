using System;
using System.IO;

namespace iTextSharp.text.exceptions {

    /**
     * Typed exception used when opening an existing PDF document.
     * Gets thrown when the document isn't a valid PDF document.
     * @since 2.1.5
     */
    public class InvalidPdfException : IOException {

        private readonly Exception cause;

        /**
         * Creates an instance of  with a message and no cause
         * @param	message	the reason why the document isn't a PDF document according to iText.
         */
        public InvalidPdfException(String message) : base(message) {
        }

        /**
	     * Creates an exception with a message and a cause
	     * @param message	the reason why the document isn't a PDF document according to iText. 
	     * @param cause the cause of the exception, if any
	     */
        public InvalidPdfException(String message, Exception cause) : base(message, cause)
        {
            this.cause = cause;
        }
    }
}
