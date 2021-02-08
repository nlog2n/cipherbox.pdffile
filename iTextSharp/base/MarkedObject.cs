using System;
using System.Collections.Generic;

using CipherBox.Pdf.Utility;


namespace iTextSharp.text {

    /**
    * Wrapper that allows to add properties to 'basic building block' objects.
    * Before iText 1.5 every 'basic building block' implemented the MarkupAttributes interface.
    * By setting attributes, you could add markup to the corresponding XML and/or HTML tag.
    * This functionality was hardly used by anyone, so it was removed, and replaced by
    * the MarkedObject functionality.
    */

    public class MarkedObject : IElement {

        /** The element that is wrapped in a MarkedObject. */
        protected internal IElement element;

        /** Contains extra markupAttributes */
        protected internal Properties markupAttributes = new Properties();
            
        /**
        * This constructor is for internal use only.
        */
        protected MarkedObject() {
            element = null;
        }
        
        /**
        * Creates a MarkedObject.
        */
        public MarkedObject(IElement element) {
            this.element = element;
        }
        
        /**
        * Gets all the chunks in this element.
        *
        * @return  an <CODE>ArrayList</CODE>
        */
        public virtual IList<Chunk> Chunks {
            get {
                return element.Chunks;
            }
        }

        /**
        * Processes the element by adding it (or the different parts) to an
        * <CODE>ElementListener</CODE>.
        *
        * @param       listener        an <CODE>ElementListener</CODE>
        * @return <CODE>true</CODE> if the element was processed successfully
        */
        public virtual bool Process(IElementListener listener) {
            try {
                return listener.Add(element);
            }
            catch (DocumentException) {
                return false;
            }
        }
        
        /**
        * Gets the type of the text element.
        *
        * @return  a type
        */
        public virtual int Type {
            get {
                return Element.MARKED;
            }
        }

        /**
        * @see com.lowagie.text.Element#isContent()
        * @since   iText 2.0.8
        */
        public bool IsContent() {
            return true;
        }

        /**
        * @see com.lowagie.text.Element#isNestable()
        * @since   iText 2.0.8
        */
        public bool IsNestable() {
            return true;
        }

        /**
        * @return the markupAttributes
        */
        public virtual Properties MarkupAttributes {
            get {
                return markupAttributes;
            }
        }
        
        public virtual void SetMarkupAttribute(String key, String value) {
            markupAttributes.Add(key, value);
        }

    }
}
