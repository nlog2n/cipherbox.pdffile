using System;

using iTextSharp.text.pdf;

namespace iTextSharp.text.pdf.fonts.cmaps 
{
    public interface ICidLocation {
        PRTokeniser GetLocation(String location);
    }
}