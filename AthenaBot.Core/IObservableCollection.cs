using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace AthenaBot
{
    /// <summary>
    /// A collection that implements INotifyPropertyChanged and INotifyCollectionChanged for MVVM and observer binding.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObservableCollection<T> :
        IList<T>,
        IEnumerable<T>,
        IObservableCollection
    {
        new T this[int index] { get; set; }

        bool Remove(Predicate<T> search);

        int RemoveAll(Predicate<T> search);

        void Sort(Comparison<T> comparison);
    }

    /// <summary>
    /// A collection that implements INotifyPropertyChanged and INotifyCollectionChanged for MVVM and observer binding.
    /// </summary>
    public interface IObservableCollection :
        IList,
        ICollection,
        IEnumerable,
        INotifyPropertyChanged,
        INotifyCollectionChanged
    {
    }

    /// <summary>
    /// A read-only collection that implements INotifyPropertyChanged and INotifyCollectionChanged for MVVM and observer binding.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReadOnlyObservableCollection<T> :
        IReadOnlyList<T>,
        IReadOnlyObservableCollection
    {
        new T this[int index] { get; }
        void Sort(Comparison<T> comparison);
    }

    /// <summary>
    /// A read-only collection that implements INotifyPropertyChanged and INotifyCollectionChanged for MVVM and observer binding.
    /// </summary>
    public interface IReadOnlyObservableCollection :
        ICollection,
        IEnumerable,
        INotifyPropertyChanged,
        INotifyCollectionChanged
    {
    }
}
