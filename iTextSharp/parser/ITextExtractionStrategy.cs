using System;

namespace CipherBox.Pdf.Parser {

    /**
     * Defines an interface for {@link RenderListener}s that can return text
     * @since 5.0.2
     */
    public interface ITextExtractionStrategy : IRenderListener {
        /**
         * Returns the result so far.
         * @return  a String with the resulting text.
         */
        String GetResultantText();
    }
}