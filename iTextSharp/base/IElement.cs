using System.Collections.Generic;


namespace iTextSharp.text {
    /// <summary>
    /// Interface for a text element.
    /// </summary>
    /// <seealso cref="T:iTextSharp.text.Anchor"/>
    /// <seealso cref="T:iTextSharp.text.Cell"/>
    /// <seealso cref="T:iTextSharp.text.Chapter"/>
    /// <seealso cref="T:iTextSharp.text.Chunk"/>
    /// <seealso cref="T:iTextSharp.text.Gif"/>
    /// <seealso cref="T:iTextSharp.text.Graphic"/>
    /// <seealso cref="T:iTextSharp.text.Header"/>
    /// <seealso cref="T:iTextSharp.text.Image"/>
    /// <seealso cref="T:iTextSharp.text.Jpeg"/>
    /// <seealso cref="T:iTextSharp.text.List"/>
    /// <seealso cref="T:iTextSharp.text.ListItem"/>
    /// <seealso cref="T:iTextSharp.text.Meta"/>
    /// <seealso cref="T:iTextSharp.text.Paragraph"/>
    /// <seealso cref="T:iTextSharp.text.Phrase"/>
    /// <seealso cref="T:iTextSharp.text.Rectangle"/>
    /// <seealso cref="T:iTextSharp.text.Row"/>
    /// <seealso cref="T:iTextSharp.text.Section"/>
    /// <seealso cref="T:iTextSharp.text.Table"/>
    public interface IElement {

        // methods
    
        /// <summary>
        /// Processes the element by adding it (or the different parts) to an
        /// IElementListener.
        /// </summary>
        /// <param name="listener">an IElementListener</param>
        /// <returns>true if the element was processed successfully</returns>
        bool Process(IElementListener listener);
    
        /// <summary>
        /// Gets the type of the text element.
        /// </summary>
        /// <value>a type</value>
        int Type {
            get;
        }
    
        /**
        * Checks if this element is a content object.
        * If not, it's a metadata object.
        * @since    iText 2.0.8
        * @return   true if this is a 'content' element; false if this is a 'medadata' element
        */
        
        bool IsContent();
        
        /**
        * Checks if this element is nestable.
        * @since    iText 2.0.8
        * @return   true if this element can be nested inside other elements.
        */
        
        bool IsNestable();
        
        /// <summary>
        /// Gets all the chunks in this element.
        /// </summary>
        /// <value>an ArrayList</value>
        IList<Chunk> Chunks {
            get;
        }
    
        /// <summary>
        /// Gets the content of the text element.
        /// </summary>
        /// <returns>the content of the text element</returns>
        string ToString();
    }
}
