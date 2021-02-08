using System;
using System.Text;

using CipherBox.Pdf.Utility;

namespace iTextSharp.text.xml.xmp 
{
    public class LangAlt : Properties {

        /** Key for the default language. */
        public const String DEFAULT = "x-default";

        /** Creates a Properties object that stores languages for use in an XmpSchema */
        public LangAlt(String defaultValue) {
            AddLanguage(DEFAULT, defaultValue);
        }

        /** Creates a Properties object that stores languages for use in an XmpSchema */
        public LangAlt() {
        }

        /**
         * Add a language.
         */
        public void AddLanguage(String language, String value) {
            this[language] = XMLUtil.EscapeXML(value, false);
        }

        /**
         * Process a property.
         */
        protected internal void Process(StringBuilder buf, String lang) {
            buf.Append("<rdf:li xml:lang=\"");
            buf.Append(lang);
            buf.Append("\" >");
            buf.Append(this[lang]);
            buf.Append("</rdf:li>");
        }

        /**
         * Creates a String that can be used in an XmpSchema.
         */
        public override String ToString() {
            StringBuilder sb = new StringBuilder();
            sb.Append("<rdf:Alt>");
            foreach (String s in this.Keys)
                Process(sb, s);
            sb.Append("</rdf:Alt>");
            return sb.ToString();
        }
    }
}
