using System;

namespace CipherBox.Pdf.Parser {

    /**
     * A text render listener that filters text operations before passing them on to a deleg
     * @since 5.0.1
     */

    public class FilteredTextRenderListener : FilteredRenderListener, ITextExtractionStrategy {

        /** The deleg that will receive the text render operation if the filters all pass */
        private ITextExtractionStrategy deleg;

        /**
         * Construction
         * @param deleg the deleg {@link RenderListener} that will receive filtered text operations
         * @param filters the Filter(s) to apply
         */
        public FilteredTextRenderListener(ITextExtractionStrategy deleg, params RenderFilter[] filters) : base(deleg, filters) {
            this.deleg = deleg;
        }

        /**
         * This class delegates this call
         * @see com.itextpdf.text.pdf.parser.TextExtractionStrategy#getResultantText()
         */
        public virtual String GetResultantText() {
            return deleg.GetResultantText();
        }
    }
}