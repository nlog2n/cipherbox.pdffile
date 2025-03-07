using System;
using System.Collections.Generic;
using System.Text;

using CipherBox.Pdf.Utility;

namespace iTextSharp.text.pdf {
    /**
    * This class expands a string into a list of numbers. The main use is to select a
    * range of pages.
    * <p>
    * The general systax is:<br>
    * [!][o][odd][e][even]start-end
    * <p>
    * You can have multiple ranges separated by commas ','. The '!' modifier removes the
    * range from what is already selected. The range changes are incremental, that is,
    * numbers are added or deleted as the range appears. The start or the end, but not both, can be ommited.
    */
    public class SequenceList {
        protected const int COMMA = 1;
        protected const int MINUS = 2;
        protected const int NOT = 3;
        protected const int TEXT = 4;
        protected const int NUMBER = 5;
        protected const int END = 6;
        protected const char EOT = '\uffff';

        private const int FIRST = 0;
        private const int DIGIT = 1;
        private const int OTHER = 2;
        private const int DIGIT2 = 3;
        private const String NOT_OTHER = "-,!0123456789";

        protected char[] text;
        protected int ptr;
        protected int number;
        protected String other;

        protected int low;
        protected int high;
        protected bool odd;
        protected bool even;
        protected bool inverse;

        protected SequenceList(String range) {
            ptr = 0;
            text = range.ToCharArray();
        }
        
        protected char NextChar() {
            while (true) {
                if (ptr >= text.Length)
                    return EOT;
                char c = text[ptr++];
                if (c > ' ')
                    return c;
            }
        }
        
        protected void PutBack() {
            --ptr;
            if (ptr < 0)
                ptr = 0;
        }
        
        protected int Type {
            get {
                StringBuilder buf = new StringBuilder();
                int state = FIRST;
                while (true) {
                    char c = NextChar();
                    if (c == EOT) {
                        if (state == DIGIT) {
                            number = int.Parse(other = buf.ToString());
                            return NUMBER;
                        }
                        else if (state == OTHER) {
                            other = buf.ToString().ToLower(System.Globalization.CultureInfo.InvariantCulture);
                            return TEXT;
                        }
                        return END;
                    }
                    switch (state) {
                        case FIRST:
                            switch (c) {
                                case '!':
                                    return NOT;
                                case '-':
                                    return MINUS;
                                case ',':
                                    return COMMA;
                            }
                            buf.Append(c);
                            if (c >= '0' && c <= '9')
                                state = DIGIT;
                            else
                                state = OTHER;
                            break;
                        case DIGIT:
                            if (c >= '0' && c <= '9')
                                buf.Append(c);
                            else {
                                PutBack();
                                number = int.Parse(other = buf.ToString());
                                return NUMBER;
                            }
                            break;
                        case OTHER:
                            if (NOT_OTHER.IndexOf(c) < 0)
                                buf.Append(c);
                            else {
                                PutBack();
                                other = buf.ToString().ToLower(System.Globalization.CultureInfo.InvariantCulture);
                                return TEXT;
                            }
                            break;
                    }
                }
            }
        }
        
        private void OtherProc() {
            if (other.Equals("odd") || other.Equals("o")) {
                odd = true;
                even = false;
            }
            else if (other.Equals("even") || other.Equals("e")) {
                odd = false;
                even = true;
            }
        }
        
        protected bool GetAttributes() {
            low = -1;
            high = -1;
            odd = even = inverse = false;
            int state = OTHER;
            while (true) {
                int type = Type;
                if (type == END || type == COMMA) {
                    if (state == DIGIT)
                        high = low;
                    return (type == END);
                }
                switch (state) {
                    case OTHER:
                        switch (type) {
                            case NOT:
                                inverse = true;
                                break;
                            case MINUS:
                                state = DIGIT2;
                                break;
                            default:
                                if (type == NUMBER) {
                                    low = number;
                                    state = DIGIT;
                                }
                                else
                                    OtherProc();
                                break;
                        }
                        break;
                    case DIGIT:
                        switch (type) {
                            case NOT:
                                inverse = true;
                                state = OTHER;
                                high = low;
                                break;
                            case MINUS:
                                state = DIGIT2;
                                break;
                            default:
                                high = low;
                                state = OTHER;
                                OtherProc();
                                break;
                        }
                        break;
                    case DIGIT2:
                        switch (type) {
                            case NOT:
                                inverse = true;
                                state = OTHER;
                                break;
                            case MINUS:
                                break;
                            case NUMBER:
                                high = number;
                                state = OTHER;
                                break;
                            default:
                                state = OTHER;
                                OtherProc();
                                break;
                        }
                        break;
                }
            }
        }
        
        /**
        * Generates a list of numbers from a string.
        * @param ranges the comma separated ranges
        * @param maxNumber the maximum number in the range
        * @return a list with the numbers as <CODE>Integer</CODE>
        */    
        public static ICollection<int> Expand(String ranges, int maxNumber) {
            SequenceList parse = new SequenceList(ranges);
            List<int> list = new List<int>();
            bool sair = false;
            while (!sair) {
                sair = parse.GetAttributes();
                if (parse.low == -1 && parse.high == -1 && !parse.even && !parse.odd)
                    continue;
                if (parse.low < 1)
                    parse.low = 1;
                if (parse.high < 1 || parse.high > maxNumber)
                    parse.high = maxNumber;
                if (parse.low > maxNumber)
                    parse.low = maxNumber;
                
                //System.out.Println("low="+parse.low+",high="+parse.high+",odd="+parse.odd+",even="+parse.even+",inverse="+parse.inverse);
                int inc = 1;
                if (parse.inverse) {
                    if (parse.low > parse.high) {
                        int t = parse.low;
                        parse.low = parse.high;
                        parse.high = t;
                    }
                    for (ListIterator<int> it = new ListIterator<int>(list); it.HasNext();) {
                        int n = it.Next();
                        if (parse.even && (n & 1) == 1)
                            continue;
                        if (parse.odd && (n & 1) == 0)
                            continue;
                        if (n >= parse.low && n <= parse.high)
                            it.Remove();
                    }
                }
                else {
                    if (parse.low > parse.high) {
                        inc = -1;
                        if (parse.odd || parse.even) {
                            --inc;
                            if (parse.even)
                                parse.low &= ~1;
                            else
                                parse.low -= ((parse.low & 1) == 1 ? 0 : 1);
                        }
                        for (int k = parse.low; k >= parse.high; k += inc) {
                            list.Add(k);
                        }
                    }
                    else {
                        if (parse.odd || parse.even) {
                            ++inc;
                            if (parse.odd)
                                parse.low |= 1;
                            else
                                parse.low += ((parse.low & 1) == 1 ? 1 : 0);
                        }
                        for (int k = parse.low; k <= parse.high; k += inc)
                            list.Add(k);
                    }
                }
            }
            return list;
        }
    }
}
