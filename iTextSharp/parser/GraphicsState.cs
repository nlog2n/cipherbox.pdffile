using System;

using iTextSharp.text.pdf;

namespace CipherBox.Pdf.Parser {

    /**
     * Keeps all the parameters of the graphics state.
     * @since   2.1.4
     */
    public class GraphicsState {
        /** The current transformation matrix. */
        internal Matrix ctm;
        /** The current character spacing. */
        internal float characterSpacing;

        public float CharacterSpacing {
            get { return characterSpacing; }
        }

        /** The current word spacing. */
        internal float wordSpacing;

        public float WordSpacing { 
			get { return wordSpacing; } 
		}

        /** The current horizontal scaling */
        internal float horizontalScaling;

        public float HorizontalScaling {
            get { return horizontalScaling; }
        }

        /** The current leading. */
        internal float leading;
        /** The active font. */
        internal CMapAwareDocumentFont font;

        public CMapAwareDocumentFont Font {
            get { return font; }
        }

        /** The current font size. */
        internal float fontSize;

        public float FontSize {
            get { return fontSize; }
        }

        /** The current render mode. */
        internal int renderMode;
        /** The current text rise */
        internal float rise;
        /** The current knockout value. */
        internal bool knockout;
        
        /**
         * Constructs a new Graphics State object with the default values.
         */
        public GraphicsState(){
            ctm = new Matrix();
            characterSpacing = 0;
            wordSpacing = 0;
            horizontalScaling = 1.0f;
            leading = 0;
            font = null;
            fontSize = 0;
            renderMode = 0;
            rise = 0;
            knockout = true;
        }
        
        /**
         * Copy constructor.
         * @param source    another GraphicsState object
         */
        public GraphicsState(GraphicsState source){
            // note: all of the following are immutable, with the possible exception of font
            // so it is safe to copy them as-is
            ctm = source.ctm;
            characterSpacing = source.characterSpacing;
            wordSpacing = source.wordSpacing;
            horizontalScaling = source.horizontalScaling;
            leading = source.leading;
            font = source.font;
            fontSize = source.fontSize;
            renderMode = source.renderMode;
            rise = source.rise;
            knockout = source.knockout;
        }

        /**
         * Getter for the current transformation matrix
         * @return the ctm
         * @since iText 5.0.1
         */
        public Matrix GetCtm() {
            return ctm;
        }

        /**
         * Getter for the character spacing.
         * @return the character spacing
         * @since iText 5.0.1
         */
        public float GetCharacterSpacing() {
            return characterSpacing;
        }

        /**
         * Getter for the word spacing
         * @return the word spacing
         * @since iText 5.0.1
         */
        public float GetWordSpacing() {
            return wordSpacing;
        }

        /**
         * Getter for the horizontal scaling
         * @return the horizontal scaling
         * @since iText 5.0.1
         */
        public float GetHorizontalScaling() {
            return horizontalScaling;
        }

        /**
         * Getter for the leading
         * @return the leading
         * @since iText 5.0.1
         */
        public float GetLeading() {
            return leading;
        }

        /**
         * Getter for the font
         * @return the font
         * @since iText 5.0.1
         */
        public CMapAwareDocumentFont GetFont() {
            return font;
        }

        /**
         * Getter for the font size
         * @return the font size
         * @since iText 5.0.1
         */
        public float GetFontSize() {
            return fontSize;
        }

        /**
         * Getter for the render mode
         * @return the renderMode
         * @since iText 5.0.1
         */
        public int GetRenderMode() {
            return renderMode;
        }

        /**
         * Getter for text rise
         * @return the text rise
         * @since iText 5.0.1
         */
        public float GetRise() {
            return rise;
        }

        /**
         * Getter for knockout
         * @return the knockout
         * @since iText 5.0.1
         */
        public bool IsKnockout() {
            return knockout;
        }
    }
}