using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using iTextSharp.text.pdf.fonts.cmaps;
using iTextSharp.text.error_messages;

namespace iTextSharp.text.pdf 
{
    /**
     * Implementation of DocumentFont used while parsing PDF streams.
     * @since 2.1.4
     */
    public class CMapAwareDocumentFont : DocumentFont {

        /** The font dictionary. */
        private PdfDictionary fontDic;
        /** the width of a space for this font, in normalized 1000 point units */
        private int spaceWidth;
        /** The CMap constructed from the ToUnicode map from the font's dictionary, if present.
         *  This CMap transforms CID values into unicode equivalent
         */
        private CMapToUnicode toUnicodeCmap;
        private CMapByteCid byteCid;
        private CMapCidUni cidUni;
        /**
         *  Mapping between CID code (single byte only for now) and unicode equivalent
         *  as derived by the font's encoding.  Only needed if the ToUnicode CMap is not provided.
         */
        private char[] cidbyte2uni;
        
        private IDictionary<int,int> uni2cid;

        public CMapAwareDocumentFont(PdfDictionary font) : base(font) {
            fontDic = font;
            InitFont();
        }
        
        /**
         * Creates an instance of a CMapAwareFont based on an indirect reference to a font.
         * @param refFont   the indirect reference to a font
         */
        public CMapAwareDocumentFont(PRIndirectReference refFont) : base(refFont) {
            fontDic = (PdfDictionary)PdfReader.GetPdfObjectRelease(refFont);
            InitFont();
        }

        private void InitFont() {
            ProcessToUnicode();
            //if (toUnicodeCmap == null)
                ProcessUni2Byte();
            
            spaceWidth = base.GetWidth(' ');
            if (spaceWidth == 0){
                spaceWidth = ComputeAverageWidth();
            }
            if (cjkEncoding != null) {
                byteCid = CMapCache.GetCachedCMapByteCid(cjkEncoding);
                cidUni = CMapCache.GetCachedCMapCidUni(uniMap);
            }

        }

        /**
         * Parses the ToUnicode entry, if present, and constructs a CMap for it
         * @since 2.1.7
         */
        private void ProcessToUnicode(){
            PdfObject toUni = PdfReader.GetPdfObjectRelease(fontDic.Get(PdfName.TOUNICODE));
            if (toUni is PRStream) {
                try {
                    byte[] touni = PdfReader.GetStreamBytes((PRStream)toUni);
                    CidLocationFromByte lb = new CidLocationFromByte(touni);
                    toUnicodeCmap = new CMapToUnicode();
                    CMapParserEx.ParseCid("", toUnicodeCmap, lb);
                    uni2cid = toUnicodeCmap.CreateReverseMapping();
                } catch {
                    toUnicodeCmap = null;
                    uni2cid = null;
                    // technically, we should log this or provide some sort of feedback... but sometimes the cmap will be junk, but it's still possible to get text, so we don't want to throw an exception
                    //throw new IllegalStateException("Unable to process ToUnicode map - " + e.GetMessage(), e);
                }
            }
            else if(isType0) {
                // fake a ToUnicode for CJK Identity-H fonts
                try {
                    PdfName encodingName = fontDic.GetAsName(PdfName.ENCODING);
                    if(encodingName == null)
                        return;
                    String enc = PdfName.DecodeName(encodingName.ToString());
                    if(!enc.Equals("Identity-H"))
                        return;
                    PdfArray df = (PdfArray)PdfReader.GetPdfObjectRelease(fontDic.Get(PdfName.DESCENDANTFONTS));
                    PdfDictionary cidft = (PdfDictionary)PdfReader.GetPdfObjectRelease(df[0]);
                    PdfDictionary cidinfo = cidft.GetAsDict(PdfName.CIDSYSTEMINFO);
                    if(cidinfo == null)
                        return;
                    PdfString ordering = cidinfo.GetAsString(PdfName.ORDERING);
                    if(ordering == null)
                        return;
                    CMapToUnicode touni = IdentityToUnicode.GetMapFromOrdering(ordering.ToUnicodeString());
                    if(touni == null)
                        return;
                    toUnicodeCmap = touni;
                    uni2cid = toUnicodeCmap.CreateReverseMapping();
                } catch(IOException ex) {
                    toUnicodeCmap = null;
                    uni2cid = null;
                    Console.WriteLine(ex.Message); // fanghui
                }
            }
        }
        
        /**
         * Inverts DocumentFont's uni2byte mapping to obtain a cid-to-unicode mapping based
         * on the font's encoding
         * @since 2.1.7
         */
        private void ProcessUni2Byte(){
    	    IntHashtable byte2uni = Byte2Uni;
    	    int[] e = byte2uni.ToOrderedKeys();
            if (e.Length == 0)
                return;
            cidbyte2uni = new char[256];
            for (int k = 0; k < e.Length; ++k) {
                int key = e[k];
                cidbyte2uni[key] = (char)byte2uni[key];
            }
            if (toUnicodeCmap != null) {
                IDictionary<int,int> dm = toUnicodeCmap.CreateDirectMapping();
                foreach (KeyValuePair<int,int> kv in dm) {
                    if (kv.Key < 256)
                        cidbyte2uni[kv.Key] = (char)kv.Value;
                }
            }
            IntHashtable diffmap = Diffmap;
            if (diffmap != null) {
                // the difference array overrides the existing encoding
                e = diffmap.ToOrderedKeys();
                for (int k = 0; k < e.Length; ++k) {
                    int n = diffmap[e[k]];
                    if (n < 256)
                        cidbyte2uni[n] = (char)e[k];
                }
            }
        }
        

        
        /**
         * For all widths of all glyphs, compute the average width in normalized 1000 point units.
         * This is used to give some meaningful width in cases where we need an average font width 
         * (such as if the width of a space isn't specified by a given font)
         * @return the average width of all non-zero width glyphs in the font
         */
        private int ComputeAverageWidth(){
            int count = 0;
            int total = 0;
            for (int i = 0; i < base.widths.Length; i++){
                if (base.widths[i] != 0){
                    total += base.widths[i];
                    count++;
                }
            }
            return count != 0 ? total/count : 0;
        }
        
        /**
         * @since 2.1.5
         * Override to allow special handling for fonts that don't specify width of space character
         * @see com.itextpdf.text.pdf.DocumentFont#getWidth(int)
         */
        public override int GetWidth(int char1) {
            if (char1 == ' ')
                return spaceWidth;
            return base.GetWidth(char1);
        }
        
        /**
         * Decodes a single CID (represented by one or two bytes) to a unicode String.
         * @param bytes     the bytes making up the character code to convert
         * @param offset    an offset
         * @param len       a length
         * @return  a String containing the encoded form of the input bytes using the font's encoding.
         */
        private String DecodeSingleCID(byte[] bytes, int offset, int len){
            if (toUnicodeCmap != null){
                if (offset + len > bytes.Length)
                    throw new  IndexOutOfRangeException(MessageLocalization.GetComposedMessage("invalid.index.1", offset + len));
                string s = toUnicodeCmap.Lookup(bytes, offset, len);
                if (s != null)
                    return s;
                if (len != 1 || cidbyte2uni == null)
                    return null;
            }

            if (len == 1){
                if (cidbyte2uni == null)
                    return "";
                else
                    return new String(cidbyte2uni, 0xff & bytes[offset], 1);
            }
            
            throw new ArgumentException("Multi-byte glyphs not implemented yet");
        }

        /**
         * Decodes a string of bytes (encoded in the font's encoding) into a unicode string
         * This will use the ToUnicode map of the font, if available, otherwise it uses
         * the font's encoding
         * @param cidbytes    the bytes that need to be decoded
         * @return  the unicode String that results from decoding
         * @since 2.1.7
         */
        public String Decode(byte[] cidbytes, int offset, int len){
            StringBuilder sb = new StringBuilder();
            if (toUnicodeCmap == null && byteCid != null) {
                CMapSequence seq = new CMapSequence(cidbytes, offset, len);
                String cid = byteCid.DecodeSequence(seq);
                foreach (char ca in cid) {
                    int c = cidUni.Lookup(ca);
                    if (c > 0)
                        sb.Append(Utilities.ConvertFromUtf32(c));
                }
            }
            else {
                for (int i = offset; i < offset + len; i++){
                    String rslt = DecodeSingleCID(cidbytes, i, 1);
                    if (rslt == null && i < offset + len - 1){
                        rslt = DecodeSingleCID(cidbytes, i, 2);
                        i++;
                    }
                    if (rslt != null)
                        sb.Append(rslt);
                }
            }
            return sb.ToString();
        }

        /**
         * Encodes bytes to a String.
         * @param bytes     the bytes from a stream
         * @param offset    an offset
         * @param len       a length
         * @return  a String encoded taking into account if the bytes are in unicode or not.
         * @deprecated method name is not indicative of what it does.  Use <code>decode</code> instead.
         */
        public String Encode(byte[] bytes, int offset, int len){
            return Decode(bytes, offset, len);    
        }
    }
}