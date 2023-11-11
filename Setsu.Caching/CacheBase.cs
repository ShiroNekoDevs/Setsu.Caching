using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
#if !NET35
using System.Threading.Tasks;
#endif

namespace Setsu.Caching
{
    public abstract class CacheBase<TKey, TEntry> where TKey: notnull where TEntry: IEntryBase
    {
        protected readonly Dictionary<TKey, TEntry> _entries = [];
        protected readonly object _syncRoot = new();
        protected DateTime _lastWipe = DateTime.Now;
        protected TimeSpan wipeInterval;
#if !NET35
        private Task? previous = null;
        public TimeSpan WipeInterval
        {
            get => wipeInterval;
            set
            {
                cts?.Cancel();
                // lets hope this won't bite me in the behind later
                previous?.ConfigureAwait(false).GetAwaiter().GetResult();
                cts = new();
                wipeInterval = value;
                previous = Task.Run(async () =>
                {
                    try
                    {
                        while (!cts.IsCancellationRequested)
                        {
                            await Task.Delay(WipeInterval, cts.Token);
                            Clean();
                        }
                    }
                    catch (TaskCanceledException)
                    {

                    }
                }, cts.Token);
            }
        }
        private CancellationTokenSource? cts = null;
#else
        public TimeSpan WipeInterval { get => wipeInterval;
            set
            {
                shouldStop = true;
                new Thread(() =>
                {
                    while (!shouldStop)
                    {
                        Thread.Sleep(WipeInterval);
                        Clean();
                    }
                    shouldStop = false;
                }).Start();
            }
        }
        private bool shouldStop = false;
#endif

        public CacheBase()
        {
#if NET35
            wipeInterval = TimeSpan.FromMinutes(5);
            new Thread(() =>
            {
                while (!shouldStop)
                {
                    Thread.Sleep(WipeInterval);
                    Clean();
                }
                shouldStop = false;
            }).Start();
#else
            WipeInterval = TimeSpan.FromMinutes(5);
#endif
        }

        protected virtual void Create(TKey key, TEntry entry)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            lock (_syncRoot)
                _entries.Add(key, entry);
        }

        protected virtual void Set(TKey key, TEntry entry)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            lock (_syncRoot)
                _entries[key] = entry;
        }

        protected virtual bool TryGetValue(TKey key,
#if !(NETSTANDARD2_0 || NET472 || NET35)
            [System.Diagnostics.CodeAnalysis.NotNullWhen(true)]
#endif
            out TEntry? value)
        {
            TEntry? entry;
            bool exists;
            lock (_syncRoot)
            {
                exists = _entries.TryGetValue(key, out entry);
            }
            if (!exists)
            {
                value = default;
                return false;
            }
            if (!entry.IsValid())
            {
                value = default;
                return false;
            }
            value = entry;
            return true;
        }

        public virtual bool Exists(TKey key)
        {
            return _entries.ContainsKey(key);
        }

        public virtual void Clean()
        {
            // clone so we can do this async
            foreach (var entry in _entries.ToArray())
            {
                if (!entry.Value.IsValid())
                    lock (_syncRoot)
                        _entries.Remove(entry.Key);
            }
        }
    }
}
