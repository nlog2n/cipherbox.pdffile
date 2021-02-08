using System;
using System.IO;
using System.Collections.Generic;

using iTextSharp.text.pdf.interfaces;
using iTextSharp.text.pdf.collection;
using iTextSharp.text.error_messages;

//using iTextSharp.text.pdf.security;


namespace iTextSharp.text.pdf
{
    /** Applies extra content to the pages of a PDF document.
    * This extra content can be all the objects allowed in PdfContentByte
    * including pages from other Pdfs. The original PDF will keep
    * all the interactive elements including bookmarks, links and form fields.
    * <p>
    * It is also possible to change the field values and to
    * flatten them. New fields can be added but not flattened.
    * @author Paulo Soares
    */
    public class PdfStamper : IPdfViewerPreferences, IPdfEncryptionSettings, IDisposable
    {
        /**
        * The writer
        */
        protected PdfStamperImp stamper;

        /// <summary>
        /// An optional string map to add or change values in the info dictionary.
        /// Entries with null values delete the key in the original info dictionary.
        /// </summary>
        public IDictionary<String, String> moreInfo;


        /** Starts the process of adding extra content to an existing PDF
        * document.
        * <p>
        * The reader will be closed when this PdfStamper is closed
        * @param reader the original document. It cannot be reused
        * @param os the output stream
        * @throws DocumentException on error
        * @throws IOException on error
        */
        public PdfStamper(PdfReader reader, Stream os)
        {
            stamper = new PdfStamperImp(reader, os, '\0');
        }

        /**
        * Starts the process of adding extra content to an existing PDF
        * document.
        * <p>
        * The reader will be closed when this PdfStamper is closed
        * @param reader the original document. It cannot be reused
        * @param os the output stream
        * @param pdfVersion the new pdf version or '\0' to keep the same version as the original
        * document
        * @throws DocumentException on error
        * @throws IOException on error
        */
        public PdfStamper(PdfReader reader, Stream os, char pdfVersion)
        {
            stamper = new PdfStamperImp(reader, os, pdfVersion);
        }

        /**
        * Replaces a page from this document with a page from other document. Only the content
        * is replaced not the fields and annotations. This method must be called before 
        * getOverContent() or getUndercontent() are called for the same page.
        * @param r the <CODE>PdfReader</CODE> from where the new page will be imported
        * @param pageImported the page number of the imported page
        * @param pageReplaced the page to replace in this document
        */
        public void ReplacePage(PdfReader r, int pageImported, int pageReplaced)
        {
            stamper.ReplacePage(r, pageImported, pageReplaced);
        }

        /**
        * Inserts a blank page. All the pages above and including <CODE>pageNumber</CODE> will
        * be shifted up. If <CODE>pageNumber</CODE> is bigger than the total number of pages
        * the new page will be the last one.
        * @param pageNumber the page number position where the new page will be inserted
        * @param mediabox the size of the new page
        */
        public void InsertPage(int pageNumber, Rectangle mediabox)
        {
            stamper.InsertPage(pageNumber, mediabox);
        }


        /**
        * Closes the document. No more content can be written after the
        * document is closed.
        * <p>
        * If closing a signed document with an external signature the closing must be done
        * in the <CODE>PdfSignatureAppearance</CODE> instance.
        * @throws DocumentException on error
        * @throws IOException on error
        */
        public void Close()
        {
            stamper.Close(moreInfo);
        }

        /** Gets a <CODE>PdfContentByte</CODE> to write under the page of
        * the original document.
        * @param pageNum the page number where the extra content is written
        * @return a <CODE>PdfContentByte</CODE> to write under the page of
        * the original document
        */
        public PdfContentByte GetUnderContent(int pageNum)
        {
            return stamper.GetUnderContent(pageNum);
        }

        /** Gets a <CODE>PdfContentByte</CODE> to write over the page of
        * the original document.
        * @param pageNum the page number where the extra content is written
        * @return a <CODE>PdfContentByte</CODE> to write over the page of
        * the original document
        */
        public PdfContentByte GetOverContent(int pageNum)
        {
            return stamper.GetOverContent(pageNum);
        }

        /** Checks if the content is automatically adjusted to compensate
        * the original page rotation.
        * @return the auto-rotation status
        */
        /** Flags the content to be automatically adjusted to compensate
        * the original page rotation. The default is <CODE>true</CODE>.
        * @param rotateContents <CODE>true</CODE> to set auto-rotation, <CODE>false</CODE>
        * otherwise
        */
        public bool RotateContents
        {
            set
            {
                stamper.RotateContents = value;
            }
            get
            {
                return stamper.RotateContents;
            }
        }

        /** Sets the encryption options for this document. The userPassword and the
        *  ownerPassword can be null or have zero length. In this case the ownerPassword
        *  is replaced by a random string. The open permissions for the document can be
        *  AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
        *  AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
        *  The permissions can be combined by ORing them.
        * @param userPassword the user password. Can be null or empty
        * @param ownerPassword the owner password. Can be null or empty
        * @param permissions the user permissions
        * @param encryptionType the type of encryption. It can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128 or ENCRYPTION_AES128.
        * Optionally DO_NOT_ENCRYPT_METADATA can be ored to output the metadata in cleartext
        * @throws DocumentException if the document is already open
        */
        public void SetEncryption(byte[] userPassword, byte[] ownerPassword, Permissions permissions, EncryptionTypes encryptionType)
        {
            if (stamper.ContentWritten)
                throw new DocumentException("content.was.already.written.to.the.output");
            stamper.SetEncryption(userPassword, ownerPassword, permissions, encryptionType);
        }

        /**
        * Sets the encryption options for this document. The userPassword and the
        *  ownerPassword can be null or have zero length. In this case the ownerPassword
        *  is replaced by a random string. The open permissions for the document can be
        *  AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
        *  AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
        *  The permissions can be combined by ORing them.
        * @param encryptionType the type of encryption. It can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128 or ENCRYPTION_AES128.
        * Optionally DO_NOT_ENCRYPT_METADATA can be ored to output the metadata in cleartext
        * @param userPassword the user password. Can be null or empty
        * @param ownerPassword the owner password. Can be null or empty
        * @param permissions the user permissions
        * @throws DocumentException if the document is already open
        */
        public void SetEncryption(EncryptionTypes encryptionType, String userPassword, String ownerPassword, Permissions permissions)
        {
            SetEncryption(DocWriter.GetISOBytes(userPassword), DocWriter.GetISOBytes(ownerPassword), permissions, encryptionType);
        }

        /** Gets a page from other PDF document. Note that calling this method more than
        * once with the same parameters will retrieve the same object.
        * @param reader the PDF document where the page is
        * @param pageNumber the page number. The first page is 1
        * @return the template representing the imported page
        */
        public PdfImportedPage GetImportedPage(PdfReader reader, int pageNumber)
        {
            return stamper.GetImportedPage(reader, pageNumber);
        }

        /** Gets the underlying PdfWriter.
        * @return the underlying PdfWriter
        */
        public PdfWriter Writer
        {
            get
            {
                return stamper;
            }
        }

        /** Gets the underlying PdfReader.
        * @return the underlying PdfReader
        */
        public PdfReader Reader
        {
            get
            {
                return stamper.reader;
            }
        }

        /** Gets the <CODE>AcroFields</CODE> object that allows to get and set field values
        * and to merge FDF forms.
        * @return the <CODE>AcroFields</CODE> object
        */
        public AcroFields AcroFields
        {
            get
            {
                return stamper.GetAcroFields();
            }
        }

        /** Determines if the fields are flattened on close. The fields added with
        * {@link #addAnnotation(PdfAnnotation,int)} will never be flattened.
        * @param flat <CODE>true</CODE> to flatten the fields, <CODE>false</CODE>
        * to keep the fields
        */
        public bool FormFlattening
        {
            set
            {
                stamper.FormFlattening = value;
            }
        }

        /** Determines if the FreeText annotations are flattened on close. 
        * @param flat <CODE>true</CODE> to flatten the FreeText annotations, <CODE>false</CODE>
        * (the default) to keep the FreeText annotations as active content.
        */
        public bool FreeTextFlattening
        {
            set
            {
                stamper.FreeTextFlattening = value;
            }
        }
        /**
        * Adds an annotation of form field in a specific page. This page number
        * can be overridden with {@link PdfAnnotation#setPlaceInPage(int)}.
        * @param annot the annotation
        * @param page the page
        */
        public void AddAnnotation(PdfAnnotation annot, int page)
        {
            stamper.AddAnnotation(annot, page);
        }

     
        /**
        * Adds the comments present in an FDF file.
        * @param fdf the FDF file
        * @throws IOException on error
        */
        public void AddComments(FdfReader fdf)
        {
            stamper.AddComments(fdf);
        }

        /**
        * Sets the bookmarks. The list structure is defined in
        * {@link SimpleBookmark}.
        * @param outlines the bookmarks or <CODE>null</CODE> to remove any
        */
        public IList<Dictionary<String, Object>> Outlines
        {
            set
            {
                stamper.Outlines = value;
            }
        }

        /**
        * Sets the thumbnail image for a page.
        * @param image the image
        * @param page the page
        * @throws PdfException on error
        * @throws DocumentException on error
        */
        public void SetThumbnail(Image image, int page)
        {
            stamper.SetThumbnail(image, page);
        }

        /**
        * Adds <CODE>name</CODE> to the list of fields that will be flattened on close,
        * all the other fields will remain. If this method is never called or is called
        * with invalid field names, all the fields will be flattened.
        * <p>
        * Calling <CODE>setFormFlattening(true)</CODE> is needed to have any kind of
        * flattening.
        * @param name the field name
        * @return <CODE>true</CODE> if the field exists, <CODE>false</CODE> otherwise
        */
        public bool PartialFormFlattening(String name)
        {
            return stamper.PartialFormFlattening(name);
        }

        /** Adds a JavaScript action at the document level. When the document
        * opens all this JavaScript runs. The existing JavaScript will be replaced.
        * @param js the JavaScript code
        */
        public string JavaScript
        {
            set
            {
                stamper.AddJavaScript(value, !PdfEncodings.IsPdfDocEncoding(value));
            }
        }

        /** Adds a file attachment at the document level. Existing attachments will be kept.
        * @param description the file description
        * @param fileStore an array with the file. If it's <CODE>null</CODE>
        * the file will be read from the disk
        * @param file the path to the file. It will only be used if
        * <CODE>fileStore</CODE> is not <CODE>null</CODE>
        * @param fileDisplay the actual file name stored in the pdf
        * @throws IOException on error
        */
        public void AddFileAttachment(String description, byte[] fileStore, String file, String fileDisplay)
        {
            AddFileAttachment(description, PdfFileSpecification.FileEmbedded(stamper, file, fileDisplay, fileStore));
        }

        /** Adds a file attachment at the document level. Existing attachments will be kept.
        * @param description the file description
        * @param fs the file specification
        */
        public void AddFileAttachment(String description, PdfFileSpecification fs)
        {
            stamper.AddFileAttachment(description, fs);
        }

        /**
        * This is the most simple way to change a PDF into a
        * portable collection. Choose one of the following names:
        * <ul>
        * <li>PdfName.D (detailed view)
        * <li>PdfName.T (tiled view)
        * <li>PdfName.H (hidden)
        * </ul>
        * Pass this name as a parameter and your PDF will be
        * a portable collection with all the embedded and
        * attached files as entries.
        * @param initialView can be PdfName.D, PdfName.T or PdfName.H
        */
        public void MakePackage(PdfName initialView)
        {
            PdfCollection collection = new PdfCollection(0);
            collection.Put(PdfName.VIEW, initialView);
            stamper.MakePackage(collection);
        }

        /**
        * Adds or replaces the Collection Dictionary in the Catalog.
        * @param    collection  the new collection dictionary.
        */
        public void MakePackage(PdfCollection collection)
        {
            stamper.MakePackage(collection);
        }

        /**
        * Sets the viewer preferences.
        * @param preferences the viewer preferences
        * @see PdfViewerPreferences#setViewerPreferences(int)
        */
        public virtual int ViewerPreferences
        {
            set
            {
                stamper.ViewerPreferences = value;
            }
        }

        /** Adds a viewer preference
        * @param preferences the viewer preferences
        * @see PdfViewerPreferences#addViewerPreference
        */

        public virtual void AddViewerPreference(PdfName key, PdfObject value)
        {
            stamper.AddViewerPreference(key, value);
        }

        /**
        * Sets the XMP metadata.
        * @param xmp
        * @see PdfWriter#setXmpMetadata(byte[])
        */
        public byte[] XmpMetadata
        {
            set
            {
                stamper.XmpMetadata = value;
            }
        }

        /**
        * Gets the 1.5 compression status.
        * @return <code>true</code> if the 1.5 compression is on
        */
        public bool FullCompression
        {
            get
            {
                return stamper.FullCompression;
            }
        }

        /**
        * Sets the document's compression to the new 1.5 mode with object streams and xref
        * streams. It can be set at any time but once set it can't be unset.
        */
        public void SetFullCompression()
        {
            stamper.SetFullCompression();
        }

        /**
        * Sets the open and close page additional action.
        * @param actionType the action type. It can be <CODE>PdfWriter.PAGE_OPEN</CODE>
        * or <CODE>PdfWriter.PAGE_CLOSE</CODE>
        * @param action the action to perform
        * @param page the page where the action will be applied. The first page is 1
        * @throws PdfException if the action type is invalid
        */
        public void SetPageAction(PdfName actionType, PdfAction action, int page)
        {
            stamper.SetPageAction(actionType, action, page);
        }

        /**
        * Sets the display duration for the page (for presentations)
        * @param seconds   the number of seconds to display the page. A negative value removes the entry
        * @param page the page where the duration will be applied. The first page is 1
        */
        public void SetDuration(int seconds, int page)
        {
            stamper.SetDuration(seconds, page);
        }

        /**
        * Sets the transition for the page
        * @param transition   the transition object. A <code>null</code> removes the transition
        * @param page the page where the transition will be applied. The first page is 1
        */
        public void SetTransition(PdfTransition transition, int page)
        {
            stamper.SetTransition(transition, page);
        }

        /**
        * Gets the PdfLayer objects in an existing document as a Map
        * with the names/titles of the layers as keys.
        * @return   a Map with all the PdfLayers in the document (and the name/title of the layer as key)
        * @since    2.1.2
        */
        public Dictionary<string, PdfLayer> GetPdfLayers()
        {
            return stamper.GetPdfLayers();
        }

        public void Dispose()
        {
            Close();
        }

        public void MarkUsed(PdfObject obj)
        {
            stamper.MarkUsed(obj);
        }

    }
}
