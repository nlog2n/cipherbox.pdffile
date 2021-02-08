using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace iTextSharp.text.io 
{
    /**
     * Utility class with commonly used stream operations
     * @since 5.3.5
     *
     */
    public static class StreamUtil {

        /**
         * Reads the full content of a stream and returns them in a byte array
         * @param is the stream to read
         * @return a byte array containing all of the bytes from the stream
         * @throws IOException if there is a problem reading from the input stream
         */
        public static byte[] InputStreamToArray(Stream inp) 
        {
            byte[] b = new byte[8192];
            MemoryStream outp = new MemoryStream();
            while (true) {
                int read = inp.Read(b, 0, b.Length);
                if (read < 1)
                    break;
                outp.Write(b, 0, read);
            }
            outp.Close();
            return outp.ToArray();
        }
        
        public static void CopyBytes(IRandomAccessSource source, long start, long length, Stream outs) {
            if (length <= 0)
                return;
            long idx = start;
            byte[] buf = new byte[8192];
            while (length > 0) {
                long n = source.Get(idx, buf,0, (int)Math.Min((long)buf.Length, length));
                if (n <= 0)
                    throw new EndOfStreamException();
                outs.Write(buf, 0, (int)n);
                idx += n;
                length -= n;
            }
        }

        internal static List<object> resourceSearch = new List<object>();

        public static void AddToResourceSearch(object obj)
        {
            lock(resourceSearch)
            {
                if(obj is Assembly)
                {
                    resourceSearch.Add(obj);
                }
                else if(obj is string)
                {
                    string f = (string)obj;
                    if(Directory.Exists(f) || File.Exists(f))
                        resourceSearch.Add(obj);
                }
            }
        }

        /** Gets the font resources.
         * @param key the name of the resource
         * @return the <CODE>Stream</CODE> to get the resource or
         * <CODE>null</CODE> if not found
         */
        public static Stream GetResourceStream(string key)
        {
            Stream istr = null;
            // Try to use resource loader to load the properties file.
            try
            {
                Assembly assm = Assembly.GetExecutingAssembly();
                istr = assm.GetManifestResourceStream(key);
            }
            catch
            {
            }
            if(istr != null)
                return istr;
            int count;
            lock(resourceSearch) {
                count = resourceSearch.Count;
            }
            for(int k = 0; k < count; ++k) {
                object obj;
                lock(resourceSearch) {
                    obj = resourceSearch[k];
                }
                try {
                    if(obj is Assembly) {
                        istr = ((Assembly)obj).GetManifestResourceStream(key);
                        if(istr != null)
                            return istr;
                    } else if(obj is string) {
                        string dir = (string)obj;
                        try {
                            istr = Assembly.LoadFrom(dir).GetManifestResourceStream(key);
                        }
                        catch { }
                        if(istr != null)
                            return istr;
                        string modkey = key.Replace('.', '/');
                        string fullPath = Path.Combine(dir, modkey);
                        if(File.Exists(fullPath)) {
                            return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        }
                        int idx = modkey.LastIndexOf('/');
                        if(idx >= 0) {
                            modkey = modkey.Substring(0, idx) + "." + modkey.Substring(idx + 1);
                            fullPath = Path.Combine(dir, modkey);
                            if(File.Exists(fullPath))
                                return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
                        }
                    }
                } catch { }
            }

            return istr;
        }
    }
}