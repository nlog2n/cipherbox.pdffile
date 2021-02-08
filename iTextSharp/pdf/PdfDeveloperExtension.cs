using System;

namespace iTextSharp.text.pdf 
{
    /**
    * Beginning with BaseVersion 1.7, the extensions dictionary lets developers
    * designate that a given document contains extensions to PDF. The presence
    * of the extension dictionary in a document indicates that it may contain
    * developer-specific PDF properties that extend a particular base version
    * of the PDF specification.
    * The extensions dictionary enables developers to identify their own extensions
    * relative to a base version of PDF. Additionally, the convention identifies
    * extension levels relative to that base version. The intent of this dictionary
    * is to enable developers of PDF-producing applications to identify company-specific
    * specifications (such as this one) that PDF-consuming applications use to
    * interpret the extensions.
    * @since   2.1.6
    */
    public class PdfDeveloperExtension {

        /** An instance of this class for Adobe 1.7 Extension level 3. */
        public static readonly PdfDeveloperExtension ADOBE_1_7_EXTENSIONLEVEL3 =
            new PdfDeveloperExtension(PdfName.ADBE, PdfWriter.PDF_VERSION_1_7, 3);
        
        /** The prefix used in the Extensions dictionary added to the Catalog. */
        protected PdfName prefix;
        /** The base version. */
        protected PdfName baseversion;
        /** The extension level within the baseversion. */
        protected int extensionLevel;
        
        /**
        * Creates a PdfDeveloperExtension object.
        * @param prefix    the prefix referring to the developer
        * @param baseversion   the number of the base version
        * @param extensionLevel    the extension level within the baseverion.
        */
        public PdfDeveloperExtension(PdfName prefix, PdfName baseversion, int extensionLevel) {
            this.prefix = prefix;
            this.baseversion = baseversion;
            this.extensionLevel = extensionLevel;
        }

        /**
        * Gets the prefix name.
        * @return  a PdfName
        */
        public PdfName Prefix {
            get {
                return prefix;
            }
        }

        /**
        * Gets the baseversion name.
        * @return  a PdfName
        */
        public PdfName Baseversion {
            get {
                return baseversion;
            }
        }

        /**
        * Gets the extension level within the baseversion.
        * @return  an integer
        */
        public int ExtensionLevel {
            get {
                return extensionLevel;
            }
        }
        
        /**
        * Generations the developer extension dictionary corresponding
        * with the prefix.
        * @return  a PdfDictionary
        */
        public PdfDictionary GetDeveloperExtensions() {
            PdfDictionary developerextensions = new PdfDictionary();
            developerextensions.Put(PdfName.BASEVERSION, baseversion);
            developerextensions.Put(PdfName.EXTENSIONLEVEL, new PdfNumber(extensionLevel));
            return developerextensions;
        }
    }
}
