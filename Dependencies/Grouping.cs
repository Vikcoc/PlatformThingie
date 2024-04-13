using System.Collections;

namespace Dependencies
{
    public class Grouping<TKey, TValue> : IGrouping<TKey, TValue>
    {
        public required TKey Key { get; set; }
        public required IEnumerable<TValue> Values { get; set; }

        public IEnumerator<TValue> GetEnumerator()
            => Values.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => Values.GetEnumerator();
    }
}
