using System;
using System.Collections.Generic;
using System.IO;

namespace iTextSharp.text.pdf 
{
    /**
    * Parses the page or template content.
    * @author Paulo Soares
    */
    public class PdfContentParser {
        
        /**
        * Commands have this type.
        */    
        public const int COMMAND_TYPE = 200;
        /**
        * Holds value of property tokeniser.
        */
        private PRTokeniser tokeniser;    
        
        /**
        * Creates a new instance of PdfContentParser
        * @param tokeniser the tokeniser with the content
        */
        public PdfContentParser(PRTokeniser tokeniser) {
            this.tokeniser = tokeniser;
        }
        
        /**
        * Parses a single command from the content. Each command is output as an array of arguments
        * having the command itself as the last element. The returned array will be empty if the
        * end of content was reached.
        * @param ls an <CODE>ArrayList</CODE> to use. It will be cleared before using. If it's
        * <CODE>null</CODE> will create a new <CODE>ArrayList</CODE>
        * @return the same <CODE>ArrayList</CODE> given as argument or a new one
        * @throws IOException on error
        */    
        public List<PdfObject> Parse(List<PdfObject> ls) {
            if (ls == null)
                ls = new List<PdfObject>();
            else
                ls.Clear();
            PdfObject ob = null;
            while ((ob = ReadPRObject()) != null) {
                ls.Add(ob);
                if (ob.Type == COMMAND_TYPE)
                    break;
            }
            return ls;
        }
        
        /**
        * Gets the tokeniser.
        * @return the tokeniser.
        */
        public PRTokeniser GetTokeniser() {
            return this.tokeniser;
        }
        
        /**
        * Sets the tokeniser.
        * @param tokeniser the tokeniser
        */
        public PRTokeniser Tokeniser {
            set {
                tokeniser = value;
            }
            get {
                return tokeniser;
            }
        }
        
        /**
        * Reads a dictionary. The tokeniser must be positioned past the "&lt;&lt;" token.
        * @return the dictionary
        * @throws IOException on error
        */    
        public PdfDictionary ReadDictionary() {
            PdfDictionary dic = new PdfDictionary();
            while (true) {
                if (!NextValidToken())
                    throw new IOException("unexpected.end.of.file");
                    if (tokeniser.TokenType == PRTokeniser.TokType.END_DIC)
                        break;
                    if (tokeniser.TokenType == PRTokeniser.TokType.OTHER && "def".CompareTo(tokeniser.StringValue) == 0)
                        continue;
                    if (tokeniser.TokenType != PRTokeniser.TokType.NAME)
                        throw new IOException("dictionary.key.is.not.a.name");
                    PdfName name = new PdfName(tokeniser.StringValue);
                    PdfObject obj = ReadPRObject();
                    int type = obj.Type;
                    if (-type == (int)PRTokeniser.TokType.END_DIC)
                        throw new IOException("unexpected.gt.gt");
                    if (-type == (int)PRTokeniser.TokType.END_ARRAY)
                        throw new IOException("unexpected.close.bracket");
                    dic.Put(name, obj);
            }
            return dic;
        }
        
        /**
        * Reads an array. The tokeniser must be positioned past the "[" token.
        * @return an array
        * @throws IOException on error
        */    
        public PdfArray ReadArray() {
            PdfArray array = new PdfArray();
            while (true) {
                PdfObject obj = ReadPRObject();
                int type = obj.Type;
                if (-type == (int)PRTokeniser.TokType.END_ARRAY)
                    break;
                if (-type == (int)PRTokeniser.TokType.END_DIC)
                    throw new IOException("Unexpected '>>'");
                array.Add(obj);
            }
            return array;
        }
        
        /**
        * Reads a pdf object.
        * @return the pdf object
        * @throws IOException on error
        */    
        public PdfObject ReadPRObject() {
            if (!NextValidToken())
                return null;
            PRTokeniser.TokType type = tokeniser.TokenType;
            switch (type) {
                case PRTokeniser.TokType.START_DIC: {
                    PdfDictionary dic = ReadDictionary();
                    return dic;
                }
                case PRTokeniser.TokType.START_ARRAY:
                    return ReadArray();
                case PRTokeniser.TokType.STRING:
                    PdfString str = new PdfString(tokeniser.StringValue, null).SetHexWriting(tokeniser.IsHexString());
                    return str;
                case PRTokeniser.TokType.NAME:
                    return new PdfName(tokeniser.StringValue);
                case PRTokeniser.TokType.NUMBER:
                    return new PdfNumber(tokeniser.StringValue);
                 case PRTokeniser.TokType.OTHER:
                    return new PdfLiteral(COMMAND_TYPE, tokeniser.StringValue);
                default:
                    return new PdfLiteral(-(int)type, tokeniser.StringValue);
            }
        }
        
        /**
        * Reads the next token skipping over the comments.
        * @return <CODE>true</CODE> if a token was read, <CODE>false</CODE> if the end of content was reached
        * @throws IOException on error
        */    
        public bool NextValidToken() {
            while (tokeniser.NextToken()) {
                if (tokeniser.TokenType == PRTokeniser.TokType.COMMENT)
                    continue;
                return true;
            }
            return false;
        }
    }
}
