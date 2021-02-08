using System;
using System.Collections;
using System.Text;
using System.IO;

using CipherBox.Cryptography;

namespace iTextSharp.text.pdf
{
    public enum Revisions
    {
        /// <summary>
        /// Undocumented and unsupported
        /// </summary>
        V0 = 0, 

        /// <summary>
        /// RC4 40-bit security. for compatibility with Acrobat 3 and 4 only. Use 128-bit encryption whenever possible
        /// </summary>
        V1 = 1, 

        /// <summary>
        /// STANDARD_ENCRYPTION_40 (or 128-bit?). default encryption for PDF 1.4 Acrobat 5.0
        /// </summary>
        V2 = 2, 

        /// <summary>
        /// STANDARD_ENCRYPTION_128
        /// </summary>
        V3 = 3, 

        /// <summary>
        /// AES_128. PDF 1.6 and above, Acroat 7.0 at least!
        /// </summary>
        V4 = 4, 

        /// <summary>
        /// AES_256. 
        /// </summary>
        V5 = 5, 
    }


    public class PdfEncryption
    {
        private Revisions revision;

        private static byte[] pad = {
        (byte)0x28, (byte)0xBF, (byte)0x4E, (byte)0x5E, (byte)0x4E, (byte)0x75,
        (byte)0x8A, (byte)0x41, (byte)0x64, (byte)0x00, (byte)0x4E, (byte)0x56,
        (byte)0xFF, (byte)0xFA, (byte)0x01, (byte)0x08, (byte)0x2E, (byte)0x2E,
        (byte)0x00, (byte)0xB6, (byte)0xD0, (byte)0x68, (byte)0x3E, (byte)0x80,
        (byte)0x2F, (byte)0x0C, (byte)0xA9, (byte)0xFE, (byte)0x64, (byte)0x53,
        (byte)0x69, (byte)0x7A};

        private static readonly byte[] salt = { (byte)0x73, (byte)0x41, (byte)0x6c, (byte)0x54 };
        internal static readonly byte[] metadataPad = { (byte)255, (byte)255, (byte)255, (byte)255 };

        internal byte[] key; // The encryption key for a particular object/generation
        internal int keySize; // The encryption key length for a particular object/generation

        internal byte[] mkey = new byte[0];         // The global encryption key 
        internal byte[] extra = new byte[5]; // Work area to prepare the object/generation bytes

        internal byte[] ownerKey = new byte[32]; // The encryption key for the owner 
        internal byte[] userKey = new byte[32]; // The encryption key for the user
        internal byte[] oeKey;
        internal byte[] ueKey;
        internal byte[] perms;

        internal IDigest md5; // The message digest algorithm MD5        

        /** The public key security handler for certificate encryption */
        //protected PdfPublicKeySecurityHandler publicKeyHandler = null;

        internal int permissions;
        public int GetPermissions() { return permissions; }

        internal byte[] documentID;
        public PdfObject FileID   { get { return CreateInfoId(documentID); } }

        internal static long seq = DateTime.Now.Ticks + Environment.TickCount;

        private int keyLength; // The generic key length. It may be 40 or 128. 

        private bool encryptMetadata;
        public bool IsMetadataEncrypted() { return encryptMetadata; }
        
        private bool embeddedFilesOnly;  // Indicates if only the embedded files have to be encrypted. @since 2.1.3
        public bool IsEmbeddedFilesOnly() { return embeddedFilesOnly; }

        private EncryptionTypes cryptoMode;
        public EncryptionTypes GetCryptoMode() { return cryptoMode; }


        public PdfEncryption()
        {
            md5 = DigestAlgorithms.GetDigestAlgorithm("MD5");
            //publicKeyHandler = new PdfPublicKeySecurityHandler();
        }

        public PdfEncryption(PdfEncryption enc) : this()
        {
            if (enc.key != null)
            {
                key = (byte[])enc.key.Clone();
            }
            keySize = enc.keySize;
            mkey = (byte[])enc.mkey.Clone();
            ownerKey = (byte[])enc.ownerKey.Clone();
            userKey = (byte[])enc.userKey.Clone();
            permissions = enc.permissions;
            if (enc.documentID != null)
            {
                documentID = (byte[])enc.documentID.Clone();
            }
            revision = enc.revision;
            keyLength = enc.keyLength;
            encryptMetadata = enc.encryptMetadata;
            embeddedFilesOnly = enc.embeddedFilesOnly;
            //publicKeyHandler = enc.publicKeyHandler;
        }

        public void SetCryptoMode(EncryptionTypes mode, int kl)
        {
            cryptoMode = mode;
            encryptMetadata = (mode & EncryptionTypes.DO_NOT_ENCRYPT_METADATA) != EncryptionTypes.DO_NOT_ENCRYPT_METADATA;
            embeddedFilesOnly = (mode & EncryptionTypes.EMBEDDED_FILES_ONLY) == EncryptionTypes.EMBEDDED_FILES_ONLY;
            mode &= EncryptionTypes.ENCRYPTION_MASK;
            switch (mode)
            {
                case EncryptionTypes.STANDARD_ENCRYPTION_40:
                    encryptMetadata = true;
                    embeddedFilesOnly = false;
                    keyLength = 40;
                    revision = Revisions.V2; // STANDARD_ENCRYPTION_40;
                    break;
                case EncryptionTypes.STANDARD_ENCRYPTION_128:
                    embeddedFilesOnly = false;
                    if (kl > 0)
                    {
                        keyLength = kl;
                    }
                    else
                    {
                        keyLength = 128;
                    }
                    revision = Revisions.V3; // STANDARD_ENCRYPTION_128;
                    break;
                case EncryptionTypes.ENCRYPTION_AES_128:
                    keyLength = 128;
                    revision = Revisions.V4; // AES_128;
                    break;
                case EncryptionTypes.ENCRYPTION_AES_256:
                    keyLength = 256;
                    keySize = 32;
                    revision = Revisions.V5; // AES_256;
                    break;
                default:
                    throw new ArgumentException("no.valid.encryption.mode");
            }
        }


        private byte[] PadPassword(byte[] userPassword)
        {
            byte[] userPad = new byte[32];
            if (userPassword == null)
            {
                Array.Copy(pad, 0, userPad, 0, 32);
            }
            else
            {
                Array.Copy(userPassword, 0, userPad, 0, Math.Min(userPassword.Length, 32));
                if (userPassword.Length < 32)
                {
                    Array.Copy(pad, 0, userPad, userPassword.Length, 32 - userPassword.Length);
                }
            }

            return userPad;
        }

        private byte[] ComputeOwnerKey(byte[] userPad, byte[] ownerPad)
        {
            byte[] ownerKey = new byte[32];

            byte[] digest = DigestAlgorithms.Digest("MD5", ownerPad);
            if (revision == Revisions.V3 || revision == Revisions.V4)
            {
                byte[] mkey = new byte[keyLength / 8];
                // only use for the input as many bit as the key consists of
                for (int k = 0; k < 50; ++k)
                {
                    Array.Copy(DigestAlgorithms.Digest("MD5", digest, 0, mkey.Length), 0, digest, 0, mkey.Length);
                }
                Array.Copy(userPad, 0, ownerKey, 0, 32);
                for (int i = 0; i < 20; ++i)
                {
                    for (int j = 0; j < mkey.Length; ++j)
                    {
                        mkey[j] = (byte)(digest[j] ^ i);
                    }
                    
                    RC4 rc4 = new RC4(mkey);
                    ownerKey = rc4.Encrypt(ownerKey);
                }
            }
            else
            {
                byte[] rc4key = new byte[5];
                Array.Copy(digest, 0, rc4key, 0, 5);
                RC4 rc4 = new RC4(rc4key);

                byte[] msg = new byte[userPad.Length];
                Array.Copy(userPad, 0, msg, 0, msg.Length);
                msg =   rc4.Encrypt(msg);
                Array.Copy(msg, 0, ownerKey, 0, msg.Length);
            }

            return ownerKey;
        }

        // ownerKey, documentID must be setuped
        private void SetupGlobalEncryptionKey(byte[] documentID, byte[] userPad, byte[] ownerKey, int permissions)
        {
            this.documentID = documentID;
            this.ownerKey = ownerKey;
            this.permissions = permissions;
            // use variable keylength
            mkey = new byte[keyLength / 8];

            //fixed by ujihara in order to follow PDF reference
            md5.Reset();
            md5.BlockUpdate(userPad, 0, userPad.Length);
            md5.BlockUpdate(ownerKey, 0, ownerKey.Length);

            byte[] ext = new byte[4];
            ext[0] = (byte)permissions;
            ext[1] = (byte)(permissions >> 8);
            ext[2] = (byte)(permissions >> 16);
            ext[3] = (byte)(permissions >> 24);
            md5.BlockUpdate(ext, 0, 4);
            if (documentID != null)
            {
                md5.BlockUpdate(documentID, 0, documentID.Length);
            }
            if (!encryptMetadata)
            {
                md5.BlockUpdate(metadataPad, 0, metadataPad.Length);
            }
            byte[] hash = new byte[md5.GetDigestSize()];
            md5.DoFinal(hash, 0);

            byte[] digest = new byte[mkey.Length];
            Array.Copy(hash, 0, digest, 0, mkey.Length);

            md5.Reset();
            // only use the really needed bits as input for the hash
            if (revision ==  Revisions.V3 || revision == Revisions.V4)
            {
                for (int k = 0; k < 50; ++k)
                {
                    Array.Copy(DigestAlgorithms.Digest("MD5", digest), 0, digest, 0, mkey.Length);
                }
            }
            Array.Copy(digest, 0, mkey, 0, mkey.Length);
        }

        // mkey must be setuped, use the revision to choose the setup method
        private void SetupUserKey()
        {
            if (revision == Revisions.V3 || revision == Revisions.V4)
            {
                md5.BlockUpdate(pad, 0, pad.Length);
                md5.BlockUpdate(documentID, 0, documentID.Length);
                byte[] digest = new byte[md5.GetDigestSize()];
                md5.DoFinal(digest, 0);
                md5.Reset();
                Array.Copy(digest, 0, userKey, 0, 16);
                for (int k = 16; k < 32; ++k)
                {
                    userKey[k] = 0;
                }
                for (int i = 0; i < 20; ++i)
                {
                    for (int j = 0; j < mkey.Length; ++j)
                    {
                        digest[j] = (byte)(mkey[j] ^ i);
                    }

                    byte[] rc4key = new byte[mkey.Length];
                    Array.Copy(digest, 0, rc4key, 0, rc4key.Length);
                    RC4 rc4 = new RC4(rc4key);
                    
                    byte[] msg = new byte[16];
                    Array.Copy(userKey, 0, msg, 0, msg.Length);
                    msg = rc4.Encrypt(msg);
                    Array.Copy(msg, 0, userKey, 0, msg.Length);
                }
            }
            else
            {
                RC4 rc4 = new RC4(mkey);
                byte[] msg = new byte[pad.Length];
                Array.Copy(pad, 0, msg, 0, msg.Length);
                msg = rc4.Encrypt(msg);
                Array.Copy(msg, 0, userKey, 0, msg.Length);
            }
        }

        // gets keylength and revision and uses revison to choose the initial values for permissions
        public void SetupAllKeys(byte[] userPassword, byte[] ownerPassword, int permissions)
        {
            if (ownerPassword == null || ownerPassword.Length == 0)
            {
                ownerPassword = DigestAlgorithms.Digest("MD5", CreateDocumentId());
            }
            md5.Reset();
            permissions |= (int)((revision == Revisions.V3 || revision == Revisions.V4 || revision == Revisions.V5) ? (uint)0xfffff0c0 : (uint)0xffffffc0);
            permissions &= unchecked((int)0xfffffffc);
            this.permissions = permissions;
            if (revision == Revisions.V5)
            {
                if (userPassword == null)
                {
                    userPassword = new byte[0];
                }
                documentID = CreateDocumentId();
                byte[] uvs = IVGenerator.GetIV(8);
                byte[] uks = IVGenerator.GetIV(8);
                key = IVGenerator.GetIV(32);
                // Algorithm 3.8.1
                IDigest md = DigestAlgorithms.GetDigestAlgorithm("SHA-256");
                md.BlockUpdate(userPassword, 0, Math.Min(userPassword.Length, 127));
                md.BlockUpdate(uvs, 0, uvs.Length);
                userKey = new byte[48];
                md.DoFinal(userKey, 0);
                System.Array.Copy(uvs, 0, userKey, 32, 8);
                System.Array.Copy(uks, 0, userKey, 40, 8);
                // Algorithm 3.8.2
                md.BlockUpdate(userPassword, 0, Math.Min(userPassword.Length, 127));
                md.BlockUpdate(uks, 0, uks.Length);
                byte[] tempDigest = new byte[32];
                md.DoFinal(tempDigest, 0);
                AESCipherCbcNoPad ac = new AESCipherCbcNoPad(true, tempDigest);
                ueKey = ac.ProcessBlock(key, 0, key.Length);
                // Algorithm 3.9.1
                byte[] ovs = IVGenerator.GetIV(8);
                byte[] oks = IVGenerator.GetIV(8);
                md.BlockUpdate(ownerPassword, 0, Math.Min(ownerPassword.Length, 127));
                md.BlockUpdate(ovs, 0, ovs.Length);
                md.BlockUpdate(userKey, 0, userKey.Length);
                ownerKey = new byte[48];
                md.DoFinal(ownerKey, 0);
                System.Array.Copy(ovs, 0, ownerKey, 32, 8);
                System.Array.Copy(oks, 0, ownerKey, 40, 8);
                // Algorithm 3.9.2
                md.BlockUpdate(ownerPassword, 0, Math.Min(ownerPassword.Length, 127));
                md.BlockUpdate(oks, 0, oks.Length);
                md.BlockUpdate(userKey, 0, userKey.Length);
                md.DoFinal(tempDigest, 0);
                ac = new AESCipherCbcNoPad(true, tempDigest);
                oeKey = ac.ProcessBlock(key, 0, key.Length);
                // Algorithm 3.10
                byte[] permsp = IVGenerator.GetIV(16);
                permsp[0] = (byte)permissions;
                permsp[1] = (byte)(permissions >> 8);
                permsp[2] = (byte)(permissions >> 16);
                permsp[3] = (byte)(permissions >> 24);
                permsp[4] = (byte)(255);
                permsp[5] = (byte)(255);
                permsp[6] = (byte)(255);
                permsp[7] = (byte)(255);
                permsp[8] = encryptMetadata ? (byte)'T' : (byte)'F';
                permsp[9] = (byte)'a';
                permsp[10] = (byte)'d';
                permsp[11] = (byte)'b';
                ac = new AESCipherCbcNoPad(true, key);
                perms = ac.ProcessBlock(permsp, 0, permsp.Length);
            }
            else
            {
                //PDF refrence 3.5.2 Standard Security Handler, Algorithum 3.3-1
                //If there is no owner password, use the user password instead.
                byte[] userPad = PadPassword(userPassword);
                byte[] ownerPad = PadPassword(ownerPassword);

                this.ownerKey = ComputeOwnerKey(userPad, ownerPad);
                documentID = CreateDocumentId();
                SetupByUserPad(this.documentID, userPad, this.ownerKey, permissions);
            }
        }


        public bool ReadKey(PdfDictionary enc, byte[] password)
        {
            // constants
           const int VALIDATION_SALT_OFFSET = 32;
           const int KEY_SALT_OFFSET = 40;
           const int SALT_LENGHT = 8;
           const int OU_LENGHT = 48;

           if (password == null)
            {
                password = new byte[0];
            }
            byte[] oValue = DocWriter.GetISOBytes(enc.Get(PdfName.O).ToString());
            byte[] uValue = DocWriter.GetISOBytes(enc.Get(PdfName.U).ToString());
            byte[] oeValue = DocWriter.GetISOBytes(enc.Get(PdfName.OE).ToString());
            byte[] ueValue = DocWriter.GetISOBytes(enc.Get(PdfName.UE).ToString());
            byte[] perms = DocWriter.GetISOBytes(enc.Get(PdfName.PERMS).ToString());
            bool isUserPass = false;
            IDigest md = DigestAlgorithms.GetDigestAlgorithm("SHA-256");
            md.BlockUpdate(password, 0, Math.Min(password.Length, 127));
            md.BlockUpdate(oValue, VALIDATION_SALT_OFFSET, SALT_LENGHT);
            md.BlockUpdate(uValue, 0, OU_LENGHT);
            byte[] hash = new byte[md.GetDigestSize()];
            md.DoFinal(hash, 0);

            bool isOwnerPass = CompareArray(hash, oValue, 32);
            AESCipherCbcNoPad ac;
            if (isOwnerPass)
            {
                md.BlockUpdate(password, 0, Math.Min(password.Length, 127));
                md.BlockUpdate(oValue, KEY_SALT_OFFSET, SALT_LENGHT);
                md.BlockUpdate(uValue, 0, OU_LENGHT);
                md.DoFinal(hash, 0);
                ac = new AESCipherCbcNoPad(false, hash);
                key = ac.ProcessBlock(oeValue, 0, oeValue.Length);
            }
            else
            {
                md.BlockUpdate(password, 0, Math.Min(password.Length, 127));
                md.BlockUpdate(uValue, VALIDATION_SALT_OFFSET, SALT_LENGHT);
                md.DoFinal(hash, 0);
                isUserPass = CompareArray(hash, uValue, 32);
                if (!isUserPass)
                    throw new BadPasswordException("bad.user.password");
                md.BlockUpdate(password, 0, Math.Min(password.Length, 127));
                md.BlockUpdate(uValue, KEY_SALT_OFFSET, SALT_LENGHT);
                md.DoFinal(hash, 0);
                ac = new AESCipherCbcNoPad(false, hash);
                key = ac.ProcessBlock(ueValue, 0, ueValue.Length);
            }
            ac = new AESCipherCbcNoPad(false, key);
            byte[] decPerms = ac.ProcessBlock(perms, 0, perms.Length);
            if (decPerms[9] != (byte)'a' || decPerms[10] != (byte)'d' || decPerms[11] != (byte)'b')
                throw new BadPasswordException("bad.user.password");
            permissions = (decPerms[0] & 0xff) | ((decPerms[1] & 0xff) << 8)
                    | ((decPerms[2] & 0xff) << 16) | ((decPerms[2] & 0xff) << 24);
            encryptMetadata = decPerms[8] == (byte)'T'; 
            return isOwnerPass;
        }

        private static bool CompareArray(byte[] a, byte[] b, int len)
        {
            for (int k = 0; k < len; ++k)
            {
                if (a[k] != b[k])  return false;
            }
            return true;
        }

        public static byte[] CreateDocumentId()
        {
            long time = DateTime.Now.Ticks + Environment.TickCount;
            long mem = GC.GetTotalMemory(false);
            String s = time + "+" + mem + "+" + (seq++);
            byte[] b = Encoding.ASCII.GetBytes(s);
            return DigestAlgorithms.Digest("MD5", b);
        }

        public void SetupByUserPassword(byte[] documentID, byte[] userPassword, byte[] ownerKey, int permissions)
        {
            SetupByUserPad(documentID, PadPassword(userPassword), ownerKey, permissions);
        }

        private void SetupByUserPad(byte[] documentID, byte[] userPad, byte[] ownerKey, int permissions)
        {
            SetupGlobalEncryptionKey(documentID, userPad, ownerKey, permissions);
            SetupUserKey();
        }

        public void SetupByOwnerPassword(byte[] documentID, byte[] ownerPassword, byte[] userKey, byte[] ownerKey, int permissions)
        {
            SetupByOwnerPad(documentID, PadPassword(ownerPassword), userKey, ownerKey, permissions);
        }

        private void SetupByOwnerPad(byte[] documentID, byte[] ownerPad, byte[] userKey, byte[] ownerKey, int permissions)
        {
            byte[] userPad = ComputeOwnerKey(ownerKey, ownerPad); //userPad will be set in this.ownerKey
            SetupGlobalEncryptionKey(documentID, userPad, ownerKey, permissions); //step 3
            SetupUserKey();
        }

        public void SetKey(byte[] key)
        {
            this.key = key;
        }

        public void SetupByEncryptionKey(byte[] key, int keylength)
        {
            mkey = new byte[keylength / 8];
            System.Array.Copy(key, 0, mkey, 0, mkey.Length);
        }

        public void SetHashKey(int number, int generation)
        {
            if (revision == Revisions.V5) return;

            md5.Reset();    //added by ujihara
            extra[0] = (byte)number;
            extra[1] = (byte)(number >> 8);
            extra[2] = (byte)(number >> 16);
            extra[3] = (byte)generation;
            extra[4] = (byte)(generation >> 8);
            md5.BlockUpdate(mkey, 0, mkey.Length);
            md5.BlockUpdate(extra, 0, extra.Length);
            if (revision == Revisions.V4)
            {
                md5.BlockUpdate(salt, 0, salt.Length);
            }
            key = new byte[md5.GetDigestSize()];
            md5.DoFinal(key, 0);
            md5.Reset();
            keySize = mkey.Length + 5;
            if (keySize > 16)
            {
                keySize = 16;
            }
        }

        public static PdfObject CreateInfoId(byte[] id)
        {
            ByteBuffer buf = new ByteBuffer(90);
            buf.Append('[').Append('<');
            for (int k = 0; k < 16; ++k)
            {
                buf.AppendHex(id[k]);
            }
            buf.Append('>').Append('<');
            id = CreateDocumentId();
            for (int k = 0; k < 16; ++k)
            {
                buf.AppendHex(id[k]);
            }
            buf.Append('>').Append(']');
            return new PdfLiteral(buf.ToByteArray());
        }

        public PdfDictionary GetEncryptionDictionary()
        {
            PdfDictionary dic = new PdfDictionary();

            dic.Put(PdfName.FILTER, PdfName.STANDARD);
            dic.Put(PdfName.O, new PdfLiteral(PdfContentByte.EscapeString(ownerKey)));
            dic.Put(PdfName.U, new PdfLiteral(PdfContentByte.EscapeString(userKey)));
            dic.Put(PdfName.P, new PdfNumber(permissions));
            dic.Put(PdfName.R, new PdfNumber((int)revision));
            if (revision == Revisions.V2 ) // STANDARD_ENCRYPTION_40)
            {
                dic.Put(PdfName.V, new PdfNumber(1));
            }
            else if (revision == Revisions.V3 && encryptMetadata)
            {
                dic.Put(PdfName.V, new PdfNumber(2));
                dic.Put(PdfName.LENGTH, new PdfNumber(128));
            }
            else if (revision == Revisions.V5)
            {
                if (!encryptMetadata)
                {
                    dic.Put(PdfName.ENCRYPTMETADATA, PdfBoolean.PDFFALSE);
                }
                dic.Put(PdfName.OE, new PdfLiteral(PdfContentByte.EscapeString(oeKey)));
                dic.Put(PdfName.UE, new PdfLiteral(PdfContentByte.EscapeString(ueKey)));
                dic.Put(PdfName.PERMS, new PdfLiteral(PdfContentByte.EscapeString(perms)));
                dic.Put(PdfName.V, new PdfNumber((int)revision));
                dic.Put(PdfName.LENGTH, new PdfNumber(256));
                PdfDictionary stdcf = new PdfDictionary();
                stdcf.Put(PdfName.LENGTH, new PdfNumber(32));
                if (embeddedFilesOnly)
                {
                    stdcf.Put(PdfName.AUTHEVENT, PdfName.EFOPEN);
                    dic.Put(PdfName.EFF, PdfName.STDCF);
                    dic.Put(PdfName.STRF, PdfName.IDENTITY);
                    dic.Put(PdfName.STMF, PdfName.IDENTITY);
                }
                else
                {
                    stdcf.Put(PdfName.AUTHEVENT, PdfName.DOCOPEN);
                    dic.Put(PdfName.STRF, PdfName.STDCF);
                    dic.Put(PdfName.STMF, PdfName.STDCF);
                }
                stdcf.Put(PdfName.CFM, PdfName.AESV3);
                PdfDictionary cf = new PdfDictionary();
                cf.Put(PdfName.STDCF, stdcf);
                dic.Put(PdfName.CF, cf);
            }
            else
            {
                if (!encryptMetadata)
                {
                    dic.Put(PdfName.ENCRYPTMETADATA, PdfBoolean.PDFFALSE);
                }
                dic.Put(PdfName.R, new PdfNumber((int)Revisions.V4));
                dic.Put(PdfName.V, new PdfNumber(4));
                dic.Put(PdfName.LENGTH, new PdfNumber(128));
                PdfDictionary stdcf = new PdfDictionary();
                stdcf.Put(PdfName.LENGTH, new PdfNumber(16));
                if (embeddedFilesOnly)
                {
                    stdcf.Put(PdfName.AUTHEVENT, PdfName.EFOPEN);
                    dic.Put(PdfName.EFF, PdfName.STDCF);
                    dic.Put(PdfName.STRF, PdfName.IDENTITY);
                    dic.Put(PdfName.STMF, PdfName.IDENTITY);
                }
                else
                {
                    stdcf.Put(PdfName.AUTHEVENT, PdfName.DOCOPEN);
                    dic.Put(PdfName.STRF, PdfName.STDCF);
                    dic.Put(PdfName.STMF, PdfName.STDCF);
                }
                if (revision == Revisions.V4)
                {
                    stdcf.Put(PdfName.CFM, PdfName.AESV2);
                }
                else
                {
                    stdcf.Put(PdfName.CFM, PdfName.V2);
                }
                PdfDictionary cf = new PdfDictionary();
                cf.Put(PdfName.STDCF, stdcf);
                dic.Put(PdfName.CF, cf);
            }

            return dic;
        }


        public OutputStreamEncryption GetEncryptionStream(Stream os)
        {
            return new OutputStreamEncryption(os, key, 0, keySize, revision);
        }

        public int CalculateStreamSize(int n)
        {
            if (revision == Revisions.V4  || revision == Revisions.V5)
                return (n & 0x7ffffff0) + 32;
            else
                return n;
        }

        public byte[] EncryptByteArray(byte[] b)
        {
            MemoryStream ba = new MemoryStream();
            OutputStreamEncryption os2 = GetEncryptionStream(ba);
            os2.Write(b, 0, b.Length);
            os2.Finish();
            return ba.ToArray();
        }

        public byte[] DecryptByteArray(byte[] b)
        {
            MemoryStream ba = new MemoryStream();
            StandardDecryption dec = new StandardDecryption(key, 0, keySize, revision);
            byte[] b2 = dec.Update(b, 0, b.Length);
            if (b2 != null)
            {
                ba.Write(b2, 0, b2.Length);
            }
            b2 = dec.Finish();
            if (b2 != null)
            {
                ba.Write(b2, 0, b2.Length);
            }
            return ba.ToArray();
        }

        public byte[] ComputeUserPassword(byte[] ownerPassword)
        {
            byte[] userPad = ComputeOwnerKey(ownerKey, PadPassword(ownerPassword));
            for (int i = 0; i < userPad.Length; i++)
            {
                bool match = true;
                for (int j = 0; j < userPad.Length - i; j++)
                {
                    if (userPad[i + j] != pad[j])
                    {
                        match = false;
                        break;
                    }
                }
                if (!match) continue;

                byte[] userPassword = new byte[i];
                System.Array.Copy(userPad, 0, userPassword, 0, i);
                return userPassword;
            }
            return userPad;
        }
    }
}