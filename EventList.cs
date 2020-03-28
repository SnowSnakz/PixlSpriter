using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixlSpriter
{
    public class EventList<T> : IList<T>
    {
        public T this[int index] 
        { 
            get 
            {
                T val = InternalList[index];
                if(OnGet != null) val = OnGet(index, val);
                return val;
            }
            set
            {
                T val = value;
                if (OnSet != null) val = OnSet(index, val);
                InternalList[index] = val;
            }
        }

        public event Func<int, T, T> OnGet;
        public event Func<int, T, T> OnSet;
        public event Func<int, T, T> OnAdd;
        public event Func<int, T, bool> OnRemove;
        public event Func<bool> OnClear;

        public int Count => InternalList.Count;

        public bool IsReadOnly => false;

        protected List<T> InternalList { get; } = new List<T>();

        public void Add(T item)
        {
            int index = Count;
            if (OnAdd != null) item = OnAdd(index, item);
            InternalList.Add(item);
        }

        public void Clear()
        {
            bool clear = true;
            if (OnClear != null) clear = OnClear();
            if (clear) InternalList.Clear();
        }

        public bool Contains(T item)
        {
            return InternalList.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            InternalList.CopyTo(array, arrayIndex);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return InternalList.GetEnumerator();
        }

        public int IndexOf(T item)
        {
            return InternalList.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (OnAdd != null) item = OnAdd(index, item);
            InternalList.Add(item);
        }

        public bool Remove(T item)
        {
            bool remove = true;
            if (OnRemove != null) remove = OnRemove(IndexOf(item), item);
            if (remove) return InternalList.Remove(item);
            return false;
        }

        public void RemoveAt(int index)
        {
            bool remove = true;
            if (OnRemove != null) remove = OnRemove(index, InternalList[index]);
            if (remove) InternalList.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InternalList.GetEnumerator();
        }
    }
}
