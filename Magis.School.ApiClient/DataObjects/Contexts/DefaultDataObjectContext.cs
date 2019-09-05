using System.Collections.Generic;

namespace Magis.School.ApiClient.DataObjects.Contexts
{
    public class DefaultDataObjectContext : DataObjectContext
    {
        public override bool Matches(IDictionary<string, object> otherContext) => true;

        public override bool Equals(DataObjectContext other) => other is DefaultDataObjectContext;

        public override bool Equals(object obj) => Equals(obj as DataObjectContext);

        public override int GetHashCode() => 0;
    }
}
