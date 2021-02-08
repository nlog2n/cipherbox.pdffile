using System;

using iTextSharp.text.pdf;
using iTextSharp.text;
using iTextSharp.text.pdf.interfaces;

namespace iTextSharp.text.pdf.intern 
{
    /**
    * Stores the PDF version information,
    * knows how to write a PDF Header,
    * and how to add the version to the catalog (if necessary).
    */

    public class PdfVersionImp : IPdfVersion {

        /** Contains different strings that are part of the header. */
        public static readonly byte[][] HEADER = {
            DocWriter.GetISOBytes("\n"),
            DocWriter.GetISOBytes("%PDF-"),
            DocWriter.GetISOBytes("\n%\u00e2\u00e3\u00cf\u00d3\n")
        };
        /** Indicates if the header was already written. */
        protected bool headerWasWritten = false;
        /** Indicates if we are working in append mode. */
        protected bool appendmode = false;
        /** The version that was or will be written to the header. */
        protected char header_version = PdfWriter.VERSION_1_4;
        /** The version that will be written to the catalog. */
        protected PdfName catalog_version = null;
        /**
         * The extensions dictionary.
         * @since	2.1.6
         */
        protected PdfDictionary extensions = null;
        
        /**
        * @see com.lowagie.text.pdf.interfaces.PdfVersion#setPdfVersion(char)
        */
        public char PdfVersion {
            set {
                if (headerWasWritten || appendmode) {
                    SetPdfVersion(GetVersionAsName(value));
                }
                else {
                    this.header_version = value;
                }
            }
        }
        
        /**
        * @see com.lowagie.text.pdf.interfaces.PdfVersion#setAtLeastPdfVersion(char)
        */
        public void SetAtLeastPdfVersion(char version) {
            if (version > header_version) {
                PdfVersion = version;
            }
        }
        
        /**
        * @see com.lowagie.text.pdf.interfaces.PdfVersion#setPdfVersion(com.lowagie.text.pdf.PdfName)
        */
        public void SetPdfVersion(PdfName version) {
            if (catalog_version == null || catalog_version.CompareTo(version) < 0) {
                this.catalog_version = version;
            }
        }
        
        /**
        * Sets the append mode.
        */
        public void SetAppendmode(bool appendmode) {
            this.appendmode = appendmode;
        }
        
        /**
        * Writes the header to the OutputStreamCounter.
        * @throws IOException 
        */
        public void WriteHeader(OutputStreamCounter os) {
            if (appendmode) {
                os.Write(HEADER[0], 0, HEADER[0].Length);
            }
            else {
                os.Write(HEADER[1], 0, HEADER[1].Length);
                os.Write(GetVersionAsByteArray(header_version), 0, GetVersionAsByteArray(header_version).Length);
                os.Write(HEADER[2], 0, HEADER[2].Length);
                headerWasWritten = true;
            }
        }
        
        /**
        * Returns the PDF version as a name.
        * @param version    the version character.
        */
        public PdfName GetVersionAsName(char version) {
            switch (version) {
            case PdfWriter.VERSION_1_2:
                return PdfWriter.PDF_VERSION_1_2;
            case PdfWriter.VERSION_1_3:
                return PdfWriter.PDF_VERSION_1_3;
            case PdfWriter.VERSION_1_4:
                return PdfWriter.PDF_VERSION_1_4;
            case PdfWriter.VERSION_1_5:
                return PdfWriter.PDF_VERSION_1_5;
            case PdfWriter.VERSION_1_6:
                return PdfWriter.PDF_VERSION_1_6;
            case PdfWriter.VERSION_1_7:
                return PdfWriter.PDF_VERSION_1_7;
            default:
                return PdfWriter.PDF_VERSION_1_4;
            }
        }
        
        /**
        * Returns the version as a byte[].
        * @param version the version character
        */
        public byte[] GetVersionAsByteArray(char version) {
            return DocWriter.GetISOBytes(GetVersionAsName(version).ToString().Substring(1));
        }

	    /** Adds the version to the Catalog dictionary. */
	    public void AddToCatalog(PdfDictionary catalog) {
		    if(catalog_version != null) {
			    catalog.Put(PdfName.VERSION, catalog_version);
		    }
            if (extensions != null) {
                catalog.Put(PdfName.EXTENSIONS, extensions);
            }
        }

        public void AddDeveloperExtension(PdfDeveloperExtension de) 
        {
            if (extensions == null) {
                extensions = new PdfDictionary();
            }
            else {
                PdfDictionary extension = extensions.GetAsDict(de.Prefix);
                if (extension != null) {
                    int diff = de.Baseversion.CompareTo(extension.GetAsName(PdfName.BASEVERSION));
                    if (diff < 0)
                        return;
                    diff = de.ExtensionLevel - extension.GetAsNumber(PdfName.EXTENSIONLEVEL).IntValue;
                    if (diff <= 0)
                        return;
                }
            }
            extensions.Put(de.Prefix, de.GetDeveloperExtensions());
        }
    }
}
