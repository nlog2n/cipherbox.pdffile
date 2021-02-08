using System;
using System.Collections.Generic;
using System.Collections;

namespace CipherBox.Pdf.Utility.Collections 
{
    public class LinkedDictionary <TKey, TValue> : IDictionary<TKey, TValue> {
        private Dictionary<TKey, LinkedListNode<KeyValuePair<TKey,TValue>>> dic;
        private LinkedList<KeyValuePair<TKey,TValue>> link;

        public LinkedDictionary() {
            dic = new Dictionary<TKey,LinkedListNode<KeyValuePair<TKey,TValue>>>();
            link = new LinkedList<KeyValuePair<TKey,TValue>>();
        }

        public void Add(TKey key, TValue value) {
            LinkedListNode<KeyValuePair<TKey,TValue>> v = new LinkedListNode<KeyValuePair<TKey,TValue>>(new KeyValuePair<TKey,TValue>(key, value));
            dic.Add(key, v);
            link.AddLast(v);
        }

        public bool ContainsKey(TKey key) {
            return dic.ContainsKey(key);
        }

        public ICollection<TKey> Keys {
            get {
                return new KeyCollection(link, dic);
            }
        }

        public bool Remove(TKey key) {
            if (dic.ContainsKey(key)) {
                link.Remove(dic[key]);
                dic.Remove(key);
                return true;
            }
            else
                return false;
        }

        public bool TryGetValue(TKey key, out TValue value) {
            if (dic.ContainsKey(key)) {
                value = dic[key].Value.Value;
                return true;
            }
            else {
                value = default(TValue);
                return false;
            }
        }

        public ICollection<TValue> Values {
            get {
                return new ValueCollection(link);
            }
        }

        public TValue this[TKey key] {
            get {
                return dic[key].Value.Value;
            }
            set {
                if (dic.ContainsKey(key)) {
                    LinkedListNode<KeyValuePair<TKey,TValue>> old = dic[key];
                    LinkedListNode<KeyValuePair<TKey,TValue>> v = new LinkedListNode<KeyValuePair<TKey,TValue>>(new KeyValuePair<TKey,TValue>(key, value));
                    link.AddAfter(old, v);
                    link.Remove(old);
                    dic[key] = v;
                }
                else {
                    Add(key, value);
                }
            }
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item) {
            Add(item.Key, item.Value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear() {
            dic.Clear();
            link.Clear();
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item) {
            if (!dic.ContainsKey(item.Key))
                return false;
            return EqualityComparer<TValue>.Default.Equals(dic[item.Key].Value.Value, item.Value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            link.CopyTo(array, arrayIndex);
        }

        int ICollection<KeyValuePair<TKey, TValue>>.Count {
            get {
                return dic.Count;    
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly {
            get {
                return false;    
            }
        }

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item) {
            if (dic.ContainsKey(item.Key) && EqualityComparer<TValue>.Default.Equals(dic[item.Key].Value.Value, item.Value)) {
                this.Remove(item.Key);
                return true;
            }
            else 
                return false;
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator() {
            return link.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return link.GetEnumerator();
        }

        public sealed class ValueCollection : ICollection<TValue> {
            private LinkedList<KeyValuePair<TKey,TValue>> link;

            public ValueCollection(LinkedList<KeyValuePair<TKey,TValue>> link) {
                this.link = link;
            }

            public void Add(TValue item) {
                throw new NotSupportedException();
            }

            public void Clear() {
                throw new NotSupportedException();
            }

            public bool Contains(TValue item) {
                foreach (KeyValuePair<TKey,TValue> kv in link) {
                    if (EqualityComparer<TValue>.Default.Equals(item, kv.Value))
                        return true;
                }
                return false;
            }

            public void CopyTo(TValue[] array, int arrayIndex) {
                foreach (KeyValuePair<TKey,TValue> kv in link) {
                    array[arrayIndex++] = kv.Value;
                }
            }

            public int Count {
                get { 
                    return link.Count;    
                }
            }

            public bool IsReadOnly {
                get { return true; }
            }

            public bool Remove(TValue item) {
                throw new NotSupportedException();
            }

            public IEnumerator<TValue> GetEnumerator() {
                return new Enumerator(link);
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return new Enumerator(link);
            }

            public struct Enumerator : IEnumerator<TValue> {
                private IEnumerator<KeyValuePair<TKey,TValue>> enu;

                public Enumerator(LinkedList<KeyValuePair<TKey,TValue>> link) {
                    enu = link.GetEnumerator();
                }

                public TValue Current {
                    get { 
                        return enu.Current.Value;   
                    }
                }

                public void Dispose() {
                }

                object IEnumerator.Current {
                    get { 
                        return enu.Current.Value;   
                    }
                }

                public bool MoveNext() {
                    return enu.MoveNext();
                }

                public void Reset() {
                    enu.Reset();
                }
            }
        }

        public sealed class KeyCollection : ICollection<TKey> {
            private LinkedList<KeyValuePair<TKey,TValue>> link;
            private Dictionary<TKey, LinkedListNode<KeyValuePair<TKey,TValue>>> dic;

            public KeyCollection(LinkedList<KeyValuePair<TKey,TValue>> link, Dictionary<TKey, LinkedListNode<KeyValuePair<TKey,TValue>>> dic) {
                this.link = link;
                this.dic = dic;
            }

            public void Add(TKey item) {
                throw new NotSupportedException();
            }

            public void Clear() {
                throw new NotSupportedException();
            }

            public bool Contains(TKey item) {
                return dic.ContainsKey(item);
            }

            public void CopyTo(TKey[] array, int arrayIndex) {
                foreach (KeyValuePair<TKey,TValue> kv in link) {
                    array[arrayIndex++] = kv.Key;
                }
            }

            public int Count {
                get { 
                    return link.Count;    
                }
            }

            public bool IsReadOnly {
                get { return true; }
            }

            public bool Remove(TKey item) {
                throw new NotSupportedException();
            }

            public IEnumerator<TKey> GetEnumerator() {
                return new Enumerator(link);
            }

            IEnumerator IEnumerable.GetEnumerator() {
                return new Enumerator(link);
            }

            public struct Enumerator : IEnumerator<TKey> {
                private IEnumerator<KeyValuePair<TKey,TValue>> enu;

                public Enumerator(LinkedList<KeyValuePair<TKey,TValue>> link) {
                    enu = link.GetEnumerator();
                }

                public TKey Current {
                    get { 
                        return enu.Current.Key;   
                    }
                }

                public void Dispose() {
                }

                object IEnumerator.Current {
                    get { 
                        return enu.Current.Key;
                    }
                }

                public bool MoveNext() {
                    return enu.MoveNext();
                }

                public void Reset() {
                    enu.Reset();
                }
            }
        }
    }
}
