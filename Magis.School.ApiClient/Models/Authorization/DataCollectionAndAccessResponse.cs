using System.Collections.Generic;

namespace Magis.School.ApiClient.Models.Authorization
{
    public class DataCollectionAndAccessResponse<TData> where TData : class
    {
        public ICollection<DataAndAccessResponse<TData>> Entries { get; set; }

        public IDictionary<string, AccessAction> AvailableActions { get; set; }
    }
}
