using System;
using System.Collections.Generic;
using System.IO;

using iTextSharp.text.pdf.interfaces;

namespace iTextSharp.text.pdf {

    /**
    * Allows you to add one (or more) existing PDF document(s) to
    * create a new PDF and add the form of another PDF document to
    * this new PDF.
    * @since 2.1.5
    */
    public class PdfCopyForms : IPdfViewerPreferences, IPdfEncryptionSettings {
        
        /** The class with the actual implementations. */
        private PdfCopyFormsImp fc;
        
        /**
        * Creates a new instance.
        * @param os the output stream
        * @throws DocumentException on error
        */    
        public PdfCopyForms(Stream os) {
            fc = new PdfCopyFormsImp(os);
        }
        
        /**
        * Concatenates a PDF document.
        * @param reader the PDF document
        * @throws DocumentException on error
        */    
        public void AddDocument(PdfReader reader) {
            fc.AddDocument(reader);
        }
        
        /**
        * Concatenates a PDF document selecting the pages to keep. The pages are described as a
        * <CODE>List</CODE> of <CODE>Integer</CODE>. The page ordering can be changed but
        * no page repetitions are allowed.
        * @param reader the PDF document
        * @param pagesToKeep the pages to keep
        * @throws DocumentException on error
        */    
        public void AddDocument(PdfReader reader, ICollection<int> pagesToKeep) {
            fc.AddDocument(reader, pagesToKeep);
        }

        /**
        * Concatenates a PDF document selecting the pages to keep. The pages are described as
        * ranges. The page ordering can be changed but
        * no page repetitions are allowed.
        * @param reader the PDF document
        * @param ranges the comma separated ranges as described in {@link SequenceList}
        * @throws DocumentException on error
        */    
        public void AddDocument(PdfReader reader, String ranges) {
            fc.AddDocument(reader, SequenceList.Expand(ranges, reader.NumberOfPages));
        }

        /**
        *Copies the form fields of this PDFDocument onto the PDF-Document which was added
        * @param reader the PDF document
        * @throws DocumentException on error
        */
        public void CopyDocumentFields(PdfReader reader) {
            fc.CopyDocumentFields(reader);
        }

     
        /**
        * Closes the output document.
        */    
        public void Close() {
            fc.Close();
        }

        /**
        * Opens the document. This is usually not needed as addDocument() will do it
        * automatically.
        */    
        public void Open() {
            fc.OpenDoc();
        }

        /**
        * Adds JavaScript to the global document
        * @param js the JavaScript
        */    
        public void AddJavaScript(String js) {
            fc.AddJavaScript(js, !PdfEncodings.IsPdfDocEncoding(js));
        }

        /**
        * Sets the bookmarks. The list structure is defined in
        * <CODE>SimpleBookmark#</CODE>.
        * @param outlines the bookmarks or <CODE>null</CODE> to remove any
        */    
        public IList<Dictionary<string,object>> Outlines {
            set {
                fc.Outlines = value;
            }
        }
        
        /** Gets the underlying PdfWriter.
        * @return the underlying PdfWriter
        */    
        public PdfWriter Writer {
            get {
                return fc;
            }
        }

        /**
        * Gets the 1.5 compression status.
        * @return <code>true</code> if the 1.5 compression is on
        */
        public bool FullCompression {
            get {
                return fc.FullCompression;
            }
        }
        
        /**
        * Sets the document's compression to the new 1.5 mode with object streams and xref
        * streams. It can be set at any time but once set it can't be unset.
        * <p>
        * If set before opening the document it will also set the pdf version to 1.5.
        */
        public void SetFullCompression() {
            fc.SetFullCompression();
        }

        /**
        * @see com.lowagie.text.pdf.interfaces.PdfEncryptionSettings#setEncryption(byte[], byte[], int, int)
        */
        public void SetEncryption(byte[] userPassword, byte[] ownerPassword, Permissions permissions, EncryptionTypes encryptionType)
        {
            fc.SetEncryption(userPassword, ownerPassword, permissions, encryptionType);
        }

        /**
        * @see com.lowagie.text.pdf.interfaces.PdfViewerPreferences#addViewerPreference(com.lowagie.text.pdf.PdfName, com.lowagie.text.pdf.PdfObject)
        */
        public void AddViewerPreference(PdfName key, PdfObject value) {
            fc.AddViewerPreference(key, value); 
        }

        /**
        * @see com.lowagie.text.pdf.interfaces.PdfViewerPreferences#setViewerPreferences(int)
        */
        public int ViewerPreferences {
            set {
                fc.ViewerPreferences = value;
            }
        }

    }
}
