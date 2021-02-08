using System;
using System.Collections.Generic;

using iTextSharp.text.pdf;


namespace CipherBox.Pdf.Parser {

    /**
     * Interface implemented by a series of content operators
     * @since 2.1.4
     */
    public interface IContentOperator {
        /**
         * Invokes a content operator.
         * @param processor the processor that is dealing with the PDF content
         * @param operator  the literal PDF syntax of the operator
         * @param operands  the operands that come with the operator
         * @throws Exception any exception can be thrown - it will be re-packaged into a runtime exception and re-thrown by the {@link PdfContentStreamProcessor}
         */
        void Invoke(PdfContentStreamProcessor processor, PdfLiteral oper, List<PdfObject> operands);

    }
}