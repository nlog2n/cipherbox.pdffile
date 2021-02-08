using System;

namespace iTextSharp.text.xml.xmp 
{
    /**
    * An implementation of an XmpSchema.
    */
    public class PdfSchema : XmpSchema {
        
        /** default namespace identifier*/
        public const String DEFAULT_XPATH_ID = "pdf";
        /** default namespace uri*/
        public const String DEFAULT_XPATH_URI = "http://ns.adobe.com/pdf/1.3/";
        /** Keywords. */
        public const String KEYWORDS = "pdf:Keywords";
        /** The PDF file version (for example: 1.0, 1.3, and so on). */
        public const String VERSION = "pdf:PDFVersion";
        /** The Producer. */
        public const String PRODUCER = "pdf:Producer";
        
        /**
        * @throws IOException
        */
        public PdfSchema() : base("xmlns:" + DEFAULT_XPATH_ID + "=\"" + DEFAULT_XPATH_URI + "\"") 
        {
            AddProducer(SoftwareVersion.Version);
        }
        
        /**
        * Adds keywords.
        * @param keywords
        */
        public void AddKeywords(String keywords) {
            this[KEYWORDS] = keywords;
        }
        
        /**
        * Adds the producer.
        * @param producer
        */
        public void AddProducer(String producer) {
            this[PRODUCER] = producer;
        }

        /**
        * Adds the version.
        * @param version
        */
        public void AddVersion(String version) {
            this[VERSION] = version;
        }
    }
}
