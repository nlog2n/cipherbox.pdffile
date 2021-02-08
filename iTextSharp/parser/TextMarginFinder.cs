using System;
using System.Drawing;

using CipherBox.Pdf.Utility;

namespace CipherBox.Pdf.Parser {

    /**
     * Allows you to find the rectangle that contains all the text in a page.
     * @since 5.0.2
     */
    public class TextMarginFinder : IRenderListener {
        private RectangleJ textRectangle = null;
        
        /**
         * Method invokes by the PdfContentStreamProcessor.
         * Passes a TextRenderInfo for every text chunk that is encountered.
         * We'll use this object to obtain coordinates.
         * @see com.itextpdf.text.pdf.parser.RenderListener#renderText(com.itextpdf.text.pdf.parser.TextRenderInfo)
         */
        public void RenderText(TextRenderInfo renderInfo) {
            if (textRectangle == null)
                textRectangle = renderInfo.GetDescentLine().GetBoundingRectange();
            else
                textRectangle.Add(renderInfo.GetDescentLine().GetBoundingRectange());
            
            textRectangle.Add(renderInfo.GetAscentLine().GetBoundingRectange());

        }

        /**
         * Getter for the left margin.
         * @return the X position of the left margin
         */
        public float GetLlx() {
            return textRectangle.X;
        }

        /**
         * Getter for the bottom margin.
         * @return the Y position of the bottom margin
         */
        public float GetLly() {
            return textRectangle.Y;
        }

        /**
         * Getter for the right margin.
         * @return the X position of the right margin
         */
        public float GetUrx() {
            return textRectangle.X + textRectangle.Width;
        }

        /**
         * Getter for the top margin.
         * @return the Y position of the top margin
         */
        public float GetUry() {
            return textRectangle.Y + textRectangle.Height;
        }

        /**
         * Gets the width of the text block.
         * @return a width
         */
        public float GetWidth() {
            return textRectangle.Width;
        }
        
        /**
         * Gets the height of the text block.
         * @return a height
         */
        public float GetHeight() {
            return textRectangle.Height;
        }
        
        /**
         * @see com.itextpdf.text.pdf.parser.RenderListener#beginTextBlock()
         */
        public void BeginTextBlock() {
            // do nothing
        }

        /**
         * @see com.itextpdf.text.pdf.parser.RenderListener#endTextBlock()
         */
        public void EndTextBlock() {
            // do nothing
        }

        /**
         * @see com.itextpdf.text.pdf.parser.RenderListener#renderImage(com.itextpdf.text.pdf.parser.ImageRenderInfo)
         */
        public void RenderImage(ImageRenderInfo renderInfo) {
            // do nothing
        }
    }
}