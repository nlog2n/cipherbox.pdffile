using System;

namespace iTextSharp.text.pdf 
{
    [Flags]
    public enum EncryptionTypes
    {
        NO_ENCRYPTION = 0, // added by fanghui

        // types of encryption
        STANDARD_ENCRYPTION_40  = 0x01, 
        STANDARD_ENCRYPTION_128 = 0x02, 
        ENCRYPTION_AES_128      = 0x04, 
        ENCRYPTION_AES_256      = 0x08, 

        // Mask to separate the encryption type from the encryption mode.
        ENCRYPTION_MASK         = 0x0F, 

        // Add this to the mode to keep the metadata in clear text 
        DO_NOT_ENCRYPT_METADATA = 0x10, 

        // Add this to the mode to keep encrypt only the embedded files.
        EMBEDDED_FILES_ONLY     = 0x30, 
    }


    // The operation permitted when the document is opened with the user password, See Table-22
    // also accept "int" input
    [Flags]
    public enum Permissions
    {
        ALLOW_DEGRADED_PRINTING  = 4,
        ALLOW_PRINTING           = 4 + 2048,
        ALLOW_MODIFY_CONTENTS    = 8,
        ALLOW_COPY               = 16,
        ALLOW_MODIFY_ANNOTATIONS = 32,
        ALLOW_FILL_IN            = 256,
        ALLOW_SCREENREADERS      = 512,
        ALLOW_ASSEMBLY           = 1024
    }




    /**
    * Encryption settings are described in section 3.5 (more specifically
    * section 3.5.2) of the PDF Reference 1.7.
    * They are explained in section 3.3.3 of the book 'iText in Action'.
    * The values of the different  preferences were originally stored
    * in class PdfWriter, but they have been moved to this separate interface
    * for reasons of convenience.
    */
    public interface IPdfEncryptionSettings 
    {
        /**
        * Sets the encryption options for this document. The userPassword and the
        * ownerPassword can be null or have zero length. In this case the ownerPassword
        * is replaced by a random string. The open permissions for the document can be
        * AllowPrinting, AllowModifyContents, AllowCopy, AllowModifyAnnotations,
        * AllowFillIn, AllowScreenReaders, AllowAssembly and AllowDegradedPrinting.
        * The permissions can be combined by ORing them.
        * @param userPassword the user password. Can be null or empty
        * @param ownerPassword the owner password. Can be null or empty
        * @param permissions the user permissions
        * @param encryptionType the type of encryption. It can be one of STANDARD_ENCRYPTION_40, STANDARD_ENCRYPTION_128 or ENCRYPTION_AES128.
        * Optionally DO_NOT_ENCRYPT_METADATA can be ored to output the metadata in cleartext
        * @throws DocumentException if the document is already open
        */
        void SetEncryption(byte[] userPassword, byte[] ownerPassword, Permissions permissions, EncryptionTypes encryptionType);

    }
}
