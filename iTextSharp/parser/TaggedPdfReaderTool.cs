using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using iTextSharp.text.pdf;
using iTextSharp.text.error_messages;
using iTextSharp.text.xml.simpleparser;
using iTextSharp.text.xml;

namespace CipherBox.Pdf.Parser {

    /**
     * Converts a tagged PDF document into an XML file.
     * 
     * @since 5.0.2
     */
    public class TaggedPdfReaderTool {

        /** The reader obj from which the content streams are read. */
        internal protected PdfReader reader;
        /** The writer obj to which the XML will be written */
        internal protected StreamWriter outp;

        /**
         * Parses a string with structured content.
         * 
         * @param reader
         *            the PdfReader that has access to the PDF file
         * @param os
         *            the Stream to which the resulting xml will be written
         * @param charset
         *            the charset to encode the data
         * @since 5.0.5
         */
        public virtual void ConvertToXml(PdfReader reader, Stream os, Encoding encoding) {
            this.reader = reader;
            outp = new StreamWriter(os, encoding);
            // get the StructTreeRoot from the root obj
            PdfDictionary catalog = reader.Catalog;
            PdfDictionary struc = catalog.GetAsDict(PdfName.STRUCTTREEROOT);
            if (struc == null)
                throw new IOException(MessageLocalization.GetComposedMessage("no.structtreeroot.found"));
            // Inspect the child or children of the StructTreeRoot
            InspectChild(struc.GetDirectObject(PdfName.K));
            outp.Flush();
            outp.Close();
        }

        /**
         * Parses a string with structured content.
         * 
         * @param reader
         *            the PdfReader that has access to the PDF file
         * @param os
         *            the Stream to which the resulting xml will be written
         */
        public void ConvertToXml(PdfReader reader, Stream os) {
            ConvertToXml(reader, os, Encoding.Default);
        }

        /**
         * Inspects a child of a structured element. This can be an array or a
         * dictionary.
         * 
         * @param k
         *            the child to inspect
         * @throws IOException
         */
        public void InspectChild(PdfObject k) {
            if (k == null)
                return;
            if (k is PdfArray)
                InspectChildArray((PdfArray) k);
            else if (k is PdfDictionary)
                InspectChildDictionary((PdfDictionary) k);
        }

        /**
         * If the child of a structured element is an array, we need to loop over
         * the elements.
         * 
         * @param k
         *            the child array to inspect
         */
        public void InspectChildArray(PdfArray k) {
            if (k == null)
                return;
            for (int i = 0; i < k.Size; i++) {
                InspectChild(k.GetDirectObject(i));
            }
        }

            /**
         * If the child of a structured element is a dictionary, we inspect the
         * child; we may also draw a tag.
         *
         * @param k
         *            the child dictionary to inspect
         */
        public virtual void InspectChildDictionary(PdfDictionary k){
            InspectChildDictionary(k, false);
        }

        /**
         * If the child of a structured element is a dictionary, we inspect the
         * child; we may also draw a tag.
         * 
         * @param k
         *            the child dictionary to inspect
         */
        public void InspectChildDictionary(PdfDictionary k, bool inspectAttributes) {
            if (k == null)
                return;
            PdfName s = k.GetAsName(PdfName.S);
            if (s != null) {
                String tagN = PdfName.DecodeName(s.ToString());
			    String tag = FixTagName(tagN);
                outp.Write("<");
                outp.Write(tag);
                if (inspectAttributes) {
                    PdfDictionary a = k.GetAsDict(PdfName.A);
                    if (a != null) {
                        Dictionary<PdfName, PdfObject>.KeyCollection keys = a.Keys;
                        foreach (PdfName key in keys) {
                            outp.Write(' ');
                            PdfObject value = a.Get(key);
                            value = PdfReader.GetPdfObject(value);
                            outp.Write(XmlName(key));
                            outp.Write("=\"");
                            outp.Write(value.ToString());
                            outp.Write("\"");
                        }
                    }
                }
                outp.Write(">");
                PdfDictionary dict = k.GetAsDict(PdfName.PG);
                if (dict != null)
                    ParseTag(tagN, k.GetDirectObject(PdfName.K), dict);
                InspectChild(k.GetDirectObject(PdfName.K));
                outp.Write("</");
                outp.Write(tag);
                outp.WriteLine(">");
            } else
                InspectChild(k.GetDirectObject(PdfName.K));
        }

        protected String XmlName(PdfName name)
        {
            String oldName = name.ToString();
            String xmlName = oldName.Remove(oldName.IndexOf("/"), 1);
            xmlName = (xmlName.ToLower()[0])
                       + xmlName.Substring(1);
            return xmlName;
        }


        private static String FixTagName(String tag) {
            StringBuilder sb = new StringBuilder();
            for (int k = 0; k < tag.Length; ++k) {
                char c = tag[k];
                bool nameStart =
                    c == ':'
                    || (c >= 'A' && c <= 'Z')
                    || c == '_'
                    || (c >= 'a' && c <= 'z')
                    || (c >= '\u00c0' && c <= '\u00d6')
                    || (c >= '\u00d8' && c <= '\u00f6')
                    || (c >= '\u00f8' && c <= '\u02ff')
                    || (c >= '\u0370' && c <= '\u037d')
                    || (c >= '\u037f' && c <= '\u1fff')
                    || (c >= '\u200c' && c <= '\u200d')
                    || (c >= '\u2070' && c <= '\u218f')
                    || (c >= '\u2c00' && c <= '\u2fef')
                    || (c >= '\u3001' && c <= '\ud7ff')
                    || (c >= '\uf900' && c <= '\ufdcf')
                    || (c >= '\ufdf0' && c <= '\ufffd');
                bool nameMiddle =
                    c == '-'
                    || c == '.'
                    || (c >= '0' && c <= '9')
                    || c == '\u00b7'
                    || (c >= '\u0300' && c <= '\u036f')
                    || (c >= '\u203f' && c <= '\u2040')
                    || nameStart;
                if (k == 0) {
                    if (!nameStart)
                        c = '_';
                }
                else {
                    if (!nameMiddle)
                        c = '-';
                }
                sb.Append(c);
            }
            return sb.ToString();
        }

        /**
         * Searches for a tag in a page.
         * 
         * @param tag
         *            the name of the tag
         * @param obj
         *            an identifier to find the marked content
         * @param page
         *            a page dictionary
         * @throws IOException
         */
        public virtual void ParseTag(String tag, PdfObject obj, PdfDictionary page) {
            // if the identifier is a number, we can extract the content right away
            if (obj is PdfNumber) {
                PdfNumber mcid = (PdfNumber) obj;
                RenderFilter filter = new MarkedContentRenderFilter(mcid.IntValue);
                ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();
                FilteredTextRenderListener listener = new FilteredTextRenderListener(strategy, new RenderFilter[]{filter});
                PdfContentStreamProcessor processor = new PdfContentStreamProcessor(
                        listener);
                processor.ProcessContent(PdfReader.GetPageContent(page), page
                        .GetAsDict(PdfName.RESOURCES));
                outp.Write(XMLUtil.EscapeXML(listener.GetResultantText(), true));
            }
            // if the identifier is an array, we call the parseTag method
            // recursively
            else if (obj is PdfArray) {
                PdfArray arr = (PdfArray) obj;
                int n = arr.Size;
                for (int i = 0; i < n; i++) {
                    ParseTag(tag, arr[i], page);
                    if (i < n - 1)
                        outp.WriteLine();
                }
            }
            // if the identifier is a dictionary, we get the resources from the
            // dictionary
            else if (obj is PdfDictionary) {
                PdfDictionary mcr = (PdfDictionary) obj;
                ParseTag(tag, mcr.GetDirectObject(PdfName.MCID), mcr
                        .GetAsDict(PdfName.PG));
            }
        }
    }
}