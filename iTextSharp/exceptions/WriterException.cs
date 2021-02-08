using System;

namespace iTextSharp.text.pdf.qrcode {

    /**
     * A base class which covers the range of exceptions which may occur when encoding a barcode using
     * the Writer framework.
     *
     * @author dswitkin@google.com (Daniel Switkin)
     */
    public sealed class WriterException : Exception {

        public WriterException()
            : base() {
        }

        public WriterException(String message)
            : base(message) {
        }
    }
}