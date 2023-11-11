using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Setsu.Caching.SingleCache;

namespace Setsu.Caching.ListCache
{
    internal class ListCacheEntry<TVal> : EntryBase, IListCacheEntry<TVal> where TVal: notnull
    {
        private List<ICacheEntry<TVal>> values = [];
        public List<ICacheEntry<TVal>> Values
        {
            get
            {
                Access();
                return values;
            }
            set
            {
                Access();
                values = value;
            }
        }
        private readonly object _syncRoot = new();

        public ICacheEntry<TVal> AppendValue(TVal val)
        {
            Values ??= [];
            var entry = new CacheEntry<TVal>().SetValue(val);
            Values.Add(entry);
            entry.Access();
            Access();
            return entry;
        }

        public void Clear()
        {
            Values?.Clear();
        }

        public void Clean()
        {
            ICacheEntry<TVal>[] entries;
            lock (_syncRoot)
            {
                entries = Values.ToArray();
            }
            foreach (var entry in entries)
            {
                if (!entry.IsValid())
                    lock (_syncRoot)
                        Values.Remove(entry);
            }
        }
    }
}
