using System;

using iTextSharp.text.pdf;
using iTextSharp.text.io;

namespace iTextSharp.text.pdf.fonts.cmaps 
{
    public class CidLocationFromByte : ICidLocation 
    {
        private byte[] data;

        public CidLocationFromByte(byte[] data) {
            this.data = data;
        }
        
        public virtual PRTokeniser GetLocation(String location) {
            return new PRTokeniser(new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateSource(data)));
        }
    }
}