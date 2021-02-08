using System;
using System.IO;

namespace iTextSharp.text.pdf
{
    /// <summary>
    /// <CODE>PdfObject</CODE> is the abstract baseclass of all PDF objects. 
    /// <P>
    /// PDF supports seven basic types of objects: bools, numbers, strings, names, arrays, dictionaries and streams. 
    /// In addition, PDF provides a null object. Objects may be labeled so that they can be referred to by other objects.<BR>
    /// All these basic PDF objects are described in the 'PDF Reference Manual version 1.3' Chapter 4 (pages 37-54).
    ///  @see        PdfNull
    ///  @see        Pdfbool
    ///  @see        PdfNumber
    ///  @see        PdfString
    ///  @see        PdfName
    ///  @see        PdfArray
    ///  @see        PdfDictionary
    ///  @see        PdfStream
    ///  @see        PdfIndirectReference
    /// </summary>
    public abstract class PdfObject : IComparable<PdfObject>
    {

        // static membervariables (all the possible types of a PdfObject)
        public const int BOOLEAN = 1;    // a possible type of <CODE>PdfObject</CODE>
        public const int NUMBER = 2;     // a possible type of <CODE>PdfObject</CODE>
        public const int STRING = 3;     // a possible type of <CODE>PdfObject</CODE> 
        public const int NAME = 4;       // a possible type of <CODE>PdfObject</CODE> 
        public const int ARRAY = 5;      // a possible type of <CODE>PdfObject</CODE> 
        public const int DICTIONARY = 6; // a possible type of <CODE>PdfObject</CODE> 
        public const int STREAM = 7;     // a possible type of <CODE>PdfObject</CODE> 
        public const int NULL = 8;       // a possible type of <CODE>PdfObject</CODE> 
        public const int INDIRECT = 10;  // a possible type of <CODE>PdfObject</CODE> 

        /** This is an empty string used for the <CODE>PdfNull</CODE>-object and for an empty <CODE>PdfString</CODE>-object. */
        public const string NOTHING = "";

        /** This is the default encoding to be used for converting strings into bytes and vice versa.
         * The default encoding is PdfDocEcoding.
         */
        public const string TEXT_PDFDOCENCODING = "PDF";

        /** This is the encoding to be used to output text in Unicode. */
        public const string TEXT_UNICODE = "UnicodeBig";

        // membervariables

        /** the content of this <CODE>PdfObject</CODE> */
        protected byte[] bytes;

        /** the type of this <CODE>PdfObject</CODE> */
        protected int type;

        /**
         * Holds value of property indRef.
         */
        protected PRIndirectReference indRef;

        /**
         * Hash code of the PdfObject instance. 
         * Unfortunately, default C# behavior does not generate unique hash code.
         */
        private readonly int hashCode = IncrementObjCounter();

        /**
         * Used for generating hash code.
         */
        static private int objCounter = 0;

        /**
         * Making hash code generation thread safe.
         */
        static readonly object locker = new object();


        // constructors

        /**
         * Constructs a <CODE>PdfObject</CODE> of a certain <VAR>type</VAR> without any <VAR>content</VAR>.
         *
         * @param        type            type of the new <CODE>PdfObject</CODE>
         */

        protected PdfObject(int type)
        {
            this.type = type;
        }

        /**
         * Constructs a <CODE>PdfObject</CODE> of a certain <VAR>type</VAR> with a certain <VAR>content</VAR>.
         *
         * @param        type            type of the new <CODE>PdfObject</CODE>
         * @param        content            content of the new <CODE>PdfObject</CODE> as a <CODE>String</CODE>.
         */

        protected PdfObject(int type, string content)
        {
            this.type = type;
            bytes = PdfEncodings.ConvertToBytes(content, null);
        }

        /**
         * Constructs a <CODE>PdfObject</CODE> of a certain <VAR>type</VAR> with a certain <VAR>content</VAR>.
         *
         * @param        type            type of the new <CODE>PdfObject</CODE>
         * @param        bytes            content of the new <CODE>PdfObject</CODE> as an array of <CODE>byte</CODE>.
         */

        protected PdfObject(int type, byte[] bytes)
        {
            this.bytes = bytes;
            this.type = type;
        }

        // methods dealing with the content of this object

        /**
         * Writes the PDF representation of this <CODE>PdfObject</CODE> as an array of <CODE>byte</CODE>s to the writer.
         * @param writer for backwards compatibility
         * @param os the outputstream to write the bytes to.
         * @throws IOException
         */

        public virtual void ToPdf(PdfWriter writer, Stream os)
        {
            if (bytes != null)
                os.Write(bytes, 0, bytes.Length);
        }

        /**
         * Gets the presentation of this object in a byte array
         * @return a byte array
         */
        public virtual byte[] GetBytes()
        {
            return bytes;
        }

        /**
         * Can this object be in an object stream?
         * @return true if this object can be in an object stream.
         */
        public bool CanBeInObjStm()
        {
            switch (type)
            {
                case NULL:
                case BOOLEAN:
                case NUMBER:
                case STRING:
                case NAME:
                case ARRAY:
                case DICTIONARY:
                    return true;
                case STREAM:
                case INDIRECT:
                default:
                    return false;
            }
        }

        /**
         * Returns the length of the PDF representation of the <CODE>PdfObject</CODE>.
         * <P>
         * In some cases, namely for <CODE>PdfString</CODE> and <CODE>PdfStream</CODE>,
         * this method differs from the method <CODE>length</CODE> because <CODE>length</CODE>
         * returns the length of the actual content of the <CODE>PdfObject</CODE>.</P>
         * <P>
         * Remark: the actual content of an object is in most cases identical to its representation.
         * The following statement is always true: Length() &gt;= PdfLength().</P>
         *
         * @return        a length
         */

        //    public int PdfLength() {
        //        return ToPdf(null).length;
        //    }

        /**
         * Returns the <CODE>String</CODE>-representation of this <CODE>PdfObject</CODE>.
         *
         * @return        a <CODE>String</CODE>
         */

        public override string ToString()
        {
            if (bytes == null)
                return "";
            else
                return PdfEncodings.ConvertToString(bytes, null);
        }

        /**
         * Returns the length of the actual content of the <CODE>PdfObject</CODE>.
         * <P>
         * In some cases, namely for <CODE>PdfString</CODE> and <CODE>PdfStream</CODE>,
         * this method differs from the method <CODE>pdfLength</CODE> because <CODE>pdfLength</CODE>
         * returns the length of the PDF representation of the object, not of the actual content
         * as does the method <CODE>length</CODE>.</P>
         * <P>
         * Remark: the actual content of an object is in some cases identical to its representation.
         * The following statement is always true: Length() &gt;= PdfLength().</P>
         *
         * @return        a length
         */

        public int Length
        {
            get
            {
                return ToString().Length;
            }
        }

        /**
         * Changes the content of this <CODE>PdfObject</CODE>.
         *
         * @param        content            the new content of this <CODE>PdfObject</CODE>
         */

        protected string Content
        {
            set
            {
                bytes = PdfEncodings.ConvertToBytes(value, null);
            }
        }

        // methods dealing with the type of this object
        public int Type { get { return type; } }
        public bool IsNull() { return (this.type == NULL); }
        public bool IsBoolean() { return (this.type == BOOLEAN); }
        public bool IsNumber() { return (this.type == NUMBER); }
        public bool IsString() { return (this.type == STRING); }
        public bool IsName() { return (this.type == NAME); }
        public bool IsArray() { return (this.type == ARRAY); }
        public bool IsDictionary() { return (this.type == DICTIONARY); }
        public bool IsStream() { return (this.type == STREAM); }
        public bool IsIndirect() { return (this.type == INDIRECT); }

        public PRIndirectReference IndRef
        {
            get
            {
                return indRef;
            }
            set
            {
                indRef = value;
            }
        }

        public int CompareTo(PdfObject obj)
        {
            return this.GetHashCode().CompareTo(obj.GetHashCode());
        }

        public override int GetHashCode()
        {
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            PdfObject pdfObject = obj as PdfObject;
            if (pdfObject == null) return false;
            return CompareTo(pdfObject) == 0;
        }

        static private int IncrementObjCounter()
        {
            lock (locker)
            {
                objCounter++;
                return objCounter;
            }

        }


    }
}
