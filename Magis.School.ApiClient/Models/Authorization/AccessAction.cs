using System.Collections.Generic;

namespace Magis.School.ApiClient.Models.Authorization
{
    public class AccessAction
    {
        public string ActionName { get; set; }

        public bool Allowed { get; set; }

        public IDictionary<string, AccessField> Fields { get; set; }

        public IDictionary<string, bool> Constraints { get; set; }
    }
}
