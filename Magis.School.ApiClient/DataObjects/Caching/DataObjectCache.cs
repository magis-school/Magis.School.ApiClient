using System;
using System.Collections.Concurrent;
using Magis.School.ApiClient.DataObjects.Base;
using Magis.School.ApiClient.DataObjects.Contexts;

namespace Magis.School.ApiClient.DataObjects.Caching
{
    internal sealed class DataObjectCache : IDisposable
    {
        private readonly ConcurrentDictionary<CacheKey, IDataObject> _cache = new ConcurrentDictionary<CacheKey, IDataObject>();

        private bool _disposed;

        public TDataObject GetOrAddDataObject<TDataObject, TContext>(TContext context, Func<TContext, TDataObject> factoryFunc)
            where TDataObject : class, IDataObject where TContext : DataObjectContext
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));
            if (factoryFunc == null)
                throw new ArgumentNullException(nameof(factoryFunc));

            var key = new CacheKey(typeof(TDataObject), context);
            return (TDataObject)_cache.GetOrAdd(key, k => factoryFunc.Invoke(context));
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            foreach (IDataObject dataObject in _cache.Values)
                dataObject.Dispose();

            _disposed = true;
        }

        private class CacheKey : IEquatable<CacheKey>
        {
            public Type DataObjectType { get; }

            public DataObjectContext DataObjectContext { get; }

            public CacheKey(Type dataObjectType, DataObjectContext dataObjectContext)
            {
                DataObjectType = dataObjectType ?? throw new ArgumentNullException(nameof(dataObjectType));
                DataObjectContext = dataObjectContext ?? throw new ArgumentNullException(nameof(dataObjectContext));
            }

            public bool Equals(CacheKey other) => other != null && DataObjectType == other.DataObjectType && DataObjectContext == other.DataObjectContext;

            public override bool Equals(object obj) => Equals(obj as CacheKey);

            public override int GetHashCode() => (DataObjectType, DataObjectContext).GetHashCode();

            public static bool operator ==(CacheKey left, CacheKey right) => Equals(left, right);

            public static bool operator !=(CacheKey left, CacheKey right) => !Equals(left, right);
        }
    }
}
