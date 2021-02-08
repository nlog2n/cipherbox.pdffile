using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

namespace iTextSharp.text.pdf 
{
    /** Reads an FDF form and makes the fields available
    * @author Paulo Soares
    */
    public class FdfReader : PdfReader 
    {
        
        internal Dictionary<String, PdfDictionary> fields;
        internal String fileSpec;
        internal PdfName encoding;
        
        /** Reads an FDF form.
        * @param filename the file name of the form
        * @throws IOException on error
        */    
        public FdfReader(String filename) : base(filename) {
        }
        
        /** Reads an FDF form.
        * @param pdfIn the byte array with the form
        * @throws IOException on error
        */    
        public FdfReader(byte[] pdfIn) : base(pdfIn) {
        }
        
        protected internal override void ReadPdf() {
            fields = new Dictionary<string,PdfDictionary>();
            try {
                tokens.CheckFdfHeader();
                RebuildXref();
                ReadDocObj();
            }
            finally {
                try {
                    tokens.Close();
                }
                catch  {
                    // empty on purpose
                }
            }
            ReadFields();
        }
        
        protected virtual void KidNode(PdfDictionary merged, String name) {
            PdfArray kids = merged.GetAsArray(PdfName.KIDS);
            if (kids == null || kids.Size == 0) {
                if (name.Length > 0)
                    name = name.Substring(1);
                fields[name] = merged;
            }
            else {
                merged.Remove(PdfName.KIDS);
                for (int k = 0; k < kids.Size; ++k) {
                    PdfDictionary dic = new PdfDictionary();
                    dic.Merge(merged);
                    PdfDictionary newDic = kids.GetAsDict(k);
                    PdfString t = newDic.GetAsString(PdfName.T);
                    String newName = name;
                    if (t != null)
                        newName += "." + t.ToUnicodeString();
                    dic.Merge(newDic);
                    dic.Remove(PdfName.T);
                    KidNode(dic, newName);
                }
            }
        }
        
        protected virtual void ReadFields() {
            catalog = trailer.GetAsDict(PdfName.ROOT);
            PdfDictionary fdf = catalog.GetAsDict(PdfName.FDF);
            if (fdf == null)
                return;
            PdfString fs = fdf.GetAsString(PdfName.F);
            if (fs != null)
                fileSpec = fs.ToUnicodeString();
            PdfArray fld = fdf.GetAsArray(PdfName.FIELDS);
            if (fld == null)
                return;
            encoding = fdf.GetAsName(PdfName.ENCODING);
            PdfDictionary merged = new PdfDictionary();
            merged.Put(PdfName.KIDS, fld);
            KidNode(merged, "");
        }

        /** Gets all the fields. The map is keyed by the fully qualified
        * field name and the value is a merged <CODE>PdfDictionary</CODE>
        * with the field content.
        * @return all the fields
        */    
        public Dictionary<String, PdfDictionary> Fields {
            get {
                return fields;
            }
        }
        
        /** Gets the field dictionary.
        * @param name the fully qualified field name
        * @return the field dictionary
        */    
        public PdfDictionary GetField(String name) {
            PdfDictionary dic;
            fields.TryGetValue(name, out dic);
            return dic;;
        }
        
        /**
        * Gets a byte[] containing a file that is embedded in the FDF.
        * @param name the fully qualified field name
        * @return the bytes of the file
        * @throws IOException 
        * @since 5.0.1 
        */
        public byte[] GetAttachedFile(String name) {
            PdfDictionary field = GetField(name);
            if (field != null) {
                PdfIndirectReference ir = (PRIndirectReference)field.Get(PdfName.V);
                PdfDictionary filespec = (PdfDictionary)GetPdfObject(ir.Number);
                PdfDictionary ef = filespec.GetAsDict(PdfName.EF);
                ir = (PRIndirectReference)ef.Get(PdfName.F);
                PRStream stream = (PRStream)GetPdfObject(ir.Number);
                return GetStreamBytes(stream);
            }
            return new byte[0];
        }
        
        /** Gets the field value or <CODE>null</CODE> if the field does not
        * exist or has no value defined.
        * @param name the fully qualified field name
        * @return the field value or <CODE>null</CODE>
        */    
        public String GetFieldValue(String name) {
            PdfDictionary field = GetField(name);
            if (field == null)
                return null;
            PdfObject v = GetPdfObject(field.Get(PdfName.V));
            if (v == null)
                return null;
            if (v.IsName())
                return PdfName.DecodeName(((PdfName)v).ToString());
            else if (v.IsString()) {
                PdfString vs = (PdfString)v;
                if (encoding == null || vs.Encoding != null)
                    return vs.ToUnicodeString();
                byte[] b = vs.GetBytes();
                if (b.Length >= 2 && b[0] == (byte)254 && b[1] == (byte)255)
                    return vs.ToUnicodeString();
                try {
                    if (encoding.Equals(PdfName.SHIFT_JIS))
                        return Encoding.GetEncoding(932).GetString(b);
                    else if (encoding.Equals(PdfName.UHC))
                        return Encoding.GetEncoding(949).GetString(b);
                    else if (encoding.Equals(PdfName.GBK))
                        return Encoding.GetEncoding(936).GetString(b);
                    else if (encoding.Equals(PdfName.BIGFIVE))
                        return Encoding.GetEncoding(950).GetString(b);
                    else if (encoding.Equals(PdfName.UTF_8))
                        return Encoding.UTF8.GetString(b);
                }
                catch  {
                }
                return vs.ToUnicodeString();
            }
            return null;
        }
        
        /** Gets the PDF file specification contained in the FDF.
        * @return the PDF file specification contained in the FDF
        */    
        public String FileSpec {
            get {
                return fileSpec;
            }
        }
    }
}
