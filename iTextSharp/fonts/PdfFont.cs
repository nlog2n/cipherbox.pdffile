using System;

using iTextSharp.text;

namespace iTextSharp.text.pdf 
{
    /**
    * <CODE>PdfFont</CODE> is the Pdf Font object.
    * <P>
    * Limitation: in this class only base 14 Type 1 fonts (courier, courier bold, courier oblique,
    * courier boldoblique, helvetica, helvetica bold, helvetica oblique, helvetica boldoblique,
    * symbol, times roman, times bold, times italic, times bolditalic, zapfdingbats) and their
    * standard encoding (standard, MacRoman, (MacExpert,) WinAnsi) are supported.<BR>
    * This object is described in the 'Portable Document Format Reference Manual version 1.3'
    * section 7.7 (page 198-203).
    *
    * @see        PdfName
    * @see        PdfDictionary
    * @see        BadPdfFormatException
    */

    public class PdfFont : IComparable<PdfFont> {
        
        
        /** the font metrics. */
        private BaseFont font;
        
        /** the size. */
        private float size;
        
        protected float hScale = 1;
        
        // constructors
        
        internal PdfFont(BaseFont bf, float size) {
            this.size = size;
            font = bf;
        }
        
        // methods
        
        /**
        * Compares this <CODE>PdfFont</CODE> with another
        *
        * @param    object    the other <CODE>PdfFont</CODE>
        * @return    a value
        */
        
        public int CompareTo(PdfFont pdfFont) {
            if (pdfFont == null) {
                return -1;
            }
            try {
                if (font != pdfFont.font) {
                    return 1;
                }
                if (this.Size != pdfFont.Size) {
                    return 2;
                }
                return 0;
            }
            catch (InvalidCastException) {
                return -2;
            }
        }
        
        /**
        * Returns the size of this font.
        *
        * @return        a size
        */
        
        internal float Size {
            get { return size; }
        }
        
        /**
        * Returns the approximative width of 1 character of this font.
        *
        * @return        a width in Text Space
        */
        
        internal float Width() {
            return Width(' ');
        }
        
        /**
        * Returns the width of a certain character of this font.
        *
        * @param        character    a certain character
        * @return        a width in Text Space
        */
        
        internal float Width(int character) {
                return font.GetWidthPoint(character, size) * hScale;
        }
        
        internal float Width(String s) {
            return font.GetWidthPoint(s, size) * hScale;
        }
        
        internal BaseFont Font {
            get { 
                return font;
            }
        }
        
        internal static PdfFont DefaultFont {
            get {
                BaseFont bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.WINANSI, false);
                return new PdfFont(bf, 12);
            }
        }

        internal float HorizontalScaling {
            set {
                this.hScale = value;
            }

            get {
                return hScale;
            }
        }
    }
}
