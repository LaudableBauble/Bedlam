using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Library.Tools
{
    /// <summary>
    /// A robust list is just like any other list but with the notable exception that it can handle concurrent modification.
    /// </summary>
    /// <typeparam name="T">The type of object to list.</typeparam>
    public class RobustList<T> : IList<T>
    {
        #region Fields
        private List<T> _Items;
        private List<T> _ToAdd;
        private List<T> _ToRemove;
        #endregion

        #region Indexers
        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        public T this[int index]
        {
            get { return _Items[index]; }
            set { _Items[index] = value; }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor for a robust list.
        /// </summary>
        public RobustList()
        {
            //Initialize the lists.
            _Items = new List<T>();
            _ToAdd = new List<T>();
            _ToRemove = new List<T>();
        }
        #endregion

        #region Methods
        /// <summary>
        /// Update the list and go through with all modifications flagged for in the previous cycle. This has to be done if the list has been modified.
        /// </summary>
        /// <returns>Whether the list needed updating or not.</returns>
        public bool Update()
        {
            //The return value.
            bool answer = _ToAdd.Count > 0 || _ToRemove.Count > 0;

            //Add and remove items from the list.
            _Items.AddRange(_ToAdd);
            _Items.RemoveAll(item => _ToRemove.Contains(item));

            //Clear the modification lists.
            _ToAdd.Clear();
            _ToRemove.Clear();

            //Return the answer.
            return answer;
        }

        /// <summary>
        /// Searches for the specified object and returns the zero-based index of the first occurrence within the entire System.Collections.Generic.List.
        /// </summary>
        /// <param name="item">The object to locate in the System.Collections.Generic.List. The value can be null for reference types.</param>
        /// <returns>The zero-based index of the first occurrence of item within the entire System.Collections.Generic.List, if found; otherwise, –1.</returns>
        public int IndexOf(T item)
        {
            return _Items.IndexOf(item);
        }
        /// <summary>
        /// Cannot insert items into the list.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Removes the element at the specified index of the System.Collections.Generic.List.
        /// </summary>
        /// <param name="index">The zero-based index of the element to remove.</param>
        public void RemoveAt(int index)
        {
            _ToRemove.Add(_Items[index]);
        }
        /// <summary>
        /// Adds an object to the end of the System.Collections.Generic.List.
        /// </summary>
        /// <param name="item">The object to be added to the end of the System.Collections.Generic.List. The value can be null for reference types.</param>
        public void Add(T item)
        {
            _ToAdd.Add(item);
        }
        /// <summary>
        /// Removes all elements from the System.Collections.Generic.List.
        /// </summary>
        public void Clear()
        {
            _ToAdd.Clear();
            _ToRemove.AddRange(_Items);
        }
        /// <summary>
        /// Determines whether an element is in the System.Collections.Generic.List.
        /// </summary>
        /// <param name="item">The object to locate in the System.Collections.Generic.List<T>. The value can be null for reference types.</param>
        /// <returns>true if item is found in the System.Collections.Generic.List; otherwise, false.</returns>
        public bool Contains(T item)
        {
            return _Items.Contains(item);
        }
        /// <summary>
        /// Copies the entire System.Collections.Generic.List to a compatible one-dimensional array, starting at the specified index of the target array.
        /// </summary>
        /// <param name="array">The one-dimensional System.Array that is the destination of the elements copied from System.Collections.Generic.List.
        /// The System.Array must have zero-based indexing.</param>
        /// <param name="arrayIndex">The zero-based index in array at which copying begins.</param>
        public void CopyTo(T[] array, int arrayIndex)
        {
            _Items.CopyTo(array, arrayIndex);
        }
        /// <summary>
        /// Removes the first occurrence of a specific object from the System.Collections.Generic.List.
        /// </summary>
        /// <param name="item">The object to remove from the System.Collections.Generic.List. The value can be null for reference types.</param>
        /// <returns>true if item is successfully removed; otherwise, false. This method also returns false if item was not found in the System.Collections.Generic.List.</returns>
        public bool Remove(T item)
        {
            _ToRemove.Add(item);
            return _Items.Contains(item);
        }
        /// <summary>
        /// Get the last item in the list.
        /// </summary>
        public T GetLastItem()
        {
            if (_ToAdd.Count == 0) { return _Items[_Items.Count - 1]; }
            else { return _ToAdd[_ToAdd.Count - 1]; }
        }
        /// <summary>
        /// Sorts the elements in the entire System.Collections.Generic.List using the specified System.Comparison.
        /// </summary>
        /// <param name="comparison">The System.Comparison to use when comparing elements.</param>
        public void Sort(Comparison<T> comparison)
        {
            _Items.Sort(comparison);
        }
        /// <summary>
        /// Sorts the elements in the entire System.Collections.Generic.List using the specified comparer.
        /// </summary>
        /// <param name="comparer">The System.Collections.Generic.IComparer implementation to use when comparing elements, or null to use the default comparer System.Collections.Generic.Comparer.Default.</param>
        public void Sort(IComparer<T> comparer)
        {
            _Items.Sort(comparer);
        }
        /// <summary>
        /// Get a generic enumerator for this collection.
        /// </summary>
        /// <returns>A generic enumerator.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return _Items.GetEnumerator();
        }
        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <returns></returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the number of elements actually contained in the System.Collections.Generic.List.
        /// </summary>
        public int Count
        {
            get { return _Items.Count; }
        }
        /// <summary>
        /// Serves no useful function.
        /// </summary>
        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }
        /// <summary>
        /// The items of this list.
        /// </summary>
        public List<T> Items
        {
            get { return new List<T>(_Items); }
        }
        #endregion
    }
}