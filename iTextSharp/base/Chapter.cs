using System;
using System.Collections.Generic;

using CipherBox.Pdf.Utility;
using iTextSharp.text.factories;


namespace iTextSharp.text 
{
    /// <summary>
    /// A Chapter is a special Section.
    /// </summary>
    /// <remarks>
    /// A chapter number has to be created using a Paragraph as title
    /// and an int as chapter number. The chapter number is shown be
    /// default. If you don't want to see the chapter number, you have to set the
    /// numberdepth to 0.
    /// </remarks>
    /// <example>
    /// <code>
    /// Paragraph title2 = new Paragraph("This is Chapter 2", FontFactory.GetFont(FontFactory.HELVETICA, 18, Font.BOLDITALIC, new BaseColor(0, 0, 255)));
    /// <strong>Chapter chapter2 = new Chapter(title2, 2);
    /// chapter2.SetNumberDepth(0);</strong>
    /// Paragraph someText = new Paragraph("This is some text");
    /// <strong>chapter2.Add(someText);</strong>
    /// Paragraph title21 = new Paragraph("This is Section 1 in Chapter 2", FontFactory.GetFont(FontFactory.HELVETICA, 16, Font.BOLD, new BaseColor(255, 0, 0)));
    /// Section section1 = <strong>chapter2.AddSection(title21);</strong>
    /// Paragraph someSectionText = new Paragraph("This is some silly paragraph in a chapter and/or section. It contains some text to test the functionality of Chapters and Section.");
    /// section1.Add(someSectionText);
    /// </code>
    /// </example>
    public class Chapter : Section 
    {
    
        // constructors
    
        /**
        * Constructs a new <CODE>Chapter</CODE>.
        * @param   number      the Chapter number
        */
        
        public Chapter(int number) : base (null, 1) {
            numbers = new List<int>();
            numbers.Add(number);
            triggerNewPage = true;
        }

        /// <summary>
        /// Constructs a new Chapter.
        /// </summary>
        /// <param name="title">the Chapter title (as a Paragraph)</param>
        /// <param name="number">the Chapter number</param>
        /// <overoads>
        /// Has three overloads.
        /// </overoads>
        public Chapter(Paragraph title, int number) : base(title, 1) 
        {
            numbers = new List<int>();
            numbers.Add(number);
            triggerNewPage = true;
        }
    
        /// <summary>
        /// Constructs a new Chapter.
        /// </summary>
        /// <param name="title">the Chapter title (as a string)</param>
        /// <param name="number">the Chapter number</param>
        /// <overoads>
        /// Has three overloads.
        /// </overoads>
        public Chapter(string title, int number) : this(new Paragraph(title), number) {}
    
        // implementation of the Element-methods
    
        /// <summary>
        /// Gets the type of the text element.
        /// </summary>
        /// <value>a type</value>
        public override int Type {
            get {
                return Element.CHAPTER;
            }
        }
    
        /**
        * @see com.lowagie.text.Element#isNestable()
        * @since   iText 2.0.8
        */
        public override bool IsNestable() {
            return false;
        }
    }
}
