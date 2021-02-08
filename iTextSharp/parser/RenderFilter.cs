using System;

namespace CipherBox.Pdf.Parser {


    /**
     * Interface for defining filters for use with {@link FilteredRenderListener}
     * @since 5.0.1
     */
    public abstract class RenderFilter {

        /**
         * @param renderInfo
         * @return true if the text render operation should be performed
         */
        public virtual bool AllowText(TextRenderInfo renderInfo){
            return true;
        }
        
        /**
         * 
         * @param renderInfo
         * @return true is the image render operation should be performed
         */
        public virtual bool AllowImage(ImageRenderInfo renderInfo){
            return true;
        }
    }
}