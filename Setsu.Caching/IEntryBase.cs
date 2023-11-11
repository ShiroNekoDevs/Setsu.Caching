using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Setsu.Caching
{
    public interface IEntryBase
    {
        public bool IsValid();
        public void Access();

        public IEntryBase SetSlidingExpiration(TimeSpan expiration);
        public IEntryBase SetAbsoluteExpiration(DateTime expiration);
        public IEntryBase SetRelativeAbsoluteExpiration(TimeSpan expiration);
    }
}
