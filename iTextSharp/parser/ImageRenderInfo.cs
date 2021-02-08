using System;
using System.IO;

using iTextSharp.text.pdf;

namespace CipherBox.Pdf.Parser {

    /**
     * Represents image data from a PDF
     * @since 5.0.1
     */
    public class ImageRenderInfo {
        /** The coordinate transformation matrix that was in effect when the image was rendered */
        private Matrix ctm;
        /** A reference to the image XObject */
        private PdfIndirectReference refi;
        /** A reference to an inline image */
        private InlineImageInfo inlineImageInfo;
        /** the color space associated with the image */
        private PdfDictionary colorSpaceDictionary;
        /** the image object to be rendered, if it has been parsed already.  Null otherwise. */
        private PdfImageObject imageObject = null;
        
        private ImageRenderInfo(Matrix ctm, PdfIndirectReference refi, PdfDictionary colorSpaceDictionary) {
            this.ctm = ctm;
            this.refi = refi;
            this.inlineImageInfo = null;
            this.colorSpaceDictionary = colorSpaceDictionary;
        }

        private ImageRenderInfo(Matrix ctm, InlineImageInfo inlineImageInfo, PdfDictionary colorSpaceDictionary) {
            this.ctm = ctm;
            this.refi = null;
            this.inlineImageInfo = inlineImageInfo;
            this.colorSpaceDictionary = colorSpaceDictionary;
        }
        
        /**
         * Create an ImageRenderInfo object based on an XObject (this is the most common way of including an image in PDF)
         * @param ctm the coordinate transformation matrix at the time the image is rendered
         * @param ref a reference to the image XObject
         * @return the ImageRenderInfo representing the rendered XObject
         * @since 5.0.1
         */
        public static ImageRenderInfo CreateForXObject(Matrix ctm, PdfIndirectReference refi, PdfDictionary colorSpaceDictionary){
            return new ImageRenderInfo(ctm, refi, colorSpaceDictionary);
        }
        
        /**
         * Create an ImageRenderInfo object based on inline image data.  This is nowhere near completely thought through
         * and really just acts as a placeholder.
         * @param ctm the coordinate transformation matrix at the time the image is rendered
         * @param imageObject the image object representing the inline image
         * @return the ImageRenderInfo representing the rendered embedded image
         * @since 5.0.1
         */
        protected internal static ImageRenderInfo CreateForEmbeddedImage(Matrix ctm, InlineImageInfo inlineImageInfo, PdfDictionary colorSpaceDictionary) {
            ImageRenderInfo renderInfo = new ImageRenderInfo(ctm, inlineImageInfo, colorSpaceDictionary);
            return renderInfo;
        }
        
        /**
         * Gets an object containing the image dictionary and bytes.
         * @return an object containing the image dictionary and byte[]
         * @since 5.0.2
         */
        public PdfImageObject GetImage() {
            PrepareImageObject();
            return imageObject;
        }
        
        private void PrepareImageObject() {
            if (imageObject != null)
                return;
            
            if (refi != null){
                PRStream stream = (PRStream)PdfReader.GetPdfObject(refi);
                imageObject = new PdfImageObject(stream, colorSpaceDictionary);
            } else if (inlineImageInfo != null){
                imageObject = new PdfImageObject(inlineImageInfo.ImageDictionary, inlineImageInfo.Samples, colorSpaceDictionary);
            }
        }
        
        /**
         * @return a vector in User space representing the start point of the xobject
         */
        public Vector GetStartPoint(){ 
            return new Vector(0, 0, 1).Cross(ctm); 
        }

        /**
         * @return The coordinate transformation matrix active when this image was rendered.  Coordinates are in User space.
         * @since 5.0.3
         */
        public Matrix GetImageCTM(){
            return ctm;
        }
        
        /**
         * @return the size of the image, in User space units
         */
        public float GetArea(){
            // the image space area is 1, so we multiply that by the determinant of the CTM to get the transformed area
            return ctm.GetDeterminant();
        }
        
        /**
         * @return an indirect reference to the image
         * @since 5.0.2
         */
        public PdfIndirectReference GetRef() {
            return refi;
        }
    }
}