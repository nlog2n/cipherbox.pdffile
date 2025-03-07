using System;
using System.Collections;
using System.Collections.Generic;

using iTextSharp.text.pdf;

namespace CipherBox.Pdf.Parser {

    /**
     * Provides information and calculations needed by render listeners
     * to display/evaluate text render operations.
     * <br><br>
     * This is passed between the {@link PdfContentStreamProcessor} and 
     * {@link RenderListener} objects as text rendering operations are
     * discovered
     */
    public class TextRenderInfo {
        
        private String text;
        private Matrix textToUserSpaceTransformMatrix;
        private GraphicsState gs;
        /**
         * Array containing marked content info for the text.
         * @since 5.0.2
         */
        private ICollection<MarkedContentInfo> markedContentInfos;
        
        /**
         * Creates a new TextRenderInfo object
         * @param text the text that should be displayed
         * @param gs the graphics state (note: at this time, this is not immutable, so don't cache it)
         * @param textMatrix the text matrix at the time of the render operation
         * @param markedContentInfo the marked content sequence, if available
         */
        internal TextRenderInfo(String text, GraphicsState gs, Matrix textMatrix, ICollection markedContentInfo) {
            this.text = text;
            this.textToUserSpaceTransformMatrix = textMatrix.Multiply(gs.ctm);
            this.gs = gs;
            this.markedContentInfos = new List<MarkedContentInfo>();
            foreach (MarkedContentInfo m in markedContentInfo) {
                this.markedContentInfos.Add(m);
            }
        }

        /**
         * Used for creating sub-TextRenderInfos for each individual character
         * @param parent the parent TextRenderInfo
         * @param charIndex the index of the character that this TextRenderInfo will represent
         * @param horizontalOffset the unscaled horizontal offset of the character that this TextRenderInfo represents
         * @since 5.3.3
         */
        private TextRenderInfo(TextRenderInfo parent, int charIndex, float horizontalOffset)
        {
            this.text = parent.text.Substring(charIndex, 1);
            this.textToUserSpaceTransformMatrix = new Matrix(horizontalOffset, 0).Multiply(parent.textToUserSpaceTransformMatrix);
            this.gs = parent.gs;
            this.markedContentInfos = parent.markedContentInfos;
        }
        
        /**
         * @return the text to render
         */
        public String GetText(){ 
            return text; 
        }

        /**
         * Checks if the text belongs to a marked content sequence
         * with a given mcid.
         * @param mcid a marked content id
         * @return true if the text is marked with this id
         * @since 5.0.2
         */
        public bool HasMcid(int mcid) {
            return HasMcid(mcid, false);
	    }

        /**
	     * Checks if the text belongs to a marked content sequence
	     * with a given mcid.
         * @param mcid a marked content id
         * @param checkTheTopmostLevelOnly indicates whether to check the topmost level of marked content stack only
         * @return true if the text is marked with this id
         * @since 5.3.5
         */
        public bool HasMcid(int mcid, bool checkTheTopmostLevelOnly) {
            if (checkTheTopmostLevelOnly) {
                if (markedContentInfos is IList) {
                    IList<MarkedContentInfo> mci = (IList<MarkedContentInfo>)markedContentInfos;
                    // Java and C# Stack classes have different numeration direction, so top element of the stack is 
                    // at last postion in Java and at first position in C#
                    return mci.Count > 0 && mci[0].GetMcid() == mcid;
                }
            } else {
                foreach (MarkedContentInfo info in markedContentInfos) {
                    if (info.HasMcid())
                        if (info.GetMcid() == mcid)
                            return true;
                }
            }
            return false;
        }

        /**
         * @return the unscaled (i.e. in Text space) width of the text
         */
        internal float GetUnscaledWidth(){ 
            return GetStringWidth(text); 
        }
        
        /**
         * Gets the baseline for the text (i.e. the line that the text 'sits' on)
         * This value includes the Rise of the draw operation - see {@link #getRise()} for the amount added by Rise
         * @return the baseline line segment
         * @since 5.0.2
         */
        public LineSegment GetBaseline(){
            return GetUnscaledBaselineWithOffset(0 + gs.rise).TransformBy(textToUserSpaceTransformMatrix);
        }

        /**
         * Gets the ascentline for the text (i.e. the line that represents the topmost extent that a string of the current font could have)
         * This value includes the Rise of the draw operation - see {@link #getRise()} for the amount added by Rise
         * @return the ascentline line segment
         * @since 5.0.2
         */
        public LineSegment GetAscentLine(){
            float ascent = gs.GetFont().GetFontDescriptor(BaseFont.ASCENT, gs.GetFontSize());
            return GetUnscaledBaselineWithOffset(ascent + gs.rise).TransformBy(textToUserSpaceTransformMatrix);
        }

        /**
         * Gets the descentline for the text (i.e. the line that represents the bottom most extent that a string of the current font could have)
         * This value includes the Rise of the draw operation - see {@link #getRise()} for the amount added by Rise
         * @return the descentline line segment
         * @since 5.0.2
         */
        public LineSegment GetDescentLine(){
            // per GetFontDescription() API, descent is returned as a negative number, so we apply that as a normal vertical offset
            float descent = gs.GetFont().GetFontDescriptor(BaseFont.DESCENT, gs.GetFontSize());
            return GetUnscaledBaselineWithOffset(descent + gs.rise).TransformBy(textToUserSpaceTransformMatrix);
        }
        
        private LineSegment GetUnscaledBaselineWithOffset(float yOffset){
            // we need to correct the width so we don't have an extra character spacing value at the end.  The extra character space is important for tracking relative text coordinate systems, but should not be part of the baseline
            float correctedUnscaledWidth = GetUnscaledWidth() - gs.characterSpacing * gs.horizontalScaling;

            return new LineSegment(new Vector(0, yOffset, 1), new Vector(correctedUnscaledWidth, yOffset, 1));
        }

        /**
         * Getter for the font
         * @return the font
         * @since iText 5.0.2
         */
        public DocumentFont GetFont() {
            return gs.GetFont();
        }


        // removing - this shouldn't be needed now that we are exposing getCharacterRenderInfos()
        //	/**
        //	 * @return The character spacing width, in user space units (Tc value, scaled to user space)
        //	 * @since 5.3.3
        //	 */
        //	public float getCharacterSpacing(){
        //		return convertWidthFromTextSpaceToUserSpace(gs.characterSpacing);
        //	}
        //	
        //	/**
        //	 * @return The word spacing width, in user space units (Tw value, scaled to user space)
        //	 * @since 5.3.3
        //	 */
        //	public float getWordSpacing(){
        //		return convertWidthFromTextSpaceToUserSpace(gs.wordSpacing);
        //	}

        /**
         * The rise represents how far above the nominal baseline the text should be rendered.  The {@link #getBaseline()}, {@link #getAscentLine()} and {@link #getDescentLine()} methods already include Rise.
         * This method is exposed to allow listeners to determine if an explicit rise was involved in the computation of the baseline (this might be useful, for example, for identifying superscript rendering)
         * @return The Rise for the text draw operation, in user space units (Ts value, scaled to user space)
         * @since 5.3.3
         */
        public float GetRise()
        {
            if (gs.rise == 0) return 0; // optimize the common case

            return ConvertHeightFromTextSpaceToUserSpace(gs.rise);
        }

        /**
         * 
         * @param width the width, in text space
         * @return the width in user space
         * @since 5.3.3
         */
        private float ConvertWidthFromTextSpaceToUserSpace(float width)
        {
            LineSegment textSpace = new LineSegment(new Vector(0, 0, 1), new Vector(width, 0, 1));
            LineSegment userSpace = textSpace.TransformBy(textToUserSpaceTransformMatrix);
            return userSpace.GetLength();
        }

        /**
         * 
         * @param height the height, in text space
         * @return the height in user space
         * @since 5.3.3
         */
        private float ConvertHeightFromTextSpaceToUserSpace(float height)
        {
            LineSegment textSpace = new LineSegment(new Vector(0, 0, 1), new Vector(0, height, 1));
            LineSegment userSpace = textSpace.TransformBy(textToUserSpaceTransformMatrix);
            return userSpace.GetLength();
        }

	

        /**
         * @return The width, in user space units, of a single space character in the current font
         */
        public float GetSingleSpaceWidth(){
            return ConvertWidthFromTextSpaceToUserSpace(GetUnscaledFontSpaceWidth());
        }
        
        /**
         * @return the text render mode that should be used for the text.  From the
         * PDF specification, this means:
         * <ul>
         *   <li>0 = Fill text</li>
         *   <li>1 = Stroke text</li>
         *   <li>2 = Fill, then stroke text</li>
         *   <li>3 = Invisible</li>
         *   <li>4 = Fill text and add to path for clipping</li>
         *   <li>5 = Stroke text and add to path for clipping</li>
         *   <li>6 = Fill, then stroke text and add to path for clipping</li>
         *   <li>7 = Add text to padd for clipping</li>
         * </ul>
         * @since iText 5.0.1
         */
        public int GetTextRenderMode(){
            return gs.renderMode;
        }
        
        /**
         * Calculates the width of a space character.  If the font does not define
         * a width for a standard space character \u0020, we also attempt to use
         * the width of \u00A0 (a non-breaking space in many fonts)
         * @return the width of a single space character in text space units
         */
        private float GetUnscaledFontSpaceWidth(){
            char charToUse = ' ';
            if (gs.font.GetWidth(charToUse) == 0)
                charToUse = '\u00A0';
            return GetStringWidth(charToUse.ToString());
        }
        
        /**
         * Gets the width of a String in text space units
         * @param string    the string that needs measuring
         * @return  the width of a String in text space units
         */
        private float GetStringWidth(String str){
            DocumentFont font = gs.font;
            char[] chars = str.ToCharArray();
            float totalWidth = 0;
            for (int i = 0; i < chars.Length; i++) {
                float w = font.GetWidth(chars[i]) / 1000.0f;
                float wordSpacing = chars[i] == 32 ? gs.wordSpacing : 0f;
                totalWidth += (w * gs.fontSize + gs.characterSpacing + wordSpacing) * gs.horizontalScaling;
            }
            
            return totalWidth;
        }

        /**
         * Provides detail useful if a listener needs access to the position of each individual glyph in the text render operation
         * @return A list of {@link TextRenderInfo} objects that represent each glyph used in the draw operation. The next effect is if there was a separate Tj opertion for each character in the rendered string
         * @since 5.3.3
         */
        public List<TextRenderInfo> GetCharacterRenderInfos()
        {
            List<TextRenderInfo> rslt = new List<TextRenderInfo>(text.Length);

            DocumentFont font = gs.font;
            char[] chars = text.ToCharArray();
            float totalWidth = 0;
            for (int i = 0; i < chars.Length; i++)
            {
                float w = font.GetWidth(chars[i]) / 1000.0f;
                float wordSpacing = chars[i] == 32 ? gs.wordSpacing : 0f;

                TextRenderInfo subInfo = new TextRenderInfo(this, i, totalWidth);
                rslt.Add(subInfo);

                totalWidth += (w * gs.fontSize + gs.characterSpacing + wordSpacing) * gs.horizontalScaling;

            }

            return rslt;
        }
    }
}