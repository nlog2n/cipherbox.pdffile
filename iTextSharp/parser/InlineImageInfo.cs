using System;

using iTextSharp.text.pdf;

namespace CipherBox.Pdf.Parser 
{
    // Represents an inline image from a PDF
    public class InlineImageInfo 
    {
        private byte[] samples;
        private PdfDictionary imageDictionary;
        
        public InlineImageInfo(byte[] samples, PdfDictionary imageDictionary) 
        {
            this.samples = samples;
            this.imageDictionary = imageDictionary;
        }
        
        // return the image dictionary associated with this inline image
        public PdfDictionary ImageDictionary { get { return imageDictionary; } }
        
        // return the raw samples associated with this inline image
        public byte[] Samples { get { return samples; } }
    }
}