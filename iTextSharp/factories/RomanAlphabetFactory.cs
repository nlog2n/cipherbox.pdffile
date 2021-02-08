using System;

namespace iTextSharp.text.factories 
{
    /**
    * This class can produce String combinations representing a number.
    * "a" to "z" represent 1 to 26, "AA" represents 27, "AB" represents 28,
    * and so on; "ZZ" is followed by "AAA".
    */
    public class RomanAlphabetFactory {

        /**
        * Translates a positive integer (not equal to zero)
        * into a String using the letters 'a' to 'z';
        * 1 = a, 2 = b, ..., 26 = z, 27 = aa, 28 = ab,...
        */
        public static String GetString(int index) {
            if (index < 1) throw new FormatException("you.can.t.translate.a.negative.number.into.an.alphabetical.value");
            
            index--;
            int bytes = 1;
            int start = 0;
            int symbols = 26;  
            while (index >= symbols + start) {
                bytes++;
                start += symbols;
                symbols *= 26;
            }
                  
            int c = index - start;
            char[] value = new char[bytes];
            while (bytes > 0) {
                value[--bytes] = (char)( 'a' + (c % 26));
                c /= 26;
            }
            
            return new String(value);
        }
        
        /**
        * Translates a positive integer (not equal to zero)
        * into a String using the letters 'a' to 'z';
        * 1 = a, 2 = b, ..., 26 = z, 27 = aa, 28 = ab,...
        */
        public static String GetLowerCaseString(int index) {
            return GetString(index);        
        }
        
        /**
        * Translates a positive integer (not equal to zero)
        * into a String using the letters 'A' to 'Z';
        * 1 = A, 2 = B, ..., 26 = Z, 27 = AA, 28 = AB,...
        */
        public static String GetUpperCaseString(int index) {
            return GetString(index).ToUpper(System.Globalization.CultureInfo.InvariantCulture);
        }

        
        /**
        * Translates a positive integer (not equal to zero)
        * into a String using the letters 'a' to 'z'
        * (a = 1, b = 2, ..., z = 26, aa = 27, ab = 28,...).
        */
        public static String GetString(int index, bool lowercase) {
            if (lowercase) {
                return GetLowerCaseString(index);
            }
            else {
                return GetUpperCaseString(index);
            }
        }
    }
}
