using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Setsu.Caching.SingleCache
{
    public interface ICacheEntry<TVal> : IEntryBase where TVal : notnull
    {
        public TVal Value { get; }
        public ICacheEntry<TVal> SetValue(TVal value);
    }
}
