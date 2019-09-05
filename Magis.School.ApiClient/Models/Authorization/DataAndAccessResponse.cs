using System;
using System.Collections.Generic;

namespace Magis.School.ApiClient.Models.Authorization
{
    public class DataAndAccessResponse<TData> where TData : class
    {
        public TData Entry { get; set; }

        public IDictionary<string, AccessAction> AvailableActions { get; set; }
    }
}
