using System;
using System.Text;

using iTextSharp.text.exceptions;
using iTextSharp.text.io;

namespace iTextSharp.text.pdf
{
    public class PRTokeniser
    {
        public enum TokType
        {
            NUMBER = 1,
            STRING,
            NAME,
            COMMENT,
            START_ARRAY,
            END_ARRAY,
            START_DIC,
            END_DIC,
            REF,
            OTHER,
            ENDOFFILE
        }

        internal const string EMPTY = "";


        private readonly RandomAccessFileOrArray file;
        protected TokType type;
        protected string stringValue;
        protected int reference;
        protected int generation;
        protected bool hexString;

        /**
         * Creates a PRTokeniser for the specified {@link RandomAccessSource}.
         * The beginning of the file is read to determine the location of the header, and the data source is adjusted
         * as necessary to account for any junk that occurs in the byte source before the header
         * @param file the source
         */
        public PRTokeniser(RandomAccessFileOrArray file)
        {
            this.file = file;
        }

        public void Seek(long pos)
        {
            file.Seek(pos);
        }

        public long FilePointer
        {
            get
            {
                return file.FilePointer;
            }
        }

        public void Close()
        {
            file.Close();
        }

        public long Length
        {
            get
            {
                return file.Length;
            }
        }

        public int Read()
        {
            return file.Read();
        }

        public RandomAccessFileOrArray SafeFile
        {
            get
            {
                return new RandomAccessFileOrArray(file);
            }
        }

        public RandomAccessFileOrArray File
        {
            get
            {
                return file;
            }
        }

        public string ReadString(int size)
        {
            StringBuilder buf = new StringBuilder();
            int ch;
            while ((size--) > 0)
            {
                ch = Read();
                if (ch == -1)
                    break;
                buf.Append((char)ch);
            }
            return buf.ToString();
        }

        public static bool IsWhitespace(int ch)
        {
            return (ch == 0 || ch == 9 || ch == 10 || ch == 12 || ch == 13 || ch == 32);
        }

        public static bool IsDelimiter(int ch)
        {
            return (ch == '(' || ch == ')' || ch == '<' || ch == '>' || ch == '[' || ch == ']' || ch == '/' || ch == '%');
        }

        public TokType TokenType
        {
            get
            {
                return type;
            }
        }

        public string StringValue
        {
            get
            {
                return stringValue;
            }
        }

        public int Reference
        {
            get
            {
                return reference;
            }
        }

        public int Generation
        {
            get
            {
                return generation;
            }
        }

        public void BackOnePosition(int ch)
        {
            if (ch != -1)
                file.PushBack((byte)ch);
        }

        public void ThrowError(string error)
        {
            throw new InvalidPdfException(string.Format("{0} at.file.pointer {1}", error, file.FilePointer));
        }

        public int GetHeaderOffset()
        {
            String str = ReadString(1024);
            int idx = str.IndexOf("%PDF-");
            if (idx < 0)
            {
                idx = str.IndexOf("%FDF-");
                if (idx < 0)
                    throw new InvalidPdfException("pdf.header.not.found");
            }
            return idx;
        }

        public char CheckPdfHeader()
        {
            file.Seek(0);
            String str = ReadString(1024);
            int idx = str.IndexOf("%PDF-");
            if (idx != 0)
                throw new InvalidPdfException("pdf.header.not.found");
            return str[7];
        }

        public void CheckFdfHeader()
        {
            file.Seek(0);
            String str = ReadString(1024);
            int idx = str.IndexOf("%FDF-");
            if (idx != 0)
                throw new InvalidPdfException("fdf.header.not.found");
        }

        public long GetStartxref()
        {
            int arrLength = 1024;
            long fileLength = file.Length;
            long pos = fileLength - arrLength;
            if (pos < 1) pos = 1;
            while (pos > 0)
            {
                file.Seek(pos);
                String str = ReadString(arrLength);
                int idx = str.LastIndexOf("startxref");
                if (idx >= 0) return pos + idx;
                pos = pos - arrLength + 9; // 9 = "startxref".length()
            }
            throw new InvalidPdfException("pdf.startxref.not.found");
        }

        public static int GetHex(int v)
        {
            if (v >= '0' && v <= '9')
                return v - '0';
            if (v >= 'A' && v <= 'F')
                return v - 'A' + 10;
            if (v >= 'a' && v <= 'f')
                return v - 'a' + 10;
            return -1;
        }

        public void NextValidToken()
        {
            int level = 0;
            string n1 = null;
            string n2 = null;
            long ptr = 0;
            while (NextToken())
            {
                if (type == TokType.COMMENT)
                    continue;
                switch (level)
                {
                    case 0:
                        {
                            if (type != TokType.NUMBER)
                                return;
                            ptr = file.FilePointer;
                            n1 = stringValue;
                            ++level;
                            break;
                        }
                    case 1:
                        {
                            if (type != TokType.NUMBER)
                            {
                                file.Seek(ptr);
                                type = TokType.NUMBER;
                                stringValue = n1;
                                return;
                            }
                            n2 = stringValue;
                            ++level;
                            break;
                        }
                    default:
                        {
                            if (type != TokType.OTHER || !stringValue.Equals("R"))
                            {
                                file.Seek(ptr);
                                type = TokType.NUMBER;
                                stringValue = n1;
                                return;
                            }
                            type = TokType.REF;
                            reference = int.Parse(n1);
                            generation = int.Parse(n2);
                            return;
                        }
                }
            }
            if (level == 1)
            { // if the level 1 check returns EOF, then we are still looking at a number - set the type back to NUMBER
                type = TokType.NUMBER;
            }
            // if we hit here, the file is either corrupt (stream ended unexpectedly),
            // or the last token ended exactly at the end of a stream.  This last
            // case can occur inside an Object Stream.
        }

        public bool NextToken()
        {
            int ch = 0;
            do
            {
                ch = file.Read();
            } while (ch != -1 && IsWhitespace(ch));
            if (ch == -1)
            {
                type = TokType.ENDOFFILE;
                return false;
            }
            // Note:  We have to initialize stringValue here, after we've looked for the end of the stream,
            // to ensure that we don't lose the value of a token that might end exactly at the end
            // of the stream
            StringBuilder outBuf = new StringBuilder();
            stringValue = EMPTY;
            switch (ch)
            {
                case '[':
                    type = TokType.START_ARRAY;
                    break;
                case ']':
                    type = TokType.END_ARRAY;
                    break;
                case '/':
                    {
                        outBuf.Length = 0;
                        type = TokType.NAME;
                        while (true)
                        {
                            ch = file.Read();
                            if (ch == -1 || IsDelimiter(ch) || IsWhitespace(ch))
                                break;
                            if (ch == '#')
                            {
                                ch = (GetHex(file.Read()) << 4) + GetHex(file.Read());
                            }
                            outBuf.Append((char)ch);
                        }
                        BackOnePosition(ch);
                        break;
                    }
                case '>':
                    ch = file.Read();
                    if (ch != '>')
                        ThrowError("'>' not expected");
                    type = TokType.END_DIC;
                    break;
                case '<':
                    {
                        int v1 = file.Read();
                        if (v1 == '<')
                        {
                            type = TokType.START_DIC;
                            break;
                        }
                        outBuf.Length = 0;
                        type = TokType.STRING;
                        hexString = true;
                        int v2 = 0;
                        while (true)
                        {
                            while (IsWhitespace(v1))
                                v1 = file.Read();
                            if (v1 == '>')
                                break;
                            v1 = GetHex(v1);
                            if (v1 < 0)
                                break;
                            v2 = file.Read();
                            while (IsWhitespace(v2))
                                v2 = file.Read();
                            if (v2 == '>')
                            {
                                ch = v1 << 4;
                                outBuf.Append((char)ch);
                                break;
                            }
                            v2 = GetHex(v2);
                            if (v2 < 0)
                                break;
                            ch = (v1 << 4) + v2;
                            outBuf.Append((char)ch);
                            v1 = file.Read();
                        }
                        if (v1 < 0 || v2 < 0)
                            ThrowError("error.reading.string");
                        break;
                    }
                case '%':
                    type = TokType.COMMENT;
                    do
                    {
                        ch = file.Read();
                    } while (ch != -1 && ch != '\r' && ch != '\n');
                    break;
                case '(':
                    {
                        outBuf.Length = 0;
                        type = TokType.STRING;
                        hexString = false;
                        int nesting = 0;
                        while (true)
                        {
                            ch = file.Read();
                            if (ch == -1)
                                break;
                            if (ch == '(')
                            {
                                ++nesting;
                            }
                            else if (ch == ')')
                            {
                                --nesting;
                            }
                            else if (ch == '\\')
                            {
                                bool lineBreak = false;
                                ch = file.Read();
                                switch (ch)
                                {
                                    case 'n':
                                        ch = '\n';
                                        break;
                                    case 'r':
                                        ch = '\r';
                                        break;
                                    case 't':
                                        ch = '\t';
                                        break;
                                    case 'b':
                                        ch = '\b';
                                        break;
                                    case 'f':
                                        ch = '\f';
                                        break;
                                    case '(':
                                    case ')':
                                    case '\\':
                                        break;
                                    case '\r':
                                        lineBreak = true;
                                        ch = file.Read();
                                        if (ch != '\n')
                                            BackOnePosition(ch);
                                        break;
                                    case '\n':
                                        lineBreak = true;
                                        break;
                                    default:
                                        {
                                            if (ch < '0' || ch > '7')
                                            {
                                                break;
                                            }
                                            int octal = ch - '0';
                                            ch = file.Read();
                                            if (ch < '0' || ch > '7')
                                            {
                                                BackOnePosition(ch);
                                                ch = octal;
                                                break;
                                            }
                                            octal = (octal << 3) + ch - '0';
                                            ch = file.Read();
                                            if (ch < '0' || ch > '7')
                                            {
                                                BackOnePosition(ch);
                                                ch = octal;
                                                break;
                                            }
                                            octal = (octal << 3) + ch - '0';
                                            ch = octal & 0xff;
                                            break;
                                        }
                                }
                                if (lineBreak)
                                    continue;
                                if (ch < 0)
                                    break;
                            }
                            else if (ch == '\r')
                            {
                                ch = file.Read();
                                if (ch < 0)
                                    break;
                                if (ch != '\n')
                                {
                                    BackOnePosition(ch);
                                    ch = '\n';
                                }
                            }
                            if (nesting == -1)
                                break;
                            outBuf.Append((char)ch);
                        }
                        if (ch == -1)
                            ThrowError("error.reading.string");
                        break;
                    }
                default:
                    {
                        outBuf.Length = 0;
                        if (ch == '-' || ch == '+' || ch == '.' || (ch >= '0' && ch <= '9'))
                        {
                            type = TokType.NUMBER;
                            if (ch == '-')
                            {
                                // Take care of number like "--234". If Acrobat can read them so must we.
                                bool minus = false;
                                do
                                {
                                    minus = !minus;
                                    ch = file.Read();
                                } while (ch == '-');
                                if (minus)
                                    outBuf.Append('-');
                            }
                            else
                            {
                                outBuf.Append((char)ch);
                                ch = file.Read();
                            }
                            while (ch != -1 && ((ch >= '0' && ch <= '9') || ch == '.'))
                            {
                                outBuf.Append((char)ch);
                                ch = file.Read();
                            }
                        }
                        else
                        {
                            type = TokType.OTHER;
                            do
                            {
                                outBuf.Append((char)ch);
                                ch = file.Read();
                            } while (ch != -1 && !IsDelimiter(ch) && !IsWhitespace(ch));
                        }
                        if (ch != -1)
                            BackOnePosition(ch);
                        break;
                    }
            }
            if (outBuf != null)
                stringValue = outBuf.ToString();
            return true;
        }

        public long LongValue
        {
            get { return long.Parse(stringValue); }
        }

        public int IntValue
        {
            get
            {
                return int.Parse(stringValue);
            }
        }

        public bool ReadLineSegment(byte[] input)
        {
            int c = -1;
            bool eol = false;
            int ptr = 0;
            int len = input.Length;
            // ssteward, pdftk-1.10, 040922: 
            // skip initial whitespace; added this because PdfReader.RebuildXref()
            // assumes that line provided by readLineSegment does not have init. whitespace;
            if (ptr < len)
            {
                while (IsWhitespace((c = Read()))) ;
            }
            while (!eol && ptr < len)
            {
                switch (c)
                {
                    case -1:
                    case '\n':
                        eol = true;
                        break;
                    case '\r':
                        eol = true;
                        long cur = FilePointer;
                        if ((Read()) != '\n')
                        {
                            Seek(cur);
                        }
                        break;
                    default:
                        input[ptr++] = (byte)c;
                        break;
                }

                // break loop? do it before we Read() again
                if (eol || len <= ptr)
                {
                    break;
                }
                else
                {
                    c = Read();
                }
            }
            if (ptr >= len)
            {
                eol = false;
                while (!eol)
                {
                    switch (c = Read())
                    {
                        case -1:
                        case '\n':
                            eol = true;
                            break;
                        case '\r':
                            eol = true;
                            long cur = FilePointer;
                            if ((Read()) != '\n')
                            {
                                Seek(cur);
                            }
                            break;
                    }
                }
            }

            if ((c == -1) && (ptr == 0))
            {
                return false;
            }
            if (ptr + 2 <= len)
            {
                input[ptr++] = (byte)' ';
                input[ptr] = (byte)'X';
            }
            return true;
        }

        public static long[] CheckObjectStart(byte[] line)
        {
            try
            {
                PRTokeniser tk = new PRTokeniser(new RandomAccessFileOrArray(new RandomAccessSourceFactory().CreateSource(line)));
                int num = 0;
                int gen = 0;
                if (!tk.NextToken() || tk.TokenType != TokType.NUMBER)
                    return null;
                num = tk.IntValue;
                if (!tk.NextToken() || tk.TokenType != TokType.NUMBER)
                    return null;
                gen = tk.IntValue;
                if (!tk.NextToken())
                    return null;
                if (!tk.StringValue.Equals("obj"))
                    return null;
                return new long[] { num, gen };
            }
            catch
            {
            }
            return null;
        }

        public bool IsHexString()
        {
            return this.hexString;
        }

    }
}
