using System;

namespace iTextSharp.text.exceptions {

    /**
     * Typed exception used when creating PDF syntax that isn't valid.
     * @since 2.1.6
     */
    public class IllegalPdfSyntaxException : ArgumentException {

        /**
         * Creates an exception saying the PDF syntax isn't correct.
         * @param	message	some extra info about the exception
         */
        public IllegalPdfSyntaxException(String message) : base(message) {
        }
    }
}
