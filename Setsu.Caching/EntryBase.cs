using Setsu.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Setsu.Caching
{
    public abstract class EntryBase : IEntryBase
    {
        internal protected DateTime LastAccess { get; set; }

        public Option<DateTime> AbsoluteExpiration = new();
        public Option<TimeSpan> SlidingExpiration = new();

        bool IEntryBase.IsValid()
        {
            if (AbsoluteExpiration.HasValue)
            {
                if (AbsoluteExpiration.Value < DateTime.Now)
                    return false;
            }
            if (SlidingExpiration.HasValue)
            {
                if (LastAccess + SlidingExpiration.Value < DateTime.Now)
                    return false;
            }
            return true;
        }

        public void Access()
        {
            LastAccess = DateTime.Now;
        }

        public virtual EntryBase SetSlidingExpiration(TimeSpan expiration)
        {
            SlidingExpiration = new(expiration);
            return this;
        }

        public virtual EntryBase SetAbsoluteExpiration(DateTime expiration)
        {
            AbsoluteExpiration = new(expiration);
            return this;
        }
        public virtual EntryBase SetRelativeAbsoluteExpiration(TimeSpan expiration)
        {
            AbsoluteExpiration = new(DateTime.Now + expiration);
            return this;
        }

        IEntryBase IEntryBase.SetSlidingExpiration(TimeSpan expiration) =>
            this.SetSlidingExpiration(expiration);

        IEntryBase IEntryBase.SetAbsoluteExpiration(DateTime expiration) =>
            this.SetAbsoluteExpiration(expiration);

        IEntryBase IEntryBase.SetRelativeAbsoluteExpiration(TimeSpan expiration) =>
            this.SetRelativeAbsoluteExpiration(expiration);
    }
}
