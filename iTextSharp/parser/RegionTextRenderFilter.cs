using System;
using System.Drawing;

using CipherBox.Pdf.Utility;

namespace CipherBox.Pdf.Parser 
{
    /**
     * A {@link RenderFilter} that only allows text within a specified rectangular region
     */
    public class RegionTextRenderFilter : RenderFilter 
    {
        /** the region to allow text from */
        private RectangleJ filterRect;
        
        /**
         * Constructs a filter
         * @param filterRect the rectangle to filter text against.  Note that this is a java.awt.Rectangle !
         */
        public RegionTextRenderFilter(RectangleJ filterRect) {
            this.filterRect = filterRect;
        }

        /**
         * Constructs a filter
         * @param filterRect the rectangle to filter text against.
         */
        public RegionTextRenderFilter(iTextSharp.text.Rectangle filterRect) 
        {
            filterRect.Normalize();
            this.filterRect = new RectangleJ(filterRect.Left, filterRect.Bottom, filterRect.Width, filterRect.Height);
        }
 
        /** 
         * @see com.itextpdf.text.pdf.parser.RenderFilter#allowText(com.itextpdf.text.pdf.parser.TextRenderInfo)
         */
        public override bool AllowText(TextRenderInfo renderInfo){
            LineSegment segment = renderInfo.GetBaseline();
            Vector startPoint = segment.GetStartPoint();
            Vector endPoint = segment.GetEndPoint();
            
            float x1 = startPoint[Vector.I1];
            float y1 = startPoint[Vector.I2];
            float x2 = endPoint[Vector.I1];
            float y2 = endPoint[Vector.I2];
            
            return filterRect.IntersectsLine(x1, y1, x2, y2);
        }
    }
}