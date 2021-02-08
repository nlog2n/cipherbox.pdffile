using System;

namespace iTextSharp.text.factories 
{
    /**
    * This class can produce String combinations representing a number built with
    * Greek letters (from alpha to omega, then alpha alpha, alpha beta, alpha gamma).
    * We are aware of the fact that the original Greek numbering is different;
    * See http://www.cogsci.indiana.edu/farg/harry/lan/grknum.htm#ancient
    * but this isn't implemented yet; the main reason being the fact that we
    * need a font that has the obsolete Greek characters qoppa and sampi.
    */
    public class GreekAlphabetFactory {
        /** 
        * Changes an int into a lower case Greek letter combination.
        * @param index the original number
        * @return the letter combination
        */
        public static String GetString(int index) {
            return GetString(index, true);
        }
        
        /** 
        * Changes an int into a lower case Greek letter combination.
        * @param index the original number
        * @return the letter combination
        */
        public static String GetLowerCaseString(int index) {
            return GetString(index);        
        }
        
        /** 
        * Changes an int into a upper case Greek letter combination.
        * @param index the original number
        * @return the letter combination
        */
        public static String GetUpperCaseString(int index) {
            return GetString(index).ToUpper(System.Globalization.CultureInfo.InvariantCulture);
        }

        /** 
        * Changes an int into a Greek letter combination.
        * @param index the original number
        * @return the letter combination
        */
        public static String GetString(int index, bool lowercase) {
            if (index < 1) return "";
            index--;
                
            int bytes = 1;
            int start = 0;
            int symbols = 24;  
            while (index >= symbols + start) {
                bytes++;
                start += symbols;
                symbols *= 24;
            }
                  
            int c = index - start;
            char[] value = new char[bytes];
            while (bytes > 0) {
                bytes--;
                value[bytes] = (char)(c % 24);
                if (value[bytes] > 16) value[bytes]++;
                value[bytes] += (char)(lowercase ? 945 : 913);
                value[bytes] = SpecialSymbol.GetCorrespondingSymbol(value[bytes]);
                c /= 24;
            }
            
            return new String(value);
        }
    }
}
