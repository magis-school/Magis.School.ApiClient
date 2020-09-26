using System.Collections;
using System.Collections.Generic;

namespace Magis.School.ApiClient.Models
{
    public class App
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public string Color { get; set; }

        public IDictionary<string, string> Icons { get; set; }

        public ICollection<string> FileExtensions { get; set; }

        public bool IsInternal { get; set; }

        public DockerVersion DockerVersion { get; set; }

        public OpsiVersion OpsiVersion { get; set; }

        public string AppSourceName { get; set; }

        public string AppSourceDisplayName { get; set; }
    }
}
