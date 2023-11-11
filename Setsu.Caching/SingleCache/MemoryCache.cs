using System.Collections.Generic;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Setsu.Caching.SingleCache;



#if !NET35
using System.Threading.Tasks;
#endif
using System.Threading;

namespace Setsu.Caching.SingleCache
{
    public class MemoryCache<TKey, TVal> : CacheBase<TKey, ICacheEntry<TVal>> where TKey : notnull where TVal : notnull
    {
        public MemoryCache() : base() { }

        public ICacheEntry<TVal> Create(TKey key, TVal val)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            var entry = new CacheEntry<TVal>();
            entry.SetValue(val);
            base.Create(key, entry);
            return entry;
        }
        public ICacheEntry<TVal> Set(TKey key, TVal val)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            var entry = new CacheEntry<TVal>();
            entry.SetValue(val);
            base.Set(key, entry);
            return entry;
        }

        public bool TryGetValue(TKey key,
#if !(NETSTANDARD2_0 || NET472 || NET35)
            [NotNullWhen(true)]
#endif
        out TVal? value)
        {
            if (base.TryGetValue(key, out var entry))
            {
                value = entry!.Value;
                entry.Access();
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }
    }
}
