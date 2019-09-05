using System;
using System.Collections.Generic;

namespace Magis.School.ApiClient.DataObjects.Contexts
{
    public abstract class DataObjectContext : IEquatable<DataObjectContext>
    {
        public abstract bool Matches(IDictionary<string, object> otherContext);

        public abstract bool Equals(DataObjectContext other);

        public abstract override bool Equals(object obj);

        public abstract override int GetHashCode();

        public static bool operator ==(DataObjectContext left, DataObjectContext right) => Equals(left, right);

        public static bool operator !=(DataObjectContext left, DataObjectContext right) => !Equals(left, right);
    }
}
