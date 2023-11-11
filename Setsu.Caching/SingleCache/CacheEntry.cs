using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Setsu.Options;

namespace Setsu.Caching.SingleCache
{
    internal sealed class CacheEntry<TVal> : EntryBase, ICacheEntry<TVal>
    {
        private TVal value;
        public TVal Value
        {
            get
            {
                Access();
                return value;
            }
            set
            {
                Access();
                this.value = value;
            }
        }
        internal DateTime LastAccess { get; set; }

        public Option<DateTime> AbsoluteExpiration = new();
        public Option<TimeSpan> SlidingExpiration = new();

        public ICacheEntry<TVal> SetValue(TVal value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            Value = value;
            LastAccess = DateTime.Now;
            return this;
        }
    }
}
