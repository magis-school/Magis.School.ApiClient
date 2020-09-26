using System.Collections.Generic;

namespace Magis.School.ApiClient.DataObjects.Contexts
{
    public class VncContainersDataObjectContext : DataObjectContext
    {
        public bool IncludeInternalContainers { get; }

        public VncContainersDataObjectContext(bool includeInternalContainers)
        {
            IncludeInternalContainers = includeInternalContainers;
        }

        /// <inheritdoc />
        public override bool Matches(IDictionary<string, object> otherContext) => (bool)otherContext["includeInternalContainers"] == IncludeInternalContainers;

        /// <inheritdoc />
        public override bool Equals(DataObjectContext other) => other is VncContainersDataObjectContext vncContainersDataObjectContext &&
                                                                vncContainersDataObjectContext.IncludeInternalContainers == IncludeInternalContainers;

        /// <inheritdoc />
        public override bool Equals(object obj) => Equals(obj as DataObjectContext);

        /// <inheritdoc />
        public override int GetHashCode() => IncludeInternalContainers ? 1 : 0;
    }
}
