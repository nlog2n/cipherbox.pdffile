using System;

namespace CipherBox.Pdf.Parser 
{
    /**
     * A text render listener that filters text operations before passing them on to a deleg
     * @since 5.0.1
     */

    public class FilteredRenderListener : IRenderListener {

        /** The deleg that will receive the text render operation if the filters all pass */
        private IRenderListener deleg;
        /** The filters to be applied */
        private RenderFilter[] filters;

        /**
         * Construction
         * @param deleg the deleg {@link RenderListener} that will receive filtered text operations
         * @param filters the Filter(s) to apply
         */
        public FilteredRenderListener(IRenderListener deleg, params RenderFilter[] filters) {
            this.deleg = deleg;
            this.filters = filters;
        }

        /**
         * Applies filters, then delegates to the deleg if all filters pass
         * @param renderInfo contains info to render text
         * @see com.itextpdf.text.pdf.parser.RenderListener#renderText(com.itextpdf.text.pdf.parser.TextRenderInfo)
         */
        public void RenderText(TextRenderInfo renderInfo) {
            foreach (RenderFilter filter in filters) {
                if (!filter.AllowText(renderInfo))
                    return;
            }
            deleg.RenderText(renderInfo);
        }

        /**
         * This class delegates this call
         * @see com.itextpdf.text.pdf.parser.RenderListener#beginTextBlock()
         */
        public void BeginTextBlock() {
            deleg.BeginTextBlock();
        }

        /**
         * This class delegates this call
         * @see com.itextpdf.text.pdf.parser.RenderListener#endTextBlock()
         */
        public void EndTextBlock() {
            deleg.EndTextBlock();
        }

        /**
         * Applies filters, then delegates to the deleg if all filters pass
         * @see com.itextpdf.text.pdf.parser.RenderListener#renderImage(com.itextpdf.text.pdf.parser.ImageRenderInfo)
         * @since 5.0.1
         */
        public void RenderImage(ImageRenderInfo renderInfo) {
            foreach (RenderFilter filter in filters) {
                if (!filter.AllowImage(renderInfo))
                    return;
            }
            deleg.RenderImage(renderInfo);
        }
    }
}