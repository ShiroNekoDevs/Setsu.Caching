using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Setsu.Caching.SingleCache;

namespace Setsu.Caching.ListCache
{
    public interface IListCacheEntry<TVal> : IEntryBase where TVal : notnull
    {
        public List<ICacheEntry<TVal>> Values { get; }
        public ICacheEntry<TVal> AppendValue(TVal val);
        public void Clear();
        public void Clean();
    }
}
