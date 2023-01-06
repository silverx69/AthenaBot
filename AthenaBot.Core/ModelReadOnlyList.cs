using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AthenaBot
{
    /// <summary>
    /// A read-only list implementation that makes it easier to use the MVVM pattern more naturally while still being supported by built-in serializers.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ModelReadOnlyList<T> :
        ModelBase,
        IReadOnlyObservableCollection<T>
    {
        public T this[int index] => InnerList[index];

        public int Count => InnerList.Count;

        public object SyncRoot => InnerList.SyncRoot;

        bool ICollection.IsSynchronized => ((ICollection)InnerList).IsSynchronized;

        protected ModelList<T> InnerList
        {
            get;
            private set;
        }

        protected ModelReadOnlyList()
        {
            InnerList = new ModelList<T>();
        }

        public ModelReadOnlyList(ModelList<T> innerList)
        {
            InnerList = innerList ?? throw new ArgumentNullException(nameof(innerList));
            InnerList.PropertyChanged += InnerList_PropertyChanged;
        }

        private void InnerList_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaisePropertyChanged(e.PropertyName);
        }

        public void CopyTo(Array array, int index)
        {
            InnerList.CopyTo(array, index);
        }

        public void Sort(Comparison<T> comparison)
        {
            InnerList.Sort(comparison);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return InnerList.GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add { InnerList.CollectionChanged += value; }
            remove { InnerList.CollectionChanged -= value; }
        }
    }
}
