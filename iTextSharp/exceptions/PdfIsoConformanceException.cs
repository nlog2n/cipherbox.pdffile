using System;

namespace iTextSharp.text.pdf{

    public class PdfIsoConformanceException : Exception
    {
        /** Serial version UID */
	    private const long serialVersionUID = -8972376258066225871L;


        /** Creates a new instance of PdfIsoConformanceException. */
        public PdfIsoConformanceException()
        {
        }

        /**
         * Creates a new instance of PdfIsoConformanceException.
         * @param s
         */
        public PdfIsoConformanceException(String s) : base(s)
        {            
        }
    }
}
