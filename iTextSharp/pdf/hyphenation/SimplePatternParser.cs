using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using CipherBox.Pdf.Utility;
using iTextSharp.text.xml.simpleparser;

namespace iTextSharp.text.pdf.hyphenation {
    /** Parses the xml hyphenation pattern.
    *
    * @author Paulo Soares
    */
    public class SimplePatternParser : ISimpleXMLDocHandler {
        internal int currElement;
        internal IPatternConsumer consumer;
        internal StringBuilder token;
        internal List<object> exception;
        internal char hyphenChar;
        
        internal const int ELEM_CLASSES = 1;
        internal const int ELEM_EXCEPTIONS = 2;
        internal const int ELEM_PATTERNS = 3;
        internal const int ELEM_HYPHEN = 4;

        /** Creates a new instance of PatternParser2 */
        public SimplePatternParser() {
            token = new StringBuilder();
            hyphenChar = '-';    // default
        }
        
        public void Parse(Stream stream, IPatternConsumer consumer) {
            this.consumer = consumer;
            try {
                SimpleXMLParser.Parse(this, stream);
            }
            finally {
                try{stream.Close();}catch{}
            }
        }
        
        protected static String GetPattern(String word) {
            StringBuilder pat = new StringBuilder();
            int len = word.Length;
            for (int i = 0; i < len; i++) {
                if (!char.IsDigit(word[i])) {
                    pat.Append(word[i]);
                }
            }
            return pat.ToString();
        }

        protected List<object> NormalizeException(List<object> ex) {
            List<object> res = new List<object>();
            for (int i = 0; i < ex.Count; i++) {
                Object item = ex[i];
                if (item is String) {
                    String str = (String)item;
                    StringBuilder buf = new StringBuilder();
                    for (int j = 0; j < str.Length; j++) {
                        char c = str[j];
                        if (c != hyphenChar) {
                            buf.Append(c);
                        } else {
                            res.Add(buf.ToString());
                            buf.Length = 0;
                            char[] h = new char[1];
                            h[0] = hyphenChar;
                            // we use here hyphenChar which is not necessarily
                            // the one to be printed
                            res.Add(new Hyphen(new String(h), null, null));
                        }
                    }
                    if (buf.Length > 0) {
                        res.Add(buf.ToString());
                    }
                } else {
                    res.Add(item);
                }
            }
            return res;
        }

        protected String GetExceptionWord(List<object> ex) {
            StringBuilder res = new StringBuilder();
            for (int i = 0; i < ex.Count; i++) {
                Object item = ex[i];
                if (item is String) {
                    res.Append((String)item);
                } else {
                    if (((Hyphen)item).noBreak != null) {
                        res.Append(((Hyphen)item).noBreak);
                    }
                }
            }
            return res.ToString();
        }

        protected static String GetInterletterValues(String pat) {
            StringBuilder il = new StringBuilder();
            String word = pat + "a";    // add dummy letter to serve as sentinel
            int len = word.Length;
            for (int i = 0; i < len; i++) {
                char c = word[i];
                if (char.IsDigit(c)) {
                    il.Append(c);
                    i++;
                } else {
                    il.Append('0');
                }
            }
            return il.ToString();
        }

        public void EndDocument() {
        }
        
        public void EndElement(String tag) {
            if (token.Length > 0) {
                String word = token.ToString();
                switch (currElement) {
                case ELEM_CLASSES:
                    consumer.AddClass(word);
                    break;
                case ELEM_EXCEPTIONS:
                    exception.Add(word);
                    exception = NormalizeException(exception);
                    consumer.AddException(GetExceptionWord(exception), new List<object>(exception));
                    break;
                case ELEM_PATTERNS:
                    consumer.AddPattern(GetPattern(word),
                                        GetInterletterValues(word));
                    break;
                case ELEM_HYPHEN:
                    // nothing to do
                    break;
                }
                if (currElement != ELEM_HYPHEN) {
                    token.Length = 0;
                }
            }
            if (currElement == ELEM_HYPHEN) {
                currElement = ELEM_EXCEPTIONS;
            } else {
                currElement = 0;
            }
        }
        
        public void StartDocument() {
        }
        
        public void StartElement(String tag, IDictionary<string,string> h) {
            if (tag.Equals("hyphen-char")) {
                String hh;
                h.TryGetValue("value", out hh);
                if (hh != null && hh.Length == 1) {
                    hyphenChar = hh[0];
                }
            } else if (tag.Equals("classes")) {
                currElement = ELEM_CLASSES;
            } else if (tag.Equals("patterns")) {
                currElement = ELEM_PATTERNS;
            } else if (tag.Equals("exceptions")) {
                currElement = ELEM_EXCEPTIONS;
                exception = new List<object>();
            } else if (tag.Equals("hyphen")) {
                if (token.Length > 0) {
                    exception.Add(token.ToString());
                }
                exception.Add(new Hyphen(h["pre"], h["no"], h["post"]));
                currElement = ELEM_HYPHEN;
            }
            token.Length = 0;
        }
        
        public void Text(String str) {
            StringTokenizer tk = new StringTokenizer(str);
            while (tk.HasMoreTokens()) {
                String word = tk.NextToken();
                // System.out.Println("\"" + word + "\"");
                switch (currElement) {
                case ELEM_CLASSES:
                    consumer.AddClass(word);
                    break;
                case ELEM_EXCEPTIONS:
                    exception.Add(word);
                    exception = NormalizeException(exception);
                    consumer.AddException(GetExceptionWord(exception), new List<object>(exception));
                    exception.Clear();
                    break;
                case ELEM_PATTERNS:
                    consumer.AddPattern(GetPattern(word),
                                        GetInterletterValues(word));
                    break;
                }
            }
        }
    }
}
