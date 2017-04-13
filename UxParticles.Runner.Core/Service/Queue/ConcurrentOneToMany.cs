namespace UxParticles.Runner.Core.Service.Queue
{
    using System.Collections.Concurrent;
    using System.Collections.Generic;

    /// <summary>
    ///     With this we cover the scenario where the
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class ConcurrentOneToMany<TKey, TElements>
    {
        private readonly ConcurrentDictionary<TKey, ConcurrentDictionary<TElements, byte>> underlyingList =
            new ConcurrentDictionary<TKey, ConcurrentDictionary<TElements, byte>>();

        /// <summary>
        /// Gets the total number of keys in the set
        /// </summary>
        public int Count => this.underlyingList.Count;


        public void AddOrUpdate(TKey key, TElements other)
        {
            this.underlyingList.AddOrUpdate(
                key, 
                k =>
                    {
                        var d = new ConcurrentDictionary<TElements, byte>();
                        d.AddOrUpdate(other, x => new byte(), (x, y) => new byte());
                        return d;
                    }, 
                (k, set) =>
                    {
                        set.AddOrUpdate(other, x => new byte(), (x, y) => new byte());
                        return set;
                    });
        }

        /// <summary>
        /// Gets the total number of uniqe items per key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>0 if the key does not exist, the number of items in the set otherwise</returns>
        public int CountByKey(TKey key)
        {
            ConcurrentDictionary<TElements, byte> value;
            if (!this.underlyingList.TryGetValue(key, out value))
            {
                return 0;
            }

            return value.Count;
        }


        public void RemoveElementFrom(TKey key, TElements other)
        {
            var relatedSet = this.underlyingList.GetOrAdd(key, new ConcurrentDictionary<TElements, byte>());
            byte removedEntry;
            relatedSet.TryRemove(other, out removedEntry);
        }
    }
}