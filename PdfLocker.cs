using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;



// derived from iTextSharp package
// http://sourceforge.net/p/itextsharp/code/HEAD/tree/book/iTextExamplesWeb/iTextExamplesWeb/iTextInAction2Ed/Chapter12/EncryptionPdf.cs#l84
using iTextSharp.text;
using iTextSharp.text.pdf;



namespace CipherBox.Pdf
{
	/// <summary>
	/// password add and remove
	/// </summary>
	public static partial class PDFHelper
	{
        // API function: it is in pdf file format
        public static bool IsMyFile(string TestFile)
        {
            FileInfo fi = new FileInfo(TestFile);
            if (!fi.Exists) return false;

            // check file extension 
            if ( ! (fi.Extension.ToUpper() == ".PDF" || fi.Extension.ToUpper() == ".FDF") ) 
                return false;

            // look into file header?

            return true;
        }


        /**
         * Give you a verbose analysis of the permissions.
         * @param permissions the permissions value of a PDF file
         * @return a String that explains the meaning of the permissions value
         */
        public static String GetPermissionsVerbose(int permissions)
        {
            StringBuilder buf = new StringBuilder("Allowed:");
            if (((int)Permissions.ALLOW_PRINTING & permissions) == (int)Permissions.ALLOW_PRINTING) buf.Append(" Printing");
            if (((int)Permissions.ALLOW_MODIFY_CONTENTS & permissions) == (int)Permissions.ALLOW_MODIFY_CONTENTS) buf.Append(" ModifyContents");
            if (((int)Permissions.ALLOW_COPY & permissions) == (int)Permissions.ALLOW_COPY) buf.Append(" Copy");
            if (((int)Permissions.ALLOW_MODIFY_ANNOTATIONS & permissions) == (int)Permissions.ALLOW_MODIFY_ANNOTATIONS) buf.Append(" ModifyAnnotations");
            if (((int)Permissions.ALLOW_FILL_IN & permissions) == (int)Permissions.ALLOW_FILL_IN) buf.Append(" FillIn");
            if (((int)Permissions.ALLOW_SCREENREADERS & permissions) == (int)Permissions.ALLOW_SCREENREADERS) buf.Append(" ScreenReaders");
            if (((int)Permissions.ALLOW_ASSEMBLY & permissions) == (int)Permissions.ALLOW_ASSEMBLY) buf.Append(" Assembly");
            if (((int)Permissions.ALLOW_DEGRADED_PRINTING & permissions) == (int)Permissions.ALLOW_DEGRADED_PRINTING) buf.Append(" DegradedPrinting");
            return buf.ToString();
        }

        // Tells you if printing is allowed.
        public static bool IsPrintingAllowed(int permissions) { return ((int)Permissions.ALLOW_PRINTING & permissions) == (int)Permissions.ALLOW_PRINTING; }

        // Tells you if modifying content is allowed.
        public static bool IsModifyContentsAllowed(int permissions) { return ((int)Permissions.ALLOW_MODIFY_CONTENTS & permissions) == (int)Permissions.ALLOW_MODIFY_CONTENTS; }

        // Tells you if copying is allowed.
        public static bool IsCopyAllowed(int permissions) { return ((int)Permissions.ALLOW_COPY & permissions) == (int)Permissions.ALLOW_COPY; }

        // Tells you if modifying annotations is allowed.
        public static bool IsModifyAnnotationsAllowed(int permissions) { return ((int)Permissions.ALLOW_MODIFY_ANNOTATIONS & permissions) == (int)Permissions.ALLOW_MODIFY_ANNOTATIONS; }

        // Tells you if filling in fields is allowed.
        public static bool IsFillInAllowed(int permissions) { return ((int)Permissions.ALLOW_FILL_IN & permissions) == (int)Permissions.ALLOW_FILL_IN; }

        // Tells you if repurposing for screenreaders is allowed.
        public static bool IsScreenReadersAllowed(int permissions) { return ((int)Permissions.ALLOW_SCREENREADERS & permissions) == (int)Permissions.ALLOW_SCREENREADERS; }

        // Tells you if document assembly is allowed.
        public static bool IsAssemblyAllowed(int permissions) { return ((int)Permissions.ALLOW_ASSEMBLY & permissions) == (int)Permissions.ALLOW_ASSEMBLY; }

        // Tells you if degraded printing is allowed.
        public static bool IsDegradedPrintingAllowed(int permissions) { return ((int)Permissions.ALLOW_DEGRADED_PRINTING & permissions) == (int)Permissions.ALLOW_DEGRADED_PRINTING; }


        /// <summary>
        /// Encrypt a PDF document. All the content, links, outlines, etc, are kept.
        /// It is also possible to change the info dictionary.
        /// throws DocumentException or IOException on error
        /// </summary>
        /// <param name="reader">the PDF to read</param>
        /// <param name="os">output stream</param>
        /// <param name="type">encryption type, can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128 or ENCRYPTION_AES128.
        /// Optionally DO_NOT_ENCRYPT_METADATA can be ORed to output the metadata in cleartext</param>
        /// <param name="userPassword">user password, can be null or empty</param>
        /// <param name="ownerPassword">user password, can be null or empty. In such case the ownerPassword is replaced by a random string</param>
        /// <param name="permissions">user permissions, can be AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
        /// AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
        /// The permissions can be combined by ORing them.</param>
        /// <param name="newInfo">optional string map, to add or change the info dictionary. Entries with null values
        /// delete the key in the original info dictionary</param>
        private static void Encrypt(PdfReader reader, Stream os, 
            EncryptionTypes type, Permissions permissions, 
            String userPassword, String ownerPassword, 
            Dictionary<string, string> newInfo)
        {
            PdfStamper stamper = new PdfStamper(reader, os);
            stamper.SetEncryption(type, userPassword, ownerPassword, permissions);
            stamper.moreInfo = newInfo; // optional
            stamper.Close();
        }



        public static string GetEncryptionInfo(string filename)
        {
            string result = "";
            PdfReader reader = null;
            try
            {
                byte[] pwdBytes = System.Text.Encoding.UTF8.GetBytes("password");
                byte[] docBytes = File.ReadAllBytes(filename);

                reader = new PdfReader(docBytes, pwdBytes);
                result += "\nVersion: 1." + reader.PdfVersion.ToString();

                EncryptionTypes mode = reader.GetCryptoMode();
                int permissions = reader.GetCryptoPermissions();
                string permStr = GetPermissionsVerbose(permissions);
                mode &= EncryptionTypes.ENCRYPTION_MASK;
                switch (mode)
                {
                    case EncryptionTypes.STANDARD_ENCRYPTION_40:
                        result += "\nEncryption: Standard";
                        result += "\nKeySize: 40bits";
                        result += "\n" + permStr;
                        break;
                    case EncryptionTypes.STANDARD_ENCRYPTION_128:
                        result += "\nEncryption: Standard";
                        result += "\nKeySize: 128bits";
                        result += "\n" + permStr;
                        break;
                    case EncryptionTypes.ENCRYPTION_AES_128:
                        result += "\nEncryption: AES";
                        result += "\nKeySize: 128bits";
                        result += "\n" + permStr;
                        break;
                    case EncryptionTypes.ENCRYPTION_AES_256:
                        result += "\nEncryption: AES";
                        result += "\nKeySize: 256bits";
                        result += "\n" + permStr;
                        break;
                    case EncryptionTypes.NO_ENCRYPTION:
                        result += "\nEncryption: None";
                        result += "\n" + permStr;
                        break;
                    default:
                        result += "\nEncryption: None";
                        result += "\n" + permStr;
                        break;
                }
            }
            catch (Exception ex)
            {
                result += "\nNot available";
                Console.WriteLine(ex.Message);
            }

            return result;
        }




        // API function
        public static bool IsEncrypted(string filename)
        {
            if (!IsMyFile(filename))
                return false;

            // Opening a document will fail with an invalid password.
            try
            {
                byte[] docBytes = File.ReadAllBytes(filename);
                PdfReader reader = new PdfReader(docBytes);
                return false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return true;
            }
        }

        public static bool VerifyPassword(string filename, string password)
        {
            try
            {
                // try open
                if (!File.Exists(filename))
                    return false;

                // owner password is required, and opening a document will fail with an invalid password.
                byte[] passwordBytes = System.Text.Encoding.UTF8.GetBytes(password);
                byte[] docBytes = File.ReadAllBytes(filename);
                PdfReader reader = new PdfReader(docBytes, passwordBytes);
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
        }





        // API function: protect a pdf document with a password.
        // assumption: original pdf file does not have owner password
        public static bool AddPassword(string filename, string password)
        {
            if (!File.Exists(filename))
                return false;

            byte[] ownerpwdbytes = System.Text.ASCIIEncoding.UTF8.GetBytes(password);
            byte[] userpwdbytes = System.Text.ASCIIEncoding.UTF8.GetBytes(password); // do we need it??

            try
            {
                // In case the original pdf was with permission only,
                // PdfReader can still read file by such globally setting
                PdfReader.unethicalreading = true; 

                // read pdf document
                byte[] docBytes = File.ReadAllBytes(filename);
                PdfReader reader = new PdfReader(docBytes);

                byte[] docEncBytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (PdfStamper stamper = new PdfStamper(reader, ms))
                    {
                        stamper.SetEncryption(
                        userpwdbytes, ownerpwdbytes,
                        Permissions.ALLOW_PRINTING | Permissions.ALLOW_SCREENREADERS,
                        EncryptionTypes.ENCRYPTION_AES_128 | EncryptionTypes.DO_NOT_ENCRYPT_METADATA
                        );
                    }
                    docEncBytes = ms.ToArray();
                }

                File.WriteAllBytes(filename, docEncBytes);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        // API function: unprotect a document (if you know the password)
        public static bool RemovePassword(string filename, string password)
        {
            if (!File.Exists(filename))
                return false;

            // bug: if i step here, then go to the end, the password is successfully removed.
            // however, if i straightly run to the end, the password is not removed!
            PdfReader.unethicalreading = true; // globally override this permission checking mechanism

            try
            {
                byte[] ownerpwdBytes = System.Text.Encoding.ASCII.GetBytes(password); // ASCII??

                // read pdf document, with owner password required
                byte[] docBytes = File.ReadAllBytes(filename);
                PdfReader reader = new PdfReader(docBytes, ownerpwdBytes);
                //PdfReader reader = new PdfReader(filename, ownerpwdBytes);

                // remove all signatures. NOT yet tested!
                /*
                List<string> signatureNames = reader.AcroFields.GetSignatureNames();
                foreach (string sn in signatureNames)
                {
                    reader.AcroFields.RemoveField(sn);
                }
                reader.AcroForm.Remove(PdfName.SIGFLAGS);
                */


                byte[] docNewBytes = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    using (PdfStamper stamper = new PdfStamper(reader, ms))
                    {
                        // stamp this pdf without encryption
                        stamper.Writer.crypto = null; // bug fix: I also change PdfWriter.crypto to public
                    }

                    docNewBytes = ms.ToArray();
                }

                Console.WriteLine("File size: " + docBytes.Length.ToString() + " bytes ==> " + docNewBytes.Length.ToString());
                File.WriteAllBytes(filename, docNewBytes);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }


        public static bool ChangePassword(string filename, string oldPassword, string newPassword)
        {
            if (!File.Exists(filename))
                return false;

            if (!RemovePassword(filename, oldPassword)) return false;

            return AddPassword(filename, newPassword);
        }

    }
}
