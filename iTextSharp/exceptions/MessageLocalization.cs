using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using iTextSharp.text.io;
using iTextSharp.text.pdf;

namespace iTextSharp.text.error_messages {

    /**
    * Localizes error messages. The messages are located in the package
    * com.lowagie.text.error_messages in the form language_country.lng.
    * The internal file encoding is UTF-8 without any escape chars, it's not a
    * normal property file. See en.lng for more information on the internal format.
    * @author Paulo Soares (psoares@glintt.com)
    */
    public class MessageLocalization {
        private static Dictionary<string,string> defaultLanguage = new Dictionary<string,string>();
        private static Dictionary<string,string> currentLanguage;
        private const String BASE_PATH = "CipherBox.Pdf.Resources";

        private MessageLocalization() {
        }

        static MessageLocalization() {
            try {
                defaultLanguage = GetLanguageMessages("en");
            } catch {
                // do nothing
            }
            if (defaultLanguage == null)
                defaultLanguage = new Dictionary<string,string>();
        }

        /**
        * Get a message without parameters.
        * @param key the key to the message
        * @return the message
        */
        public static String GetMessage(String key) 
        {
            Dictionary<string,string> cl = currentLanguage;
            String val;
            if (cl != null) {
                cl.TryGetValue(key, out val);
                if (val != null)
                    return val;
            }
            cl = defaultLanguage;
            cl.TryGetValue(key, out val);
            if (val != null)
                return val;
            return key;  // No message found
        }

        /**
        * Get a message with parameters. The parameters will replace the strings
        * "{1}", "{2}", ..., "{n}" found in the message.
        * @param key the key to the message
        * @param p the variable parameter
        * @return the message
        */
        public static String GetComposedMessage(String key, params object[] p) {
            String msg = GetMessage(key);
            for (int k = 0; k < p.Length; ++k) {
                msg = msg.Replace("{"+(k+1)+"}", p[k].ToString());
            }
            return msg;
        }

        /**
        * Sets the language to be used globally for the error messages. The language
        * is a two letter lowercase country designation like "en" or "pt". The country
        * is an optional two letter uppercase code like "US" or "PT".
        * @param language the language
        * @param country the country
        * @return true if the language was found, false otherwise
        * @throws IOException on error
        */
        public static bool SetLanguage(String language) {
            Dictionary<string,string> lang = GetLanguageMessages(language);
            if (lang == null)
                return false;
            currentLanguage = lang;
            return true;
        }

        /**
        * Sets the error messages directly from a Reader.
        * @param r the Reader
        * @throws IOException on error
        */
        public static void SetMessages(TextReader r) {
            currentLanguage = ReadLanguageStream(r);
        }

        private static Dictionary<string,string> GetLanguageMessages(String language) 
        {
            if (language == null)
                throw new ArgumentException("The language cannot be null.");
            Stream isp = null;
            try {
                String file = language + ".lng";
                isp = StreamUtil.GetResourceStream(BASE_PATH + file);
                if (isp != null)
                    return ReadLanguageStream(isp);
                else
                    return null;
            }
            finally {
                try {
                    isp.Close();
                } catch {
                }
                // do nothing
            }
        }

        private static Dictionary<string,string> ReadLanguageStream(Stream isp) {
            return ReadLanguageStream(new StreamReader(isp, Encoding.UTF8));
        }

        private static Dictionary<string,string> ReadLanguageStream(TextReader br) {
            Dictionary<string,string> lang = new Dictionary<string,string>();
            String line;
            while ((line = br.ReadLine()) != null) {
                int idxeq = line.IndexOf('=');
                if (idxeq < 0)
                    continue;
                String key = line.Substring(0, idxeq).Trim();
                if (key.StartsWith("#"))
                    continue;
                lang[key] = line.Substring(idxeq + 1);
            }
            return lang;
        }
    }
}
