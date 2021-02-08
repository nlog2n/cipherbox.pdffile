using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// PdfContentReaderTool.cs

using CipherBox.Pdf.Utility;
using iTextSharp.text;
using iTextSharp.text.pdf;

using CipherBox.Pdf.Parser;

namespace CipherBox.Pdf
{
    /**
     * A utility class that makes it cleaner to process content from pages of a PdfReader
     * through a specified RenderListener.
     */
    public class PdfReaderContentParser
    {
        /** the reader this parser will process */
        private PdfReader reader;

        public PdfReaderContentParser(PdfReader reader)
        {
            this.reader = reader;
        }

        /**
         * Processes content from the specified page number using the specified listener
         * @param <E> the type of the renderListener - this makes it easy to chain calls
         * @param pageNumber the page number to process
         * @param renderListener the listener that will receive render callbacks
         * @return the provided renderListener
         * @throws IOException if operations on the reader fail
         */
        public E ProcessContent<E>(int pageNumber, E renderListener) where E : IRenderListener
        {
            PdfDictionary pageDic = reader.GetPageN(pageNumber);
            PdfDictionary resourcesDic = pageDic.GetAsDict(PdfName.RESOURCES);

            PdfContentStreamProcessor processor = new PdfContentStreamProcessor(renderListener);
            processor.ProcessContent(ContentByteUtils.GetContentBytesForPage(reader, pageNumber), resourcesDic);
            return renderListener;
        }
    }



    /**
     * Tool that parses the content of a PDF document.
     */
    public class PdfContentReaderTool
    {
        #region Extracts text from a PDF file.

        /**
         * Extract text from a specified page using an extraction strategy.
         * @param reader the reader to extract text from
         * @param pageNumber the page to extract text from
         * @param strategy the strategy to use for extracting text
         * @return the extracted text
         * @throws IOException if any operation fails while reading from the provided PdfReader
         */
        public static String GetTextFromPage(PdfReader reader, int pageNumber, ITextExtractionStrategy strategy)
        {
            PdfReaderContentParser parser = new PdfReaderContentParser(reader);
            return parser.ProcessContent(pageNumber, strategy).GetResultantText();

        }

        /**
         * Extract text from a specified page using the default strategy.
         * <p><strong>Note:</strong> the default strategy is subject to change.  If using a specific strategy
         * is important, use {@link PdfTextExtractor#getTextFromPage(PdfReader, int, TextExtractionStrategy)}
         * @param reader the reader to extract text from
         * @param pageNumber the page to extract text from
         * @return the extracted text
         * @throws IOException if any operation fails while reading from the provided PdfReader
         */
        public static String GetTextFromPage(PdfReader reader, int pageNumber)
        {
            return GetTextFromPage(reader, pageNumber, new LocationTextExtractionStrategy());
        }

        #endregion

        
        
        /**
         * Shows the detail of a dictionary.
         * This is similar to the PdfLister functionality.
         * @param dic   the dictionary of which you want the detail
         * @return  a String representation of the dictionary
         */
        public static string ShowPageDictionary(PdfDictionary dic)
        {
            return ShowPageDictionary(dic, 0);
        }

        /**
         * Shows the detail of a dictionary recursively.
         * @param dic   the dictionary of which you want the detail
         * @param depth the depth of the current dictionary (for nested dictionaries)
         * @return  a String representation of the dictionary
         */
        private static String ShowPageDictionary(PdfDictionary dic, int depth)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append('(');
            IList<PdfName> subDictionaries = new List<PdfName>();
            foreach (PdfName key in dic.Keys)
            {
                PdfObject val = dic.GetDirectObject(key);
                if (val.IsDictionary())
                    subDictionaries.Add(key);
                builder.Append(key);
                builder.Append('=');
                builder.Append(val);
                builder.Append(", ");
            }
            builder.Length = (dic.Keys.Count > 0 ? builder.Length - 2 : builder.Length); // remove last ", "
            builder.Append(')');

            foreach (PdfName pdfSubDictionaryName in subDictionaries)
            {
                builder.Append('\n');
                for (int i = 0; i < depth + 1; i++)
                {
                    builder.Append('\t');
                }
                builder.Append("Subdictionary ");
                builder.Append(pdfSubDictionaryName);
                builder.Append(" = ");
                PdfDictionary subDic = dic.GetAsDict(pdfSubDictionaryName);
                builder.Append(ShowPageDictionary(subDic, depth + 1));
            }
            return builder.ToString();
        }

        /**
         * Displays a summary of the entries in the XObject dictionary for the stream
         * @param resourceDic the resource dictionary for the stream
         * @return a string with the summary of the entries
         * @throws IOException
         * @since 5.0.2
         */
        public static string ShowPageXObjects(PdfDictionary page)
        {
            PdfDictionary resources = page.GetAsDict(PdfName.RESOURCES);
            PdfDictionary xobjects = resources.GetAsDict(PdfName.XOBJECT);
            if (xobjects == null)
                return "No XObjects";

            StringBuilder sb = new StringBuilder();
            foreach (PdfName entryName in xobjects.Keys)
            {
                PdfStream xobjectStream = xobjects.GetAsStream(entryName);

                sb.Append("------ " + entryName + " - subtype = " + xobjectStream.Get(PdfName.SUBTYPE) + " = " + xobjectStream.GetAsNumber(PdfName.LENGTH) + " bytes ------\n");

                if (!xobjectStream.Get(PdfName.SUBTYPE).Equals(PdfName.IMAGE))
                {
                    byte[] contentBytes = ContentByteUtils.GetContentBytesFromContentObject(xobjectStream);
                    foreach (byte b in contentBytes)
                    {
                        sb.Append((char)b);
                    }
                    sb.Append("------ " + entryName + " - subtype = " + xobjectStream.Get(PdfName.SUBTYPE) + "End of Content" + "------\n");
                }
            }

            return sb.ToString();
        }


        private static string ShowPageContents(PdfDictionary page)
        {
            return null;
        }

        /**
         * Writes information about a specific page from PdfReader to the specified output stream.
         * @since 2.1.5
         * @param reader    the PdfReader to read the page content from
         * @param pageNum   the page number to read
         * @param out       the output stream to send the content to
         * @throws IOException
         */
        private static void ShowPage(PdfReader reader, int pageNum, TextWriter outp)
        {
            outp.WriteLine("==============Page " + pageNum + "====================");
            outp.WriteLine("- - - - - Dictionary - - - - - -");
            PdfDictionary page = reader.GetPageN(pageNum);
            outp.WriteLine(ShowPageDictionary(page));

            outp.WriteLine("- - - - - XObject Summary - - - - - -");
            outp.WriteLine(ShowPageXObjects(page));

            outp.WriteLine("- - - - - Content Stream - - - - - -");
            outp.Flush();
            
            RandomAccessFileOrArray f = reader.SafeFile;
            byte[] contentBytes = reader.GetPageContent(pageNum, f);
            f.Close();

            foreach (byte b in contentBytes)
            {
                outp.Write((char)b);
            }
            outp.Flush();

            outp.WriteLine("- - - - - Text Extraction - - - - - -");
            String extractedText = GetTextFromPage(reader, pageNum);
            if (extractedText.Length != 0)
                outp.WriteLine(extractedText);
            else
                outp.WriteLine("No text found on page " + pageNum);

            outp.WriteLine();
        }

        /**
         * Writes information about each page in a PDF file to the specified output stream.
         * @since 2.1.5
         * @param pdfFile   a File instance referring to a PDF file
         * @param out       the output stream to send the content to
         * @throws IOException
         */
        public static void ShowPage(string pdfFile, int pageNum, TextWriter outp)
        {
            PdfReader reader = new PdfReader(pdfFile);

            if (pageNum <= 0)
            {
                int maxPageNum = reader.NumberOfPages;
                for (pageNum = 1; pageNum <= maxPageNum; pageNum++)
                {
                    ShowPage(reader, pageNum, outp);
                }
            }
            else
            {
                ShowPage(reader, pageNum, outp);
            }
        }


    }
}