using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UxParticles.Runner.Core.Service.Queue
{
    using System.Collections.Concurrent;

    /// <summary>
    /// With this we cover the scenario where the 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public class ConcurrentOneToMany<TKey, TElements>
    {
        private ConcurrentDictionary<TKey, ConcurrentDictionary<TElements, byte>> underlyingList = new ConcurrentDictionary<TKey, ConcurrentDictionary<TElements, byte>>();
         
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

        public void RemoveElementFrom(TKey key, TElements other)
        {
            var relatedSet = underlyingList.GetOrAdd(key, new ConcurrentDictionary<TElements, byte>());
            byte removedEntry;
            relatedSet.TryRemove(other, out removedEntry);
        }

        public int Count => this.underlyingList.Count;

        public int CountByKey(TKey key)
        {
            ConcurrentDictionary<TElements, byte> value;
            if (!this.underlyingList.TryGetValue(key, out value))
            {
                return 0;
            }

            return value.Count;
        }
    }
}
