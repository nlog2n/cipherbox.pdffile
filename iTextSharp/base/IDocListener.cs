using System;


namespace iTextSharp.text {
    /// <summary>
    /// A class that implements DocListener will perform some
    /// actions when some actions are performed on a Document.
    /// </summary>
    /// <seealso cref="T:iTextSharp.text.IElementListener"/>
    /// <seealso cref="T:iTextSharp.text.Document"/>
    /// <seealso cref="T:iTextSharp.text.DocWriter"/>
    public interface IDocListener : IElementListener, IDisposable {
    
        // methods
    
        /// <summary>
        /// Signals that the Document has been opened and that
        /// Elements can be added.
        /// </summary>
        void Open();
    
        /// <summary>
        /// Signals that the Document was closed and that no other
        /// Elements will be added.
        /// </summary>
        /// <remarks>
        /// The output stream of every writer implementing IDocListener will be closed.
        /// </remarks>
        void Close();

        /// <summary>
        /// Signals that an new page has to be started.
        /// </summary>
        /// <returns>true if the page was added, false if not.</returns>
        bool NewPage();
    
        /// <summary>
        /// Sets the pagesize.
        /// </summary>
        /// <param name="pageSize">the new pagesize</param>
        /// <returns>a boolean</returns>
        bool SetPageSize(Rectangle pageSize);
    
        /// <summary>
        /// Sets the margins.
        /// </summary>
        /// <param name="marginLeft">the margin on the left</param>
        /// <param name="marginRight">the margin on the right</param>
        /// <param name="marginTop">the margin on the top</param>
        /// <param name="marginBottom">the margin on the bottom</param>
        /// <returns></returns>
        bool SetMargins(float marginLeft, float marginRight, float marginTop, float marginBottom);
    
        /**
        * Parameter that allows you to do margin mirroring (odd/even pages)
        * @param marginMirroring
        * @return true if succesfull
        */
        bool SetMarginMirroring(bool marginMirroring);

        /**
        * Parameter that allows you to do top/bottom margin mirroring (odd/even pages)
        * @param marginMirroringTopBottom
        * @return true if successful
        * @since	2.1.6
        */
        bool SetMarginMirroringTopBottom(bool marginMirroringTopBottom); // [L6]

        /// <summary>
        /// Sets the page number.
        /// </summary>
        /// <value>the new page number</value>
        int PageCount {
            set;
        }
    
        /// <summary>
        /// Sets the page number to 0.
        /// </summary>
        void ResetPageCount();    
    }
}
