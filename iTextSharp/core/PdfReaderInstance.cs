using System;
using System.IO;
using System.Collections.Generic;

using iTextSharp.text.error_messages;
using iTextSharp.text;

namespace iTextSharp.text.pdf {
    /**
    * Instance of PdfReader in each output document.
    *
    * @author Paulo Soares
    */
    public class PdfReaderInstance {
        internal static PdfLiteral IDENTITYMATRIX = new PdfLiteral("[1 0 0 1 0 0]");
        internal static PdfNumber ONE = new PdfNumber(1);
        internal int[] myXref;
        internal PdfReader reader;
        internal RandomAccessFileOrArray file;
        internal Dictionary<int, PdfImportedPage> importedPages = new Dictionary<int,PdfImportedPage>();
        internal PdfWriter writer;
        internal Dictionary<int,object> visited = new Dictionary<int,object>();
        internal List<int> nextRound = new List<int>();
        
        internal PdfReaderInstance(PdfReader reader, PdfWriter writer) {
            this.reader = reader;
            this.writer = writer;
            file = reader.SafeFile;
            myXref = new int[reader.XrefSize];
        }
        
        internal PdfReader Reader {
            get {
                return reader;
            }
        }
        
        internal PdfImportedPage GetImportedPage(int pageNumber) {
            if (!reader.IsOpenedWithFullPermissions)
                throw new ArgumentException("pdfreader.not.opened.with.owner.password");
            if (pageNumber < 1 || pageNumber > reader.NumberOfPages)
                throw new ArgumentException("invalid.page.number. "+ pageNumber);
            PdfImportedPage pageT;
            if (!importedPages.TryGetValue(pageNumber, out pageT)) {
                pageT = new PdfImportedPage(this, writer, pageNumber);
                importedPages[pageNumber] = pageT;
            }
            return pageT;
        }
        
        internal int GetNewObjectNumber(int number, int generation) {
            if (myXref[number] == 0) {
                myXref[number] = writer.IndirectReferenceNumber;
                nextRound.Add(number);
            }
            return myXref[number];
        }
        
        internal RandomAccessFileOrArray ReaderFile {
            get {
                return file;
            }
        }
        
        internal PdfObject GetResources(int pageNumber) {
            PdfObject obj = PdfReader.GetPdfObjectRelease(reader.GetPageNRelease(pageNumber).Get(PdfName.RESOURCES));
            return obj;
        }
        
        /**
        * Gets the content stream of a page as a PdfStream object.
        * @param   pageNumber          the page of which you want the stream
        * @param   compressionLevel    the compression level you want to apply to the stream
        * @return  a PdfStream object
        * @since   2.1.3 (the method already existed without param compressionLevel)
        */
        internal PdfStream GetFormXObject(int pageNumber, int compressionLevel) {
            PdfDictionary page = reader.GetPageNRelease(pageNumber);
            PdfObject contents = PdfReader.GetPdfObjectRelease(page.Get(PdfName.CONTENTS));
            PdfDictionary dic = new PdfDictionary();
            byte[] bout = null;
            if (contents != null) {
                if (contents.IsStream())
                    dic.Merge((PRStream)contents);
                else
                    bout = reader.GetPageContent(pageNumber, file);
            }
            else
                bout = new byte[0];
            dic.Put(PdfName.RESOURCES, PdfReader.GetPdfObjectRelease(page.Get(PdfName.RESOURCES)));
            dic.Put(PdfName.TYPE, PdfName.XOBJECT);
            dic.Put(PdfName.SUBTYPE, PdfName.FORM);
            PdfImportedPage impPage = importedPages[pageNumber];
            dic.Put(PdfName.BBOX, new PdfRectangle(impPage.BoundingBox));
            PdfArray matrix = impPage.Matrix;
            if (matrix == null)
                dic.Put(PdfName.MATRIX, IDENTITYMATRIX);
            else
                dic.Put(PdfName.MATRIX, matrix);
            dic.Put(PdfName.FORMTYPE, ONE);
            PRStream stream;
            if (bout == null) {
                stream = new PRStream((PRStream)contents, dic);
            }
            else {
                stream = new PRStream(reader, bout);
                stream.Merge(dic);
            }
            return stream;
        }
        
        internal void WriteAllVisited() {
            while (nextRound.Count > 0) {
                List<int> vec = nextRound;
                nextRound = new List<int>();
                foreach (int i in vec) {
                    if (!visited.ContainsKey(i)) {
                        visited[i] = null;
                        writer.AddToBody(reader.GetPdfObjectRelease(i), myXref[i]);
                    }
                }
            }
        }
        
        public void WriteAllPages() {
            try {
                file.ReOpen();
                foreach (PdfImportedPage ip in importedPages.Values) {
                    if (ip.IsToCopy()) {
                        writer.AddToBody(ip.GetFormXObject(writer.CompressionLevel), ip.IndirectReference);
                        ip.SetCopied();
                    }
                }
                WriteAllVisited();
            }
            finally {
                try {
                    file.Close();
                }
                catch  {
                    //Empty on purpose
                }
            }
        }
    }
}
