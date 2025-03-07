using System;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Text;

using CipherBox.Pdf.Utility;

namespace iTextSharp.text.pdf {

    /**
     * <CODE>PdfArray</CODE> is the PDF Array object.
     * <P>
     * An array is a sequence of PDF objects. An array may contain a mixture of object types.
     * An array is written as a left square bracket ([), followed by a sequence of objects,
     * followed by a right square bracket (]).<BR>
     * This object is described in the 'Portable Document Format Reference Manual version 1.3'
     * section 4.6 (page 40).
     *
     * @see        PdfObject
     */

    public class PdfArray : PdfObject, IEnumerable<PdfObject>, IEnumerable {
        
        // membervariables
        
    /** this is the actual array of PdfObjects */
        protected List<PdfObject> arrayList;
        
        // constructors
        
    /**
     * Constructs an empty <CODE>PdfArray</CODE>-object.
     */
        
        public PdfArray() : base(ARRAY) {
            arrayList = new List<PdfObject>();
        }
        
    /**
     * Constructs an <CODE>PdfArray</CODE>-object, containing 1 <CODE>PdfObject</CODE>.
     *
     * @param    object        a <CODE>PdfObject</CODE> that has to be added to the array
     */
        
        public PdfArray(PdfObject obj) : base(ARRAY) {
            arrayList = new List<PdfObject>();
            arrayList.Add(obj);
        }
        
        public PdfArray(float[] values) : base(ARRAY) {
            arrayList = new List<PdfObject>();
            Add(values);
        }
        
        public PdfArray(int[] values) : base(ARRAY) {
            arrayList = new List<PdfObject>();
            Add(values);
        }
        
        /**
         * Constructs a PdfArray with the elements of an ArrayList.
         * Throws a ClassCastException if the ArrayList contains something
         * that isn't a PdfObject.
         * @param   l   an ArrayList with PdfObjects
         * @since 2.1.3
         */
        public PdfArray(IList<PdfObject> l) : this() {
            foreach (PdfObject o in l)
                Add(o);
        }

    /**
     * Constructs an <CODE>PdfArray</CODE>-object, containing all the <CODE>PdfObject</CODE>s in a given <CODE>PdfArray</CODE>.
     *
     * @param    array        a <CODE>PdfArray</CODE> that has to be added to the array
     */
        
        public PdfArray(PdfArray array) : base(ARRAY) {
            arrayList = new List<PdfObject>(array.arrayList);
        }
        
        // methods overriding some methods in PdfObject
        
    /**
     * Returns the PDF representation of this <CODE>PdfArray</CODE>.
     *
     * @return        an array of <CODE>byte</CODE>s
     */
        
        public override void ToPdf(PdfWriter writer, Stream os) {
            os.WriteByte((byte)'[');
            bool first = true;
            PdfObject obj = null;
            int type = 0;
            foreach (PdfObject obja in arrayList) {
                obj = obja ?? PdfNull.PDFNULL;
                type = obj.Type;
                if (!first && type != PdfObject.ARRAY && type != PdfObject.DICTIONARY
                        && type != PdfObject.NAME && type != PdfObject.STRING)
                    os.WriteByte((byte)' ');
                first = false;
                obj.ToPdf(writer, os);
            }
            os.WriteByte((byte)']');
        }
        
        // methods concerning the ArrayList-membervalue
        
        // ARRAY CONTENT METHODS
        
        /**
         * Overwrites a specified location of the array.
         * 
         * @param idx The index of the element to be overwritten
         * @param obj new value for the specified index
         * @throws IndexOutOfBoundsException if the specified position doesn't exist
         * @return the previous value
         * @since 2.1.5
         */
        public PdfObject this[int idx] {
            get {
                return arrayList[idx];
            }
            set {
                arrayList[idx] = value;
            }
        }

        /**
         * Remove the element at the specified position from the array.
         * 
         * Shifts any subsequent elements to the left (subtracts one from their
         * indices).
         * 
         * @param idx The index of the element to be removed.
         * @throws IndexOutOfBoundsException the specified position doesn't exist
         * @since 2.1.5
         */
        public PdfObject Remove(int idx) {
            PdfObject tmp = arrayList[idx];
            arrayList.RemoveAt(idx);
            return tmp;
        }

    /**
     * Returns an ArrayList containing <CODE>PdfObject</CODE>s.
     *
     * @return        an ArrayList
     */
        
        public List<PdfObject> ArrayList {
            get {
                return arrayList;
            }
        }
        
    /**
     * Returns the number of entries in the array.
     *
     * @return        the size of the ArrayList
     */
        
        public int Size {
            get {
                return arrayList.Count;
            }
        }
        
        /**
         * Returns <CODE>true</CODE> if the array is empty.
         * 
         * @return <CODE>true</CODE> if the array is empty
         * @since 2.1.5
         */
        public bool IsEmpty() {
            return arrayList.Count == 0;
        }

    /**
     * Adds a <CODE>PdfObject</CODE> to the <CODE>PdfArray</CODE>.
     *
     * @param        object            <CODE>PdfObject</CODE> to add
     * @return        <CODE>true</CODE>
     */
        
        public virtual bool Add(PdfObject obj) {
            arrayList.Add(obj);
            return true;
        }
        
        public virtual bool Add(float[] values) {
            for (int k = 0; k < values.Length; ++k)
                arrayList.Add(new PdfNumber(values[k]));
            return true;
        }
        
        public virtual bool Add(int[] values) {
            for (int k = 0; k < values.Length; ++k)
                arrayList.Add(new PdfNumber(values[k]));
            return true;
        }
        
        /**
         * Inserts the specified element at the specified position.
         * 
         * Shifts the element currently at that position (if any) and
         * any subsequent elements to the right (adds one to their indices).
         *
         * @param index The index at which the specified element is to be inserted
         * @param element The element to be inserted
         * @throws IndexOutOfBoundsException if the specified index is larger than the
         *   last position currently set, plus 1. 
         * @since 2.1.5
         */
        public virtual void Add(int index, PdfObject element) {
            arrayList.Insert(index, element);
        }

        /**
         * Inserts a <CODE>PdfObject</CODE> at the beginning of the
         * <CODE>PdfArray</CODE>.
         * 
         * The <CODE>PdfObject</CODE> will be the first element, any other elements
         * will be shifted to the right (adds one to their indices).
         *
         * @param object The <CODE>PdfObject</CODE> to add
         */
        public virtual void AddFirst(PdfObject obj) {
            arrayList.Insert(0, obj);
        }
        
    /**
     * Checks if the <CODE>PdfArray</CODE> already contains a certain <CODE>PdfObject</CODE>.
     *
     * @param        object            <CODE>PdfObject</CODE> to check
     * @return        <CODE>true</CODE>
     */
        
        public bool Contains(PdfObject obj) {
            return arrayList.Contains(obj);
        }

        public ListIterator<PdfObject> GetListIterator() {
            return new ListIterator<PdfObject>(arrayList);
        }

        public override string ToString() {
            /*return arrayList.ToString();*/
            StringBuilder sb = new StringBuilder();
            sb.Append('[');
            for (int i = 0; i < arrayList.Count; i++) {
                PdfObject o = arrayList[i];
                if (o != null)
                    sb.Append(o.ToString());
                if (i < arrayList.Count - 1)
                    sb.Append(", ");
            }
            sb.Append(']');
            return sb.ToString();
        }

    //    public PdfObject GetPdfObject( int idx ) {
    //        return (PdfObject)arrayList[idx];
    //    }
        
        public PdfObject GetDirectObject( int idx ) {
            return PdfReader.GetPdfObject(this[idx]);
        }
        
        // more of the same like PdfDictionary. (MAS 2/17/06)
        public PdfDictionary GetAsDict(int idx) {
            PdfDictionary dict = null;
            PdfObject orig = GetDirectObject(idx);
            if (orig != null && orig.IsDictionary())
                dict = (PdfDictionary) orig;
            return dict;
        }
        
        public PdfArray GetAsArray(int idx) {
            PdfArray array = null;
            PdfObject orig = GetDirectObject(idx);
            if (orig != null && orig.IsArray())
                array = (PdfArray) orig;
            return array;
        }
        
        public PdfStream GetAsStream(int idx) {
            PdfStream stream = null;
            PdfObject orig = GetDirectObject(idx);
            if (orig != null && orig.IsStream())
                stream = (PdfStream) orig;
            return stream;
        }
        
        public PdfString GetAsString(int idx) {
            PdfString str = null;
            PdfObject orig = GetDirectObject(idx);
            if (orig != null && orig.IsString())
                str = (PdfString) orig;
            return str;
        }
        
        public PdfNumber GetAsNumber(int idx) {
            PdfNumber number = null;
            PdfObject orig = GetDirectObject(idx);
            if (orig != null && orig.IsNumber())
                number = (PdfNumber) orig;
            return number;
        }
        
        public PdfName GetAsName(int idx) {
            PdfName name = null;
            PdfObject orig = GetDirectObject(idx);
            if (orig != null && orig.IsName())
                name = (PdfName) orig;
            return name;
        }
        
        public PdfBoolean GetAsBoolean(int idx) {
            PdfBoolean b = null;
            PdfObject orig = GetDirectObject(idx);
            if (orig != null && orig.IsBoolean())
                b = (PdfBoolean) orig;
            return b;
        }
        
        public PdfIndirectReference GetAsIndirectObject(int idx) {
            PdfIndirectReference refi = null;
            PdfObject orig = this[idx]; // not getDirect this time.
            if (orig != null && orig.IsIndirect())
                refi = (PdfIndirectReference) orig;
            return refi;
        }

        public IEnumerator<PdfObject> GetEnumerator() {
            return arrayList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return arrayList.GetEnumerator();
        }

        /**
         * 
         * @return this PdfArray's values as a long[] 
         * @since 5.3.5
         */
        public long[] AsLongArray(){
            long[] rslt = new long[Size];
            for (int k = 0; k < rslt.Length; ++k) {
                rslt[k] = GetAsNumber(k).LongValue;
            }
            return rslt;
        }
    }
}
