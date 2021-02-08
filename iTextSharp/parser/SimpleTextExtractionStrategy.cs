using System;
using System.Text;

namespace CipherBox.Pdf.Parser {

    /**
     * A simple text extraction renderer.
     * 
     * This renderer keeps track of the current Y position of each string.  If it detects
     * that the y position has changed, it inserts a line break into the output.  If the
     * PDF renders text in a non-top-to-bottom fashion, this will result in the text not
     * being a true representation of how it appears in the PDF.
     * 
     * This renderer also uses a simple strategy based on the font metrics to determine if
     * a blank space should be inserted into the output.
     * 
     * @since   2.1.5
     */
    public class SimpleTextExtractionStrategy : ITextExtractionStrategy {

        private Vector lastStart;
        private Vector lastEnd;
        
        /** used to store the resulting String. */
        private StringBuilder result = new StringBuilder();

        /**
         * Creates a new text extraction renderer.
         */
        public SimpleTextExtractionStrategy() {
        }

        /**
         * @since 5.0.1
         */
        public virtual void BeginTextBlock() {
        }

        /**
         * @since 5.0.1
         */
        public virtual void EndTextBlock() {
        }
        
        /**
         * Returns the result so far.
         * @return  a String with the resulting text.
         */
        public virtual String GetResultantText(){
            return result.ToString();
        }

        /**
         * Used to actually append text to the text results.  Subclasses can use this to insert
         * text that wouldn't normally be included in text parsing (e.g. result of OCR performed against
         * image content)
         * @param text the text to append to the text results accumulated so far
         */
        protected void AppendTextChunk(string text){
    	    result.Append(text);
        }

        protected void AppendTextChunk(char text) {
            result.Append(text);
        }

        /**
         * Captures text using a simplified algorithm for inserting hard returns and spaces
         * @param   renderInfo  render info
         */
        public virtual void RenderText(TextRenderInfo renderInfo) {
            bool firstRender = result.Length == 0;
            bool hardReturn = false;

            LineSegment segment = renderInfo.GetBaseline();
            Vector start = segment.GetStartPoint();
            Vector end = segment.GetEndPoint();
            
            if (!firstRender){
                Vector x0 = start;
                Vector x1 = lastStart;
                Vector x2 = lastEnd;
                
                // see http://mathworld.wolfram.com/Point-LineDistance2-Dimensional.html
                float dist = (x2.Subtract(x1)).Cross((x1.Subtract(x0))).LengthSquared / x2.Subtract(x1).LengthSquared;

                float sameLineThreshold = 1f; // we should probably base this on the current font metrics, but 1 pt seems to be sufficient for the time being
                if (dist > sameLineThreshold)
                    hardReturn = true;
                
                // Note:  Technically, we should check both the start and end positions, in case the angle of the text changed without any displacement
                // but this sort of thing probably doesn't happen much in reality, so we'll leave it alone for now
            }
            
            if (hardReturn){
                //System.out.Println("<< Hard Return >>");
                AppendTextChunk('\n');
            } else if (!firstRender){ 
                if (result[result.Length-1] != ' ' && renderInfo.GetText().Length > 0 && renderInfo.GetText()[0] != ' '){ // we only insert a blank space if the trailing character of the previous string wasn't a space, and the leading character of the current string isn't a space
                    float spacing = lastEnd.Subtract(start).Length;
                    if (spacing > renderInfo.GetSingleSpaceWidth()/2f){
                        AppendTextChunk(' ');
                        //System.out.Println("Inserting implied space before '" + renderInfo.GetText() + "'");
                    }
                }
            } else {
                //System.out.Println("Displaying first string of content '" + text + "' :: x1 = " + x1);
            }
            
            //System.out.Println("[" + renderInfo.GetStartPoint() + "]->[" + renderInfo.GetEndPoint() + "] " + renderInfo.GetText());
            AppendTextChunk(renderInfo.GetText());

            lastStart = start;
            lastEnd = end;            
        }

        /**
         * no-op method - this renderer isn't interested in image events
         * @see com.itextpdf.text.pdf.parser.RenderListener#renderImage(com.itextpdf.text.pdf.parser.ImageRenderInfo)
         * @since 5.0.1
         */
        public virtual void RenderImage(ImageRenderInfo renderInfo) {
            // do nothing - we aren't tracking images in this renderer
        }
    }
}