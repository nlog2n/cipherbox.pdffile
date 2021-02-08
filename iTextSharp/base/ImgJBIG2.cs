using System;

using iTextSharp.text.pdf;
using CipherBox.Cryptography;

namespace iTextSharp.text 
{
    /**
    * Support for JBIG2 images.
    * @since 2.1.5
    */
    public class ImgJBIG2 : Image {
        
        /** JBIG2 globals */
        private byte[] global;
        /** A unique hash */
        private byte[] globalHash;
        
        /**
        * Copy contstructor.
        * @param    image another Image
        */
        ImgJBIG2(Image image) : base(image) {
        }

        /**
        * Empty constructor.
        */
        public ImgJBIG2() : base((Image) null) {
        }

        /**
        * Actual constructor for ImgJBIG2 images.
        * @param    width   the width of the image
        * @param    height  the height of the image
        * @param    data    the raw image data
        * @param    globals JBIG2 globals
        */
        public ImgJBIG2(int width, int height, byte[] data, byte[] globals) : base((Uri)null) {
            type = Element.JBIG2;
            originalType = ORIGINAL_JBIG2;
            scaledHeight = height;
            this.Top = scaledHeight;
            scaledWidth = width;
            this.Right = scaledWidth;
            bpc = 1;
            colorspace = 1;
            rawData = data;
            plainWidth = this.Width;
            plainHeight = this.Height;
            if ( globals != null ) {
                this.global = globals;
                try {
                    this.globalHash = DigestAlgorithms.Digest("MD5", this.global);
                } catch {
                    //ignore
                }
            }
        }
        
        /**
        * Getter for the JBIG2 global data.
        * @return   an array of bytes
        */
        public byte[] GlobalBytes {
            get {
                return this.global;
            }
        }
        
        /**
        * Getter for the unique hash.
        * @return   an array of bytes
        */
        public byte[] GlobalHash {
            get {
                return this.globalHash;
            }
        }
    }
}
