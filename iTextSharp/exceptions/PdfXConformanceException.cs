using System;

namespace iTextSharp.text.pdf {
    /**
    *
    * @author  psoares
    */
    public class PdfXConformanceException : PdfIsoConformanceException {
        
        /** Creates a new instance of PdfXConformanceException. */
        public PdfXConformanceException() {
        }
        
        /**
        * Creates a new instance of PdfXConformanceException.
        * @param s
        */
        public PdfXConformanceException(String s) : base(s) {
        }    
    }
}
