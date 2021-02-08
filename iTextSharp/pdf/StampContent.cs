using System;

namespace iTextSharp.text.pdf {
    public class StampContent : PdfContentByte {
        internal PdfStamperImp.PageStamp ps;
        internal PageResources pageResources;
        
        /** Creates a new instance of StampContent */
        internal StampContent(PdfStamperImp stamper, PdfStamperImp.PageStamp ps) : base(stamper) {
            this.ps = ps;
            pageResources = ps.pageResources;
        }
        
        public override void SetAction(PdfAction action, float llx, float lly, float urx, float ury) {
            ((PdfStamperImp)writer).AddAnnotation(new PdfAnnotation(writer, llx, lly, urx, ury, action), ps.pageN);
        }

        /**
        * Gets a duplicate of this <CODE>PdfContentByte</CODE>. All
        * the members are copied by reference but the buffer stays different.
        *
        * @return a copy of this <CODE>PdfContentByte</CODE>
        */
        public override PdfContentByte Duplicate {
            get {
                return new StampContent((PdfStamperImp)writer, ps);
            }
        }

        internal override PageResources PageResources {
            get {
                return pageResources;
            }
        }
        
        internal override void AddAnnotation(PdfAnnotation annot) {
            ((PdfStamperImp)writer).AddAnnotation(annot, ps.pageN);
        }
    }
}
