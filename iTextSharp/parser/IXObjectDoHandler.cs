using System;

using iTextSharp.text.pdf;

namespace CipherBox.Pdf.Parser 
{
    public interface IXObjectDoHandler 
    {
        void HandleXObject(PdfContentStreamProcessor processor, PdfStream stream, PdfIndirectReference refi);
    }
}