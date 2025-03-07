using System;

using iTextSharp.text.error_messages;
using iTextSharp.text.pdf;

namespace iTextSharp.text 
{
    /// <summary>
    /// PdfTemplate that has to be inserted into the document
    /// </summary>
    /// <seealso cref="T:iTextSharp.text.Element"/>
    /// <seealso cref="T:iTextSharp.text.Image"/>
    public class ImgTemplate : Image {
    
        /// <summary>
        /// Creats an Image from a PdfTemplate.
        /// </summary>
        /// <param name="image">the Image</param>
        public ImgTemplate(Image image) : base(image) {}
    
        /// <summary>
        /// Creats an Image from a PdfTemplate.
        /// </summary>
        /// <param name="template">the PdfTemplate</param>
        public ImgTemplate(PdfTemplate template) : base((Uri)null) 
        {
            if (template == null)
                throw new DocumentException("BadElement: the.template.can.not.be.null");
            if (template.Type == PdfTemplate.TYPE_PATTERN)
                throw new DocumentException("BadElement: a.pattern.can.not.be.used.as.a.template.to.create.an.image");

            this.type = Element.IMGTEMPLATE;
            this.scaledHeight = template.Height;
            this.Top = scaledHeight;
            this.scaledWidth = template.Width;
            this.Right = scaledWidth;
            this.TemplateData = template;
            this.plainWidth = this.Width;
            this.plainHeight = this.Height;
        }
    }
}
