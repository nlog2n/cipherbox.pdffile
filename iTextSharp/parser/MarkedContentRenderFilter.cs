using System;

namespace CipherBox.Pdf.Parser {

    /**
     * A {@link RenderFilter} that only allows text within a specified marked content sequence.
     * @since 5.0.2
     */
    public class MarkedContentRenderFilter : RenderFilter {
        
        /** The MCID to match. */
        private int mcid;
        
        /**
         * Constructs a filter
         * @param mcid the MCID to match
         */
        public MarkedContentRenderFilter(int mcid) {
            this.mcid = mcid;
        }

        /** 
         * @see com.itextpdf.text.pdf.parser.RenderFilter#allowText(com.itextpdf.text.pdf.parser.TextRenderInfo)
         */
        public override bool AllowText(TextRenderInfo renderInfo){
            return renderInfo.HasMcid(mcid);
        }


    }
}