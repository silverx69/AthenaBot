using System.Collections;

namespace AthenaBot
{
    public static partial class EnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action) {
            foreach (var item in collection)
                action(item);
        }

        public static bool Contains<T>(this IEnumerable<T> collection, Predicate<T> predicate) {
            foreach (T item in collection)
                if (predicate(item)) return true;

            return false;
        }

        public static T Find<T>(this IEnumerable<T> collection, Predicate<T> predicate) {
            foreach (var item in collection)
                if (predicate(item)) return item;

            return default;
        }

        public static int FindIndex<T>(this IEnumerable<T> collection, Predicate<T> predicate) {
            int index = -1;
            foreach (var item in collection) {
                ++index;
                if (predicate(item)) return index;
            }
            return -1;
        }

        public static IEnumerable<T> FindAll<T>(this IEnumerable<T> collection, Predicate<T> predicate) {
            var ret = new List<T>();

            foreach (var item in collection)
                if (predicate(item)) ret.Add(item);

            return ret;
        }

        public static IObservableCollection<TResult> Cast<TResult>(this IObservableCollection collection) {
            var cast = ((IEnumerable)collection).Cast<TResult>();
            if (cast is ModelList<TResult> list)
                return list;
            return new ModelList<TResult>(cast);
        }

        public static IObservableCollection<T> ToObservable<T>(this IEnumerable<T> collection) {
            if (collection is ModelList<T> list)
                return list;
            return new ModelList<T>(collection);
        }
    }
}
