using System;
using System.IO;

namespace iTextSharp.text.pdf 
{
    public class PdfLiteral : PdfObject 
    {
        private long position;

        public PdfLiteral(string text) : base(0, text) 
        {}
        
        public PdfLiteral(byte[] b) : base(0, b) 
        {}
        
        public PdfLiteral(int type, string text) : base(type, text) 
        {}
        
        public PdfLiteral(int type, byte[] b) : base(type, b) 
        {}

        public PdfLiteral(int size) : base(0, (byte[])null) 
        {
            bytes = new byte[size];
            for (int k = 0; k < size; ++k) 
            {
               bytes[k] = 32;
            }
        }

        public override void ToPdf(PdfWriter writer, Stream os) 
        {
            if (os is OutputStreamCounter)
            {
                position = ((OutputStreamCounter)os).Counter;
            }
            base.ToPdf(writer, os);
        }

        public long Position 
        {
            get {
                return position;
            }
        }

        public int PosLength 
        {
            get {
                if (bytes != null)
                    return bytes.Length;
                else
                    return 0;
            }
        }
    }
}
