using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Setsu.Caching.SingleCache;

namespace Setsu.Caching.ListCache
{
    public sealed class ListMemoryCache<TKey, TVal> : CacheBase<TKey, IListCacheEntry<TVal>> where TKey : notnull where TVal : notnull
    {
        public IListCacheEntry<TVal> Create(TKey key, TVal val, out ICacheEntry<TVal> createdEntry)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            var entry = new ListCacheEntry<TVal>();
            createdEntry = entry.AppendValue(val);
            base.Create(key, entry);
            return entry;
        }
        public IListCacheEntry<TVal> Set(TKey key, TVal val, out ICacheEntry<TVal> createdEntry)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            var entry = new ListCacheEntry<TVal>();
            createdEntry = entry.AppendValue(val);
            base.Set(key, entry);
            return entry;
        }
        public ICacheEntry<TVal> Append(TKey key, TVal value)
        {
            if (base.TryGetValue(key, out var entry))
            {
                return entry.AppendValue(value);
            }
            else
            {
                throw new KeyNotFoundException("The cache key was not found in this cache.");
            }
        }
        public bool TryAppend(TKey key, TVal value,
#if !(NETSTANDARD2_0 || NET472 || NET35)
            [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
#endif
            out ICacheEntry<TVal>? created)
        {
            if (base.TryGetValue(key, out var entry))
            {
                created = entry.AppendValue(value);
                return true;
            }
            created = null;
            return false;
        }
        public IListCacheEntry<TVal> CreateOrAppend(TKey key, TVal value, out ICacheEntry<TVal> createdEntry)
        {
            if (base.TryGetValue(key, out var entry))
            {
                createdEntry = entry!.AppendValue(value);
                return entry;
            }
            return Create(key, value, out createdEntry);
        }

        public bool TryGetValue(TKey key,
#if !(NETSTANDARD2_0 || NET472 || NET35)
            [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
#endif
        out IListCacheEntry<TVal>? value)
        {
            if (base.TryGetValue(key, out var entry))
            {
                entry!.Access();
                value = entry;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        public override void Clean()
        {
            try
            {
                // clone so we can do this async
                KeyValuePair<TKey, IListCacheEntry<TVal>>[] entries;
                lock (_syncRoot)
                {
                    entries = _entries.ToArray();
                }
                foreach (var entry in entries)
                {
                    if (!entry.Value.IsValid())
                        lock (_syncRoot)
                            _entries.Remove(entry.Key);
                    else
                    {
                        entry.Value.Clean();
                        if (!entry.Value.IsValid())
                            lock (_syncRoot)
                                _entries.Remove(entry.Key);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
