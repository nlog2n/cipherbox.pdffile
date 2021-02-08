using System;
using System.Collections.Generic;

namespace CipherBox.Pdf.Utility 
{
    /// <summary>
    /// Summary description for ListIterator.
    /// </summary>
    public class ListIterator<T> {
        IList<T> col;
        int cursor = 0;
        int lastRet = -1;

        public ListIterator(IList<T> col) {
            this.col = col;
        }

        public bool HasNext() {
            return cursor != col.Count;
        }

        public T Next() {
            T next = col[cursor];
            lastRet = cursor++;
            return next;
        }

        public T Previous() {
            int i = cursor - 1;
            T previous = col[i];
            lastRet = cursor = i;
            return previous;
        }

        public void Remove() {
            if (lastRet == -1)
                throw new InvalidOperationException();
            col.RemoveAt(lastRet);
            if (lastRet < cursor)
                cursor--;
            lastRet = -1;
        }
    }
}
