using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;

using CipherBox.Pdf.Utility;
using iTextSharp.text.io;

namespace iTextSharp.text.pdf {

    public class GlyphList {
        private static Dictionary<int,string> unicode2names = new Dictionary<int,string>();
        private static Dictionary<string,int[]> names2unicode = new Dictionary<string,int[]>();
    
        static GlyphList() {
            Stream istr = null;
            try {
                istr = StreamUtil.GetResourceStream(BaseFont.RESOURCE_PATH + "glyphlist.txt");
                if (istr == null) {
                    String msg = "glyphlist.txt not found as resource.";
                    throw new Exception(msg);
                }
                byte[] buf = new byte[1024];
                MemoryStream outp = new MemoryStream();
                while (true) {
                    int size = istr.Read(buf, 0, buf.Length);
                    if (size == 0)
                        break;
                    outp.Write(buf, 0, size);
                }
                istr.Close();
                istr = null;
                String s = PdfEncodings.ConvertToString(outp.ToArray(), null);
                StringTokenizer tk = new StringTokenizer(s, "\r\n");
                while (tk.HasMoreTokens()) {
                    String line = tk.NextToken();
                    if (line.StartsWith("#"))
                        continue;
                    StringTokenizer t2 = new StringTokenizer(line, " ;\r\n\t\f");
                    String name = null;
                    String hex = null;
                    if (!t2.HasMoreTokens())
                        continue;
                    name = t2.NextToken();
                    if (!t2.HasMoreTokens())
                        continue;
                    hex = t2.NextToken();
                    int num = int.Parse(hex, NumberStyles.HexNumber);
                    unicode2names[num] = name;
                    names2unicode[name] = new int[]{num};
                }
            }
            catch (Exception e) {
                Console.Error.WriteLine("glyphlist.txt loading error: " + e.Message);
            }
            finally {
                if (istr != null) {
                    try {
                        istr.Close();
                    }
                    catch {
                        // empty on purpose
                    }
                }
            }
        }
    
        public static int[] NameToUnicode(string name) {
            int[] v;
            names2unicode.TryGetValue(name, out v);
            if (v == null && name.Length == 7 && name.ToLowerInvariant().StartsWith("uni")) {
                try {
                    return new int[]{int.Parse(name.Substring(3), NumberStyles.HexNumber)};
                }
                catch {
                }
            }
            return v;
        }
    
        public static string UnicodeToName(int num) {
            string a;
            unicode2names.TryGetValue(num, out a);
            return a;
        }
    }
}
