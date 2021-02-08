using System;

namespace CipherBox.Pdf.Parser {

    /**
     * A callback interface that receives notifications from the {@link PdfContentStreamProcessor}
     * as various render operations are required.
     * <br>
     * Important:  This interface may be converted to an abstract base class in the future
     * to allow for adding additional render calls as the content stream processor is enhanced
     * @since 5.0
     */
    public interface IRenderListener {

        /**
         * Called when a new text block is beginning (i.e. BT)
         * @since iText 5.0.1
         */
        void BeginTextBlock();

        /**
         * Called when text should be rendered
         * @param renderInfo information specifying what to render
         */
        void RenderText(TextRenderInfo renderInfo);

        
        /**
         * Called when a text block has ended (i.e. ET)
         * @since iText 5.0.1
         */
        void EndTextBlock();

        /**
         * Called when image should be rendered
         * @param renderInfo information specifying what to render
         * @since iText 5.0.1
         */
        void RenderImage(ImageRenderInfo renderInfo);
    }
}