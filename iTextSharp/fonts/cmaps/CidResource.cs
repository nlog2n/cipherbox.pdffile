using System;
using System.IO;

using iTextSharp.text.error_messages;
using iTextSharp.text.pdf;
using iTextSharp.text.io;

namespace iTextSharp.text.pdf.fonts.cmaps 
{
    public class CidResource : ICidLocation
    {
        public virtual PRTokeniser GetLocation(String location) 
        {
            String fullName = BaseFont.RESOURCE_PATH + "cmaps." + location;
            Stream inp = StreamUtil.GetResourceStream(fullName);
            if (inp == null)
                throw new IOException(string.Format("the.cmap {0} was not found", fullName));
            return new PRTokeniser(new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateSource(inp)));
        }
    }
}