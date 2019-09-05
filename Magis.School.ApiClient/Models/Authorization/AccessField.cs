using System.Collections.Generic;

namespace Magis.School.ApiClient.Models.Authorization
{
    public class AccessField
    {
        public string FieldName { get; set; }

        public bool Allowed { get; set; }

        public IDictionary<string, bool> Constraints { get; set; }
    }
}
