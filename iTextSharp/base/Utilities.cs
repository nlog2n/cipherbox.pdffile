using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using CipherBox.Pdf.Utility;
using iTextSharp.text.pdf;
namespace iTextSharp.text {

    /**
    * A collection of convenience methods that were present in many different iText
    * classes.
    */

    public class Utilities {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public static ICollection<K> GetKeySet<K,V>(Dictionary<K,V> table) {
            return (table == null) ? (ICollection<K>)new List<K>() : (ICollection<K>)table.Keys;
        }

        /**
        * Utility method to extend an array.
        * @param original the original array or <CODE>null</CODE>
        * @param item the item to be added to the array
        * @return a new array with the item appended
        */    
        public static Object[][] AddToArray(Object[][] original, Object[] item) {
            if (original == null) {
                original = new Object[1][];
                original[0] = item;
                return original;
            }
            else {
                Object[][] original2 = new Object[original.Length + 1][];
                Array.Copy(original, 0, original2, 0, original.Length);
                original2[original.Length] = item;
                return original2;
            }
        }

	    /**
	    * Checks for a true/false value of a key in a Properties object.
	    * @param attributes
	    * @param key
	    * @return
	    */
	    public static bool CheckTrueOrFalse(Properties attributes, String key) {
		    return Util.EqualsIgnoreCase("true", attributes[key]);
	    }

        /// <summary>
        /// This method makes a valid URL from a given filename.
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="filename">a given filename</param>
        /// <returns>a valid URL</returns>
        public static Uri ToURL(string filename) {
            try {
                return new Uri(filename);
            }
            catch {
                return new Uri(Path.GetFullPath(filename));
            }
        }
    
        /**
        * Unescapes an URL. All the "%xx" are replaced by the 'xx' hex char value.
        * @param src the url to unescape
        * @return the eunescaped value
        */    
        public static String UnEscapeURL(String src) {
            StringBuilder bf = new StringBuilder();
            char[] s = src.ToCharArray();
            for (int k = 0; k < s.Length; ++k) {
                char c = s[k];
                if (c == '%') {
                    if (k + 2 >= s.Length) {
                        bf.Append(c);
                        continue;
                    }
                    int a0 = PRTokeniser.GetHex((int)s[k + 1]);
                    int a1 = PRTokeniser.GetHex((int)s[k + 2]);
                    if (a0 < 0 || a1 < 0) {
                        bf.Append(c);
                        continue;
                    }
                    bf.Append((char)(a0 * 16 + a1));
                    k += 2;
                }
                else
                    bf.Append(c);
            }
            return bf.ToString();
        }
        
        private static byte[] skipBuffer = new byte[4096];

        /// <summary>
        /// This method is an alternative for the Stream.Skip()-method
        /// that doesn't seem to work properly for big values of size.
        /// </summary>
        /// <param name="istr">the stream</param>
        /// <param name="size">the number of bytes to skip</param>
        public static void Skip(Stream istr, int size) {
            while (size > 0) {
                int r = istr.Read(skipBuffer, 0, Math.Min(skipBuffer.Length, size));
                if (r <= 0)
                    return;
                size -= r;
            }
        }

        /**
        * Measurement conversion from millimeters to points.
        * @param    value   a value in millimeters
        * @return   a value in points
        * @since    2.1.2
        */
        public static float MillimetersToPoints(float value) {
            return InchesToPoints(MillimetersToInches(value));
        }

        /**
        * Measurement conversion from millimeters to inches.
        * @param    value   a value in millimeters
        * @return   a value in inches
        * @since    2.1.2
        */
        public static float MillimetersToInches(float value) {
            return value / 25.4f;
        }

        /**
        * Measurement conversion from points to millimeters.
        * @param    value   a value in points
        * @return   a value in millimeters
        * @since    2.1.2
        */
        public static float PointsToMillimeters(float value) {
            return InchesToMillimeters(PointsToInches(value));
        }

        /**
        * Measurement conversion from points to inches.
        * @param    value   a value in points
        * @return   a value in inches
        * @since    2.1.2
        */
        public static float PointsToInches(float value) {
            return value / 72f;
        }

        /**
        * Measurement conversion from inches to millimeters.
        * @param    value   a value in inches
        * @return   a value in millimeters
        * @since    2.1.2
        */
        public static float InchesToMillimeters(float value) {
            return value * 25.4f;
        }

        /**
        * Measurement conversion from inches to points.
        * @param    value   a value in inches
        * @return   a value in points
        * @since    2.1.2
        */
        public static float InchesToPoints(float value) {
            return value * 72f;
        }

        public static bool IsSurrogateHigh(char c) {
            return c >= '\ud800' && c <= '\udbff';
        }

        public static bool IsSurrogateLow(char c) {
            return c >= '\udc00' && c <= '\udfff';
        }

        public static bool IsSurrogatePair(string text, int idx) {
            if (idx < 0 || idx > text.Length - 2)
                return false;
            return IsSurrogateHigh(text[idx]) && IsSurrogateLow(text[idx + 1]);
        }

        public static bool IsSurrogatePair(char[] text, int idx) {
            if (idx < 0 || idx > text.Length - 2)
                return false;
            return IsSurrogateHigh(text[idx]) && IsSurrogateLow(text[idx + 1]);
        }

        public static int ConvertToUtf32(char highSurrogate, char lowSurrogate) {
             return (((highSurrogate - 0xd800) * 0x400) + (lowSurrogate - 0xdc00)) + 0x10000;
        }

        public static int ConvertToUtf32(char[] text, int idx) {
             return (((text[idx] - 0xd800) * 0x400) + (text[idx + 1] - 0xdc00)) + 0x10000;
        }

        public static int ConvertToUtf32(string text, int idx) {
             return (((text[idx] - 0xd800) * 0x400) + (text[idx + 1] - 0xdc00)) + 0x10000;
        }

        public static string ConvertFromUtf32(int codePoint) {
            if (codePoint < 0x10000)
                return Char.ToString((char)codePoint);
            codePoint -= 0x10000;
            return new string(new char[]{(char)((codePoint / 0x400) + 0xd800), (char)((codePoint % 0x400) + 0xdc00)});
        }

        /**
        * Reads the contents of a file to a String.
        * @param	path	the path to the file
        * @return	a String with the contents of the file
        * @since	iText 5.0.0
        */
	    public static String ReadFileToString(String path) {
            using (StreamReader sr = new StreamReader(path, Encoding.Default)) {
                return sr.ReadToEnd();
            }
	    }

        /**
         * Converts an array of bytes to a String of hexadecimal values
         * @param bytes	a byte array
         * @return	the same bytes expressed as hexadecimal values
         */
        public static String ConvertToHex(byte[] bytes) {
	        ByteBuffer buf = new ByteBuffer();
	        foreach (byte b in bytes) {
	            buf.AppendHex(b);
	        }
	        return PdfEncodings.ConvertToString(buf.ToByteArray(), null).ToUpper();
	    }

        public static float ComputeTabSpace(float lx, float rx, float tab) {
            return ComputeTabSpace(rx - lx, tab);
        }

        public static float ComputeTabSpace(float width, float tab) {
            width = (float)Math.Round(width, 3);
            tab = (float)Math.Round(tab, 3);

            return tab - width % tab;
        }
    }
}
